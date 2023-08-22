using Epica.Web.Operacion.Config;
using Epica.Web.Operacion.Helpers;
using Epica.Web.Operacion.Models.Common;
using Epica.Web.Operacion.Models.Entities;
using Epica.Web.Operacion.Models.Request;
using Epica.Web.Operacion.Models.ViewModels;
using Epica.Web.Operacion.Services.Log;
using Epica.Web.Operacion.Services.Transaccion;
using Epica.Web.Operacion.Services.UserResolver;
using Epica.Web.Operacion.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using static Epica.Web.Operacion.Controllers.CuentaController;

namespace Epica.Web.Operacion.Controllers;

[Authorize]
public class TarjetasController : Controller
{
    #region "Locales"
    private readonly IClientesApiClient _clientesApiClient;
    private readonly ITarjetasApiClient _tarjetasApiClient;
    private readonly ILogsApiClient _logsApiClient;
    private readonly UserContextService _userContextService;
    #endregion

    #region "Constructores"
    public TarjetasController(IClientesApiClient clientesApiClient,
        ITarjetasApiClient tarjetasApiClient,
        UserContextService userContextService,
        ILogsApiClient logsApiClient)
    {
        _clientesApiClient = clientesApiClient;
        _userContextService = userContextService;
        _tarjetasApiClient = tarjetasApiClient;
        _logsApiClient = logsApiClient;
    }
    #endregion

    #region "Funciones"
    [Authorize]
    public IActionResult Index()
    {
        var loginResponse = _userContextService.GetLoginResponse();
        var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Tarjetas" && modulo.Ver == 0);
        if (validacion == true || validacion != null) return View(loginResponse);

        return NotFound();
    }

