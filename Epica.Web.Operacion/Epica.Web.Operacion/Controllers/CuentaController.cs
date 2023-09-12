using Epica.Web.Operacion.Config;
using Epica.Web.Operacion.Helpers;
using Epica.Web.Operacion.Models;
using Epica.Web.Operacion.Models.Common;
using Epica.Web.Operacion.Models.Request;
using Epica.Web.Operacion.Models.ViewModels;
using Epica.Web.Operacion.Services.Transaccion;
using Epica.Web.Operacion.Services.UserResolver;
using Epica.Web.Operacion.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using static Epica.Web.Operacion.Controllers.TransaccionesController;

namespace Epica.Web.Operacion.Controllers;

[Authorize]
public class CuentaController : Controller
{
    #region "Locales"
    private readonly ICuentaApiClient _cuentaApiClient;
    private readonly IClientesApiClient _usuariosApiClient;
    private readonly ITransaccionesApiClient _transaccionesApiClient;
    private readonly UserContextService _userContextService;
    #endregion

    #region "Constructores"
    public CuentaController(ICuentaApiClient cuentaApiClient,
        IClientesApiClient usuariosApiClient,
        ITransaccionesApiClient transaccionesApiClient,
        UserContextService userContextService)
    {

        _cuentaApiClient = cuentaApiClient;
        _usuariosApiClient = usuariosApiClient;
        _transaccionesApiClient = transaccionesApiClient;
        _userContextService = userContextService;
    }
    #endregion

    #region "Funciones"    

    #region Consulta
    [Authorize]
    public IActionResult Index()
    {
        var loginResponse = _userContextService.GetLoginResponse();
        var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Cuentas" && modulo.Ver == 0);
        if (validacion == true)
            return View();

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

        var gridData = new ResponseGrid<CuentasResponseGrid>();
        List<CuentasResponse> ListPF = new List<CuentasResponse>();

        var filtroNoCuenta = filters.FirstOrDefault(x => x.Key == "noCuenta");

        if (filtroNoCuenta.Value != null)
        {
            string noCuenta = filtroNoCuenta.Value;
            var listPF2 = await _cuentaApiClient.GetListaByNumeroCuentaAsync(noCuenta);

            foreach (var cuenta in listPF2)
            {
                CuentasResponse cuentasResponse = new CuentasResponse
                {
                    idCliente = cuenta.IdCliente,
                    saldo = decimal.Parse(cuenta.Saldo.ToString()),
                    alias = cuenta.Alias,
                    fechaActualizacion = cuenta.FechaActualizacion,
                    estatus = cuenta.Estatus,
                    nombrePersona = cuenta.NombrePersona,
                    fechaAlta = cuenta.FechaAlta,
                    tipoPersona = cuenta.TipoPersona,
                    idCuenta = cuenta.IdCuenta,
                    noCuenta = cuenta.NoCuenta,
                    fechaAltaFormat = cuenta.FechaAlta,
                    fechaActualizacionformat = cuenta.FechaActualizacion
                };

                ListPF.Add(cuentasResponse);
            }
        }
        else
        {
            ListPF = await _cuentaApiClient.GetCuentasAsync(1, 100);

        }

        //Entorno local de pruebas
        //ListPF = GetList();

        var List = new List<CuentasResponseGrid>();
        foreach (var row in ListPF)
        {
            List.Add(new CuentasResponseGrid
            {
                idCuenta = row.idCuenta,
                nombrePersona = row.nombrePersona,
                noCuenta = row.noCuenta + "|" + row.idCuenta.ToString() + "|" + row.idCliente.ToString(),
                saldo = row.saldo,
                estatus = row.estatus,
                tipoPersona = row.tipoPersona,
                Acciones = await this.RenderViewToStringAsync("~/Views/Cuenta/_Acciones.cshtml", row)
            });
        }
        if (!string.IsNullOrEmpty(request.Busqueda))
        {
            List = List.Where(x =>
            (x.idCuenta.ToString().ToUpper() ?? "").Contains(request.Busqueda.ToUpper()) ||
            (x.nombrePersona?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.noCuenta?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.saldo.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.estatus.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.tipoPersona?.ToLower() ?? "").Contains(request.Busqueda.ToLower())
            ).ToList();
        }
        //Aplicacion de Filtros temporal, 
        var filtronombreCliente = filters.FirstOrDefault(x => x.Key == "NombreCliente");
        var filtroEstatus = filters.FirstOrDefault(x => x.Key == "estatus");
        var filtroSaldo = filters.FirstOrDefault(x => x.Key == "saldo");
        var filtroTipo = filters.FirstOrDefault(x => x.Key == "tipoPersona");

        if (filtronombreCliente.Value != null)
        {
            List = List.Where(x => x.nombrePersona.Contains(Convert.ToString(filtronombreCliente.Value))).ToList();
        }

        if (filtroNoCuenta.Value != null)
        {
            string filtro = Convert.ToString(filtroNoCuenta.Value).ToUpper();
            List = List.Where(x => x.noCuenta != null &&
                                  x.noCuenta.Length >= 16 &&
                                  x.noCuenta.Substring(0, 16) == filtro).ToList();
        }

        if (filtroEstatus.Value != null)
        {
            List = List.Where(x => x.estatus == Convert.ToInt32(filtroEstatus.Value)).ToList();
        }

        if (filtroSaldo.Value != null)
        {
            List = List.Where(x => x.saldo == Convert.ToDecimal(filtroSaldo.Value)).ToList();
        }

        if (filtroTipo.Value != null)
        {
            List = List.Where(x => x.tipoPersona.Contains(Convert.ToString(filtroTipo.Value))).ToList();
        }

        gridData.Data = List;
        gridData.RecordsTotal = List.Count;
        gridData.Data = gridData.Data.Skip(skip).Take(pageSize).ToList();
        filterRecord = string.IsNullOrEmpty(request.Busqueda) ? gridData.RecordsTotal ?? 0 : gridData.Data.Count;
        gridData.RecordsFiltered = filterRecord;
        gridData.Draw = draw;

        return Json(gridData);
    }
    #endregion

