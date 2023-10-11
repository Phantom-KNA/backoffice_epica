using Epica.Web.Operacion.Helpers;
using Epica.Web.Operacion.Models.Common;
using Epica.Web.Operacion.Models.Request;
using Epica.Web.Operacion.Models.Response;
using Epica.Web.Operacion.Models.ViewModels;
using Epica.Web.Operacion.Services.Catalogos;
using Epica.Web.Operacion.Services.Log;
using Epica.Web.Operacion.Services.Transaccion;
using Epica.Web.Operacion.Utilities;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Text;

namespace Epica.Web.Operacion.Controllers;

[Authorize]
public class TarjetasController : Controller
{
    #region "Locales"
    private readonly IClientesApiClient _clientesApiClient;
    private readonly ITarjetasApiClient _tarjetasApiClient;
    private readonly ILogsApiClient _logsApiClient;
    private readonly ICatalogosApiClient _catalogosApiClient;
    private readonly UserContextService _userContextService;
    #endregion

    #region "Constructores"
    public TarjetasController(IClientesApiClient clientesApiClient,
        ITarjetasApiClient tarjetasApiClient,
        UserContextService userContextService,
        ICatalogosApiClient catalogosApiClient,
        ILogsApiClient logsApiClient)
    {
        _clientesApiClient = clientesApiClient;
        _userContextService = userContextService;
        _tarjetasApiClient = tarjetasApiClient;
        _logsApiClient = logsApiClient;
        _catalogosApiClient = catalogosApiClient;
    }
    #endregion

    #region "Funciones"

    #region Consulta Tarjetas
    [Authorize]
    public IActionResult Index()
    {
        var loginResponse = _userContextService.GetLoginResponse();
        var validacion = loginResponse?.AccionesPorModulo!.Any(modulo => modulo.ModuloAcceso == "Tarjetas" && modulo.Ver == 0);
        if (validacion == true)
            return View(loginResponse);

        return NotFound();
    }

    [Authorize]
    [HttpPost]
    public async Task<JsonResult> Consulta(List<RequestListFilters> filters)
    {
        var request = new RequestList();

        int totalRecord = 0;
        int filterRecord = 0;
        int paginacion = 0;
        int columna = 0;
        bool tipoFiltro = false;

        var draw = Request.Form["draw"].FirstOrDefault();
        int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
        int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

        request.Pagina = skip / pageSize + 1;
        request.Registros = pageSize;
        request.Busqueda = Request.Form["search[value]"].FirstOrDefault();
        request.ColumnaOrdenamiento = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
        request.Ordenamiento = Request.Form["order[0][dir]"].FirstOrDefault();

        if (request.ColumnaOrdenamiento != null)
        {
            if (request.ColumnaOrdenamiento == "Vinculo")
            {
                columna = 1;
            }
            else if (request.ColumnaOrdenamiento == "tarjeta")
            {
                columna = 2;
            }
            else if (request.ColumnaOrdenamiento == "clabe")
            {
                columna = 3;
            }
            else if (request.ColumnaOrdenamiento == "proxyNumber")
            {
                columna = 4;
            }
            else if (request.ColumnaOrdenamiento == "Estatus")
            {
                columna = 5;
            }
        }

        if (request.Ordenamiento != null)
        {
            if (request.Ordenamiento == "asc")
            {
                tipoFiltro = true;
            }
            else
            {
                tipoFiltro = false;
            }
        }

        var gridData = new ResponseGrid<TarjetasResponseGrid>();
        List<TarjetasResponse> ListPF = new List<TarjetasResponse>();

        if (!string.IsNullOrEmpty(request.Busqueda))
        {
            filters.RemoveAll(x => x.Key == "tarjeta");

            filters.Add(new RequestListFilters
            {
                Key = "tarjeta",
                Value = request.Busqueda
            });
        }

        //Validar si hay algun filtro con valor ingresado
        var validaFiltro = filters.Where(x => x.Value != null).ToList();

        if (validaFiltro.Count != 0)
        {
            (ListPF, paginacion) = await _tarjetasApiClient.GetTarjetasFilterAsync(Convert.ToInt32(request.Pagina), Convert.ToInt32(request.Registros), columna, tipoFiltro, filters);
        }
        else
        {
            (ListPF, paginacion) = await _tarjetasApiClient.GetTarjetasAsync(Convert.ToInt32(request.Pagina), Convert.ToInt32(request.Registros), columna, tipoFiltro);
        }

        //Entorno local de pruebas
        //ListPF = GetList();

        var List = new List<TarjetasResponseGrid>();
        foreach (var row in ListPF)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(row.tarjeta);
            var ally = System.Convert.ToBase64String(plainTextBytes);

            row.cadena = ally;
            row.nombreCompleto = row.nombreCompleto == null ? "N/A" : row.nombreCompleto;
            List.Add(new TarjetasResponseGrid
            {
                idCuentaAhorro = row.idCuentaAhorro,
                idCliente = row.idCliente,
                Vinculo = row.nombreCompleto + "|" + row.idCliente.ToString(),
                nombreCompleto = row.nombreCompleto,
                proxyNumber = row.proxyNumber,
                clabe = row.clabe,
                tarjeta = row.tarjeta,
                Estatus = row.Estatus,
                Acciones = await this.RenderViewToStringAsync("~/Views/Tarjetas/_Acciones.cshtml", row)
            });
        }
        //if (!string.IsNullOrEmpty(request.Busqueda))
        //{
        //    List = List.Where(x =>
        //    (x.idCuentaAhorro.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
        //    (x.idCliente.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
        //    (x.nombreCompleto?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
        //    (x.proxyNumber?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
        //    (x.clabe?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
        //    (x.tarjeta?.ToLower() ?? "").Contains(request.Busqueda.ToLower())
        //    //(x.Estatus.ToString()?.ToLower() ?? "").Contains(request.Busqueda.ToLower())
        //    ).ToList();
        //}