    [Authorize]
    public async Task<ActionResult> RegistroTarjetaCliente()
    {
        var loginResponse = _userContextService.GetLoginResponse();
        var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Tarjetas" && modulo.Insertar == 0);
        if (validacion == true || validacion != null)
        {
            TarjetasRegistroTarjetaClienteViewModel taReTarjetaClienteViewModel = new TarjetasRegistroTarjetaClienteViewModel
            {
                TarjetasDetalles = new RegistrarTarjetaRequest()
            };

            ViewBag.Accion = "RegistrarTarjetaCliente";
            ViewBag.TituloForm = "Registar tarjeta a cliente";
            return View(taReTarjetaClienteViewModel);
        }

        return NotFound();
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult>BuscarClientes(string cliente)
    {
        var listaClientes = await _clientesApiClient.GetClientesbyNombreAsync(cliente);
        return Json(listaClientes);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> RegistrarTarjetaCliente(TarjetasRegistroTarjetaClienteViewModel model)
    {
        var loginResponse = _userContextService.GetLoginResponse();
        var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Tarjetas" && modulo.Insertar == 0);
        if (validacion == true || validacion != null)
        {
            try
            {
                var response = await _tarjetasApiClient.GetRegistroTarjetaAsync(model.TarjetasDetalles);

                LogRequest logRequest = new LogRequest
                {
                    IdUser = loginResponse.IdUsuario.ToString(),
                    Modulo = "Tarjetas",
                    Fecha = DateTime.Now,
                    NombreEquipo = Environment.MachineName,
                    Accion = "Insertar",
                    Ip = PublicIpHelper.GetPublicIp() ?? "0.0.0.0",
                    Envio = JsonConvert.SerializeObject(model.TarjetasDetalles),
                    Respuesta = (response == "Exitoso") ? "true" : "false",
                    Error = response,
                    IdRegistro = 0
                };
                await _logsApiClient.InsertarLogAsync(logRequest);

                if (response == "Exitoso")
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("RegistroTarjetaCliente");
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        return NotFound();
    }

    [Authorize]
    [HttpPost]
    public async Task<JsonResult> Consulta(List<RequestListFilters> filters)
    {
        var request = new RequestList();

        int totalRecord = 0;
        int filterRecord = 0;

        var draw = Request.Form["draw"].FirstOrDefault();
        int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
        int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

        request.Pagina = skip / pageSize + 1;
        request.Registros = pageSize;
        request.Busqueda = Request.Form["search[value]"].FirstOrDefault();
        request.ColumnaOrdenamiento = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
        request.Ordenamiento = Request.Form["order[0][dir]"].FirstOrDefault();

        var gridData = new ResponseGrid<TarjetasResponseGrid>();
        List<TarjetasResponse> ListPF = new List<TarjetasResponse>();

        ListPF = await _tarjetasApiClient.GetTarjetasAsync(1,200);

        //Entorno local de pruebas
        //ListPF = GetList();

        var List = new List<TarjetasResponseGrid>();
        foreach (var row in ListPF)
        {
            List.Add(new TarjetasResponseGrid
            {
                idCuentaAhorro = row.idCuentaAhorro,
                idCliente = row.idCliente,
                nombreCompleto = row.nombreCompleto,
                proxyNumber = row.proxyNumber,
                clabe = row.clabe,
                tarjeta = row.tarjeta,
                //Estatus = row.Estatus,
                Acciones = await this.RenderViewToStringAsync("~/Views/Tarjetas/_Acciones.cshtml", row)
            });
        }
        if (!string.IsNullOrEmpty(request.Busqueda))
        {
            List = List.Where(x =>
            (x.idCuentaAhorro.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.idCliente.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.nombreCompleto?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.proxyNumber?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.clabe?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.tarjeta?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) 
            //(x.Estatus.ToString()?.ToLower() ?? "").Contains(request.Busqueda.ToLower())
            ).ToList();
        }

        //Aplicacion de Filtros temporal, 
        var filtronombreTitular = filters.FirstOrDefault(x => x.Key == "nombreTitular");
        var filtrotarjeta = filters.FirstOrDefault(x => x.Key == "tarjeta");
        var filtrocuentaClabe = filters.FirstOrDefault(x => x.Key == "cuentaClabe");
        var filtronumeroProxy = filters.FirstOrDefault(x => x.Key == "numeroProxy");

        if (filtronombreTitular.Value != null)
        {
            List = List.Where(x => x.nombreCompleto == Convert.ToString(filtronombreTitular.Value)).ToList();
        }

        if (filtrotarjeta.Value != null)
        {
            List = List.Where(x => x.tarjeta.Contains(Convert.ToString(filtrotarjeta.Value))).ToList();
        }

        if (filtrocuentaClabe.Value != null)
        {
            List = List.Where(x => x.clabe == Convert.ToString(filtrocuentaClabe.Value)).ToList();
        }

        if (filtronumeroProxy.Value != null)
        {
            List = List.Where(x => x.proxyNumber == Convert.ToString(filtronumeroProxy.Value)).ToList();
        }

        gridData.Data = List;
        gridData.RecordsTotal = List.Count;
        gridData.Data = gridData.Data.Skip(skip).Take(pageSize).ToList();
        filterRecord = string.IsNullOrEmpty(request.Busqueda) ? gridData.RecordsTotal ?? 0 : gridData.Data.Count;
        gridData.RecordsFiltered = filterRecord;
        gridData.Draw = draw;

        return Json(gridData);
    }

    [Authorize]
    [Route("Clientes/Detalle/Tarjetas/RegistrarTarjetas")]
    [HttpPost]
    public async Task<JsonResult> RegistrarTarjetas(RegistrarTarjetaRequest model)
    {
        string response = "";

        try
        {

            response = await _tarjetasApiClient.GetRegistroTarjetaAsync(model);

        }
        catch (Exception ex)
        {
            response = "";
        }

        return Json(model);
    }

    #endregion

    #region "Modelos"

    public class GenerarNombreResponse
    {
        public string Nombre { get; set; }
        public string NombreCompleto { get; set; }
    }

    public class RequestListFilters
    {
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }
    #endregion

    #region "Funciones Auxiliares"

    private GenerarNombreResponse generarnombre(int i)
    {
        var res = new GenerarNombreResponse();
        Random rnd = new Random((int)DateTime.Now.Ticks);

        i = i - 1;

        String[] nombres = { "Juan", "Pablo", "Paco", "Jose", "Alberto", "Roberto", "Genaro", "Andrea", "Maria", "Carmen" };
        String[] apellidos = { "Sanchez", "Perez", "Lopez", "Torres", "Alvarez", "Martinez", "Guzman", "Rodriguez", "Flores", "Vazquez" };
        String[] apellidosM = { "Vazquez", "Flores", "Rodriguez", "Guzman", "Martinez", "Alvarez", "Torres", "Lopez", "Perez", "Sanchez" };

        var genNombre = nombres[i];

        String gen = genNombre + " " + apellidos[i] + " " + apellidos[i];

        res.Nombre = genNombre;
        res.NombreCompleto = gen;

        return res;
    }

    public static string GenNumeroTelefono(int length)
    {
        if (length > 0)
        {
            var sb = new StringBuilder();

            var rnd = SeedRandom();
            for (int i = 0; i < length; i++)
            {
                sb.Append(rnd.Next(0, 9).ToString());
            }

            return sb.ToString();
        }

        return string.Empty;
    }

    public static string GenerarEmail()
    {
        //return string.Format("{0}@{1}.com", GenerateRandomAlphabetString(10), GenerateRandomAlphabetString(10));
        return string.Format("{0}@{1}.com", GenerateRandomAlphabetString(10), "gmail");
    }
    /// <summary>
    /// Gets a string from the English alphabet at random
    /// </summary>
    public static string GenerateRandomAlphabetString(int length)
    {
        string allowedChars = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var rnd = SeedRandom();

        char[] chars = new char[length];
        for (int i = 0; i < length; i++)
        {
            chars[i] = allowedChars[rnd.Next(allowedChars.Length)];
        }

        return new string(chars);
    }
    private static Random SeedRandom()
    {
        return new Random(Guid.NewGuid().GetHashCode());
    }

    public List<TarjetasResponse> GetList()
    {


        var List = new List<TarjetasResponse>();
        Random rnd = new Random();
        const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        String[] characters2 = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };

        try
        {
            for (int i = 1; i <= 10; i++)
            {
                var gen = generarnombre(i);

                var pf = new TarjetasResponse();
                pf.idCuentaAhorro = i;
                pf.idCliente = i+10;
                pf.nombreCompleto = gen.NombreCompleto;
                pf.proxyNumber = string.Format("02{0}",i);
                pf.clabe = string.Format("01232002769794212{0}",i);
                pf.tarjeta = string.Format("547016034567123{0}", i);
                //pf.Estatus = 1;

                List.Add(pf);
            }
        }
        catch (Exception e)
        {
            List = new List<TarjetasResponse>();
        }

        return List;
    }
    #endregion
}