    #region Detalle Cuenta
    [Authorize]
    [Route("Cuentas/Detalle/Movimientos")]
    public async Task<IActionResult> Cuentas(int id, int cliente, string noCuenta)
    {
        var loginResponse = _userContextService.GetLoginResponse();
        var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Cuentas" && modulo.Ver == 0);
        if (validacion == true)
        {
            ClienteDetailsResponse user = await _usuariosApiClient.GetDetallesCliente(cliente);

            if (user.value == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.UrlView = "Movimientos";
            ClientesHeaderViewModel header = new ClientesHeaderViewModel
            {
                Id = user.value.IdCliente,
                NombreCompleto = user.value.Nombre + " " + user.value.ApellidoPaterno + " " + user.value.ApellidoMaterno,
                Telefono = user.value.Telefono,
                Correo = user.value.Email,
                Curp = user.value.CURP,
                Organizacion = user.value.Organizacion,
                Rfc = user.value.RFC,
                Sexo = user.value.Sexo,
                NoCuenta = noCuenta
            };
            ViewBag.Info = header;
            ViewBag.Nombre = header.NombreCompleto;
            ViewBag.AccountID = id;
            ViewBag.NumCuenta = noCuenta;

            ModificarTransaccionRequest renderInfo = new ModificarTransaccionRequest
            {
                NombreOrdenante = header.NombreCompleto,
                NoCuentaOrdenante = noCuenta,
                ClaveRastreo = string.Format("AQPAY1000000{0}", DateTime.Now.ToString("yyyymmddhhmmss"))
            };
            ViewBag.DatosRef = renderInfo;
            return View("~/Views/Cuenta/DetallesCuenta/Transacciones/DetalleMovimientos.cshtml");
        }

        return NotFound();
    }

    //[Authorize]
    //[Route("Cuentas/Detalle/CobranzaReferenciada")]
    //public async Task<IActionResult> CobranzaReferenciada(int id, int cliente, string noCuenta)
    //{
    //    var loginResponse = _userContextService.GetLoginResponse();
    //    var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Cuentas" && modulo.Ver == 0);
    //    if (validacion == true)
    //    {
    //        ClienteDetailsResponse user = await _usuariosApiClient.GetDetallesCliente(cliente);

    //        if (user.value == null)
    //        {
    //            return RedirectToAction("Index");
    //        }

    //        ViewBag.UrlView = "Movimientos";
    //        ClientesHeaderViewModel header = new ClientesHeaderViewModel
    //        {
    //            Id = user.value.IdCliente,
    //            NombreCompleto = user.value.Nombre + " " + user.value.ApellidoPaterno + " " + user.value.ApellidoMaterno,
    //            Telefono = user.value.Telefono,
    //            Correo = user.value.Email,
    //            Curp = user.value.CURP,
    //            Organizacion = user.value.Organizacion,
    //            Rfc = user.value.RFC,
    //            Sexo = user.value.Sexo,
    //            NoCuenta = noCuenta
    //        };
    //        ViewBag.Info = header;
    //        ViewBag.Nombre = header.NombreCompleto;
    //        ViewBag.AccountID = id;
    //        ViewBag.NumCuenta = noCuenta;
    //        ViewBag.Cliente = cliente;

    //        return View("~/Views/Cuenta/DetallesCuenta/Transacciones/DetalleMovimientos.cshtml");
    //    }

    //    return NotFound();
    //}

    [HttpPost]
    public async Task<JsonResult> ConsultaCuentas(string id, string TipoConsulta)
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

        var gridData = new ResponseGrid<ResumenTransaccionResponseGrid>();
        List<TransaccionesResponse> ListPF = new List<TransaccionesResponse>();
        TransaccionesDetailsgeneralResponse resp = new TransaccionesDetailsgeneralResponse();

        if (TipoConsulta == "IsGeneral") {
            
            resp = await _transaccionesApiClient.GetTransaccionesClienteAsync(Convert.ToInt32(id));
            ListPF = resp.value;

        } else {

            resp = await _transaccionesApiClient.GetTransaccionesCuentaAsync(Convert.ToInt32(id));
            ListPF = resp.value;
        }

        var List = new List<ResumenTransaccionResponseGrid>();

        if (ListPF != null) {
        foreach (var row in ListPF)
        {
            List.Add(new ResumenTransaccionResponseGrid
            {
                id = row.idTransaccion,
                cuetaOrigenOrdenante = row.cuetaOrigenOrdenante,
                claveRastreo = row.claveRastreo,
                nombreOrdenante = row.nombreOrdenante,
                nombreBeneficiario = row.nombreBeneficiario,
                monto = row.monto,
                estatus = row.estatus,
                concepto = row.concepto,
                idMedioPago = row.idMedioPago,
                idCuentaAhorro = row.idCuentaAhorro,
                fechaAlta = row.fechaAlta,
                Acciones = await this.RenderViewToStringAsync("~/Views/Clientes/Detalles/Transacciones/_Acciones.cshtml", row)
            });
        }
        if (!string.IsNullOrEmpty(request.Busqueda))
        {
            List = List.Where(x =>
            (x.id.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.claveRastreo?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.nombreOrdenante?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.nombreBeneficiario?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.monto.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.estatus.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.concepto?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.idMedioPago.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.idCuentaAhorro.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.fechaAlta.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower())
            ).ToList();
        }
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
    [Route("Cuentas/Detalle/Transacciones/RegistrarTransaccion")]
    [HttpPost]
    public async Task<JsonResult> RegistrarTransaccion(RegistrarTransaccionRequest model)
    {
        MensajeResponse responses = new MensajeResponse();

        try
        {
            model.FechaOperacion = DateTime.Now.ToString("dd/mm/yyyy");
            //model.CuentaOrigenOrdenante = model.NoCuentaOrdenante;
            //model.CuentaDestinoBeneficiario = model.NoCuentaBeneficiario;

            responses = await _transaccionesApiClient.GetRegistroTransaccion(model);

        } catch (Exception ex) {
            responses.Error = true;
            responses.Detalle = ex.Message;
        }

        return Json(model);
    }
    #endregion


    [Authorize]
    [HttpPost]
    public async Task<JsonResult> ConsultarSubCuentas(int id)
    {
        var ListPF = await _cuentaApiClient.GetCobranzaReferenciadaAsync(id);
        //var ListPF = GetList();
        return Json(ListPF);
    }
    [Authorize]
    public async Task<IActionResult> DetalleCuenta(string numCuenta)
    {
        var result = new JsonResultDto();
        try
        {
            var detalleTransaccion = await _cuentaApiClient.GetDetalleCuentasAsync(numCuenta);

            if (detalleTransaccion != null)
            {
                result.Error = false;
                result.Result = await this.RenderViewToStringAsync("~/Views/Cuenta/_Detalle.cshtml", detalleTransaccion.First());
            }
            else
            {
                result.Error = true;
                result.ErrorDescription = "ERROR";
                return Json(result);
            }
        }
        catch (Exception)
        {
            result.Error = true;
            result.ErrorDescription = "Error1";
        }
        return Json(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<JsonResult> GestionarEstatusCuentas(int id, string Estatus)
    {
        try
        {

            if (Estatus == "True") {

            } else if (Estatus == "False") {

            } else {
                //Deteccion de posible cambio en codigo HTML a través de inspeccionar elemento
                return Json(BadRequest());
            }
        } catch (Exception ex) {

            return Json(BadRequest());
        }

        return Json(Ok());
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

    public List<CuentasResponse> GetList()
    {


        var List = new List<CuentasResponse>();
        Random rnd = new Random();
        const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        String[] characters2 = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };

        try
        {
            for (int i = 1; i <= 10; i++)
            {
                var gen = generarnombre(i);

                var pf = new CuentasResponse();
                pf.idCuenta = i;
                pf.nombrePersona = gen.NombreCompleto;
                pf.noCuenta = string.Format("465236478963245{0}", i);
                pf.saldo = Convert.ToDecimal(rnd.Next(0001, 99999).ToString("C2"));
                pf.estatus = 1;
                pf.tipoPersona = "Persona fisica";

                List.Add(pf);
            }
        }
        catch (Exception e)
        {
            List = new List<CuentasResponse>();
        }

        return List;
    }
    #endregion
}