        gridData.Data = List;
        gridData.RecordsTotal = paginacion;
        filterRecord = string.IsNullOrEmpty(request.Busqueda) ? gridData.RecordsTotal ?? 0 : gridData.Data.Count;
        gridData.RecordsFiltered = filterRecord;
        gridData.Draw = draw;

        return Json(gridData);
    }

    [HttpPost]
    public async Task<JsonResult> GestionarEstatusTarjetas(string NumGen, string estatus, int id)
    {
        var base64EncodedBytes = System.Convert.FromBase64String(NumGen);
        var tarjeta = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

        var loginResponse = _userContextService.GetLoginResponse();
        MensajeResponse response = new MensajeResponse();
        int EstatusTarjeta = 0;

        try
        {
            string accion = "";

            if (estatus == "False")
            {
                EstatusTarjeta = 2;
                accion = "Bloquear Tarjeta";
            }
            else if (estatus == "True")
            {
                EstatusTarjeta = 1;
                accion = "Desbloquear Tarjeta";
            }

            response = await _tarjetasApiClient.GetBloqueoTarjeta(tarjeta, EstatusTarjeta, loginResponse.Token!);

            LogRequest logRequest = new LogRequest
            {
                IdUser = loginResponse.IdUsuario.ToString(),
                Modulo = "Tarjetas",
                Fecha = HoraHelper.GetHoraCiudadMexico(),
                NombreEquipo = loginResponse.NombreDispositivo,
                Accion = accion,
                Ip = loginResponse.DireccionIp,
                Envio = JsonConvert.SerializeObject(NumGen),
                Respuesta = response.Error.ToString(),
                Error = response.Error ? JsonConvert.SerializeObject(response.Detalle) : string.Empty,
                IdRegistro = id
            };
            await _logsApiClient.InsertarLogAsync(logRequest);

        }
        catch (Exception ex)
        {
            response.Detalle = ex.Message;
        }

        return Json(response);
    }
    #endregion

    #region Registrar Tarjeta
    [Authorize]
    public async Task<ActionResult> RegistroTarjetaCliente()
    {
        var loginResponse = _userContextService.GetLoginResponse();
        var validacion = loginResponse?.AccionesPorModulo!.Any(modulo => modulo.ModuloAcceso == "Tarjetas" && modulo.Insertar == 0);
        if (validacion == true)
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
    public async Task<IActionResult> RegistrarTarjetaCliente(TarjetasRegistroTarjetaClienteViewModel model)
    {
        var loginResponse = _userContextService.GetLoginResponse();
        var validacion = loginResponse?.AccionesPorModulo!.Any(modulo => modulo.ModuloAcceso == "Tarjetas" && modulo.Insertar == 0);
        if (validacion == true)
        {
            try
            {
                var response = await _tarjetasApiClient.GetRegistroTarjetaAsync(model.TarjetasDetalles);

                string detalle = response.Detalle ?? "";
                int idRegistro = 0;

                try
                {
                    idRegistro = int.Parse(detalle);
                }
                catch (FormatException)
                {
                    idRegistro = 0;
                }

                LogRequest logRequest = new LogRequest
                {
                    IdUser = loginResponse.IdUsuario.ToString(),
                    Modulo = "Tarjetas",
                    Fecha = HoraHelper.GetHoraCiudadMexico(),
                    NombreEquipo = loginResponse.NombreDispositivo,
                    Accion = "Insertar",
                    Ip = loginResponse.DireccionIp,
                    Envio = JsonConvert.SerializeObject(model.TarjetasDetalles),
                    Respuesta = response.Error.ToString(),
                    Error = response.Error ? JsonConvert.SerializeObject(response.Detalle) : string.Empty,
                    IdRegistro = idRegistro
                };
                await _logsApiClient.InsertarLogAsync(logRequest);

                if (response.Codigo == "200")
                {
                    return Ok(new { success = true });
                }
                else
                {
                    return Ok(new { success = false, response.Detalle });
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        return NotFound();
    }

    [Authorize]
    [Route("Clientes/Detalle/Tarjetas/RegistrarTarjetas")]
    [HttpPost]
    public async Task<JsonResult> RegistrarTarjetas(RegistrarTarjetaRequest model)
    {
        var loginResponse = _userContextService.GetLoginResponse();
        MensajeResponse response = new MensajeResponse();

        try
        {
            response = await _tarjetasApiClient.GetRegistroTarjetaAsync(model);

            string detalle = response.Detalle ?? "";
            int idRegistro = 0;

            try
            {
                idRegistro = int.Parse(detalle);
            }
            catch (FormatException)
            {
                idRegistro = 0;
            }

            LogRequest logRequest = new LogRequest
            {
                IdUser = loginResponse.IdUsuario.ToString(),
                Modulo = "Tarjetas",
                Fecha = HoraHelper.GetHoraCiudadMexico(),
                NombreEquipo = loginResponse.NombreDispositivo,
                Accion = "Insertar",
                Ip = loginResponse.DireccionIp,
                Envio = JsonConvert.SerializeObject(model),
                Respuesta = response.Error.ToString(),
                Error = response.Error ? JsonConvert.SerializeObject(response.Detalle) : string.Empty,
                IdRegistro = idRegistro
            };
            await _logsApiClient.InsertarLogAsync(logRequest);
            //await _logsApiClient.InsertarLogAsync(logRequest);
        }
        catch (Exception ex)
        {
            response.Detalle = ex.Message;
        }

        return Json(response);
    }
    #endregion

    #endregion

    #region "Modelos"

    #endregion

    #region "Funciones Auxiliares"

    #endregion
}
