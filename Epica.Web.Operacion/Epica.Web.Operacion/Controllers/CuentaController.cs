using Epica.Web.Operacion.Config;
using Epica.Web.Operacion.Helpers;
using Epica.Web.Operacion.Models;
using Epica.Web.Operacion.Models.Common;
using Epica.Web.Operacion.Models.Request;
using Epica.Web.Operacion.Models.Response;
using Epica.Web.Operacion.Models.ViewModels;
using Epica.Web.Operacion.Services.Log;
using Epica.Web.Operacion.Services.Transaccion;
using Epica.Web.Operacion.Services.UserResolver;
using Epica.Web.Operacion.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using static Epica.Web.Operacion.Controllers.TransaccionesController;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Epica.Web.Operacion.Controllers;

[Authorize]
public class CuentaController : Controller
{
    #region "Locales"
    private readonly ICuentaApiClient _cuentaApiClient;
    private readonly IClientesApiClient _usuariosApiClient;
    private readonly ITransaccionesApiClient _transaccionesApiClient;
    private readonly UserContextService _userContextService;
    private readonly ILogsApiClient _logsApiClient;
    #endregion

    #region "Constructores"
    public CuentaController(ICuentaApiClient cuentaApiClient,
        IClientesApiClient usuariosApiClient,
        ITransaccionesApiClient transaccionesApiClient,
        ILogsApiClient logsApiClient,
        UserContextService userContextService)
    {

        _cuentaApiClient = cuentaApiClient;
        _usuariosApiClient = usuariosApiClient;
        _transaccionesApiClient = transaccionesApiClient;
        _userContextService = userContextService;
        _logsApiClient = logsApiClient;
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
            if (request.ColumnaOrdenamiento == "Clabe") {
                columna = 1;
            } else if (request.ColumnaOrdenamiento == "NoCuenta") {
                columna = 2;
            } else if (request.ColumnaOrdenamiento == "nombrePersona") {
                columna = 3;
            } else if (request.ColumnaOrdenamiento == "Saldo") {
                columna = 4;
            } else if (request.ColumnaOrdenamiento == "Tipo") {
                columna = 5;
            } else if (request.ColumnaOrdenamiento == "Estatus") {
                columna = 6;
            } else if (request.ColumnaOrdenamiento == "BloqueoSpeiOut") {
                columna = 7;
            }
        }

        if (request.Ordenamiento != null)
        {
            if (request.Ordenamiento == "asc") {
                tipoFiltro = true;
            } else {
                tipoFiltro = false;
            }
        }

        var gridData = new ResponseGrid<CuentasResponseGrid>();
        List<CuentasResponse> ListPF = new List<CuentasResponse>();

        if (!string.IsNullOrEmpty(request.Busqueda))
        {
            filters.RemoveAll(x => x.Key == "cuentaClabe");

            filters.Add(new RequestListFilters
            {
                Key = "cuentaClabe",
                Value = request.Busqueda
            });
        }

        //Validar si hay algun filtro con valor ingresado
        var validaFiltro = filters.Where(x => x.Value != null).ToList();

        if (validaFiltro.Count != 0) {
            (ListPF, paginacion) = await _cuentaApiClient.GetCuentasFilterAsync(Convert.ToInt32(request.Pagina), Convert.ToInt32(request.Registros), columna, tipoFiltro, filters);
        } else {
            (ListPF, paginacion) = await _cuentaApiClient.GetCuentasAsync(Convert.ToInt32(request.Pagina), Convert.ToInt32(request.Registros), columna, tipoFiltro);
        }

        //Entorno local de pruebas
        //ListPF = GetList();

        var List = new List<CuentasResponseGrid>();
        foreach (var row in ListPF)
        {
            List.Add(new CuentasResponseGrid
            {
                idCuenta = row.idCuenta,
                nombrePersona = !string.IsNullOrWhiteSpace(row.nombrePersona) ? row.nombrePersona: "-",
                noCuenta = row.noCuenta + "|" + row.idCuenta.ToString() + "|" + row.idCliente.ToString(),
                saldo = row.saldo,
                estatus = row.estatus,
                tipoPersona = !string.IsNullOrWhiteSpace(row.tipoPersona) ? row.tipoPersona : "-",
                alias = "",
                clabe = !string.IsNullOrWhiteSpace(row.clabe) ? row.clabe : "-",
                bloqueoSPEIOut = row.bloqueoSPEIOut,
                Acciones = await this.RenderViewToStringAsync("~/Views/Cuenta/_Acciones.cshtml", row)
            });
        }
        //if (!string.IsNullOrEmpty(request.Busqueda))
        //{
        //    List = List.Where(x =>
        //    (x.idCuenta.ToString().ToUpper() ?? "").Contains(request.Busqueda.ToUpper()) ||
        //    (x.nombrePersona?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
        //    (x.noCuenta?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
        //    (x.saldo.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
        //    (x.estatus.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
        //    (x.tipoPersona?.ToLower() ?? "").Contains(request.Busqueda.ToLower())
        //    ).ToList();
        //}

        gridData.Data = List;
        gridData.RecordsTotal = paginacion;
        filterRecord = string.IsNullOrEmpty(request.Busqueda) ? gridData.RecordsTotal ?? 0 : gridData.Data.Count;
        gridData.RecordsFiltered = filterRecord;
        gridData.Draw = draw;

        return Json(gridData);
    }

    [Authorize]
    [HttpPost]
    public async Task<JsonResult> GestionarEstatusCuentas(int id, string Estatus, int token, string cs)
    {
        MensajeResponse respuesta = new MensajeResponse();
        var loginResponse = _userContextService.GetLoginResponse();
        var accion = "";

        try
        {

            if (Estatus == "True")
            {
                respuesta = await _cuentaApiClient.BloqueoCuentaAsync(id, 0, token, cs);
                accion = "Desbloquear Cuenta";
            }
            else if (Estatus == "False")
            {
                respuesta = await _cuentaApiClient.BloqueoCuentaAsync(id, 1, token, cs);
                accion = "Bloquear Cuenta";
            }
            else
            {
                //Deteccion de posible cambio en codigo HTML a través de inspeccionar elemento
                return Json(BadRequest());
            }

            LogRequest logRequest = new LogRequest
            {
                IdUser = loginResponse.IdUsuario.ToString(),
                Modulo = "Cuentas",
                Fecha = HoraHelper.GetHoraCiudadMexico(),
                NombreEquipo = loginResponse.NombreDispositivo,
                Accion = accion,
                Ip = loginResponse.DireccionIp,
                Envio = JsonConvert.SerializeObject(id),
                Respuesta = respuesta.Error.ToString(),
                Error = respuesta.Error ? JsonConvert.SerializeObject(respuesta.Detalle) : string.Empty,
                IdRegistro = id
            };

            await _logsApiClient.InsertarLogAsync(logRequest);

        }
        catch (Exception ex)
        {

            return Json(BadRequest());
        }

        return Json(Ok());
    }

    [HttpPost]
    public async Task<JsonResult> GestionarEstatusSpeiOutCuentas(int id, string Estatus, int token, string cs)
    {
        MensajeResponse respuesta = new MensajeResponse();
        var loginResponse = _userContextService.GetLoginResponse();
        var accion = "";

        try
        {

            if (Estatus == "True")
            {
                respuesta = await _cuentaApiClient.BloqueoCuentaSpeiOutAsync(id, 0, token, cs);
                accion = "Desbloquear Spei Out";
            }
            else if (Estatus == "False")
            {
                respuesta = await _cuentaApiClient.BloqueoCuentaSpeiOutAsync(id, 1, token, cs);
                accion = "Bloquear Spei Out";
            }
            else
            {
                //Deteccion de posible cambio en codigo HTML a través de inspeccionar elemento
                return Json(BadRequest());
            }

            LogRequest logRequest = new LogRequest
            {
                IdUser = loginResponse.IdUsuario.ToString(),
                Modulo = "Cuentas",
                Fecha = HoraHelper.GetHoraCiudadMexico(),
                NombreEquipo = loginResponse.NombreDispositivo,
                Accion = accion,
                Ip = loginResponse.DireccionIp,
                Envio = JsonConvert.SerializeObject(id),
                Respuesta = respuesta.Error.ToString(),
                Error = respuesta.Error ? JsonConvert.SerializeObject(respuesta.Detalle) : string.Empty,
                IdRegistro = id
            };

            await _logsApiClient.InsertarLogAsync(logRequest);
        }
        catch (Exception ex)
        {

            return Json(BadRequest());
        }

        return Json(Ok());
    }

    [Authorize]
    [HttpPost]
    public async Task<JsonResult> ConsultarSubCuentas(int id)
    {
        var ListPF = await _cuentaApiClient.GetCobranzaReferenciadaAsync(id);
        //var ListPF = GetList();
        return Json(ListPF);
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
                cuetaOrigenOrdenante = !string.IsNullOrEmpty(row.cuetaOrigenOrdenante) ? row.cuetaOrigenOrdenante : "-",
                claveRastreo = !string.IsNullOrEmpty(row.claveRastreo) ? row.claveRastreo : "-",
                nombreOrdenante = !string.IsNullOrEmpty(row.nombreOrdenante) ? row.nombreOrdenante : "-",
                nombreBeneficiario = !string.IsNullOrEmpty(row.nombreBeneficiario) ? row.nombreBeneficiario : "-",
                monto = row.monto,
                estatus = row.estatus,
                concepto = row.concepto,
                idMedioPago = row.idMedioPago,
                idCuentaAhorro = row.idCuentaAhorro,
                fechaAlta = row.fechaAlta,
                fechaInstruccion = row.fechaInstruccion,
                fechaAutorizacion = row.fechaAutorizacion,
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


        gridData.RecordsTotal = List.Count;
        gridData.Data = List.Skip(skip).Take(pageSize).ToList();
        filterRecord = string.IsNullOrEmpty(request.Busqueda) ? gridData.RecordsTotal ?? 0 : gridData.Data.Count;
        gridData.RecordsFiltered = filterRecord;
        gridData.Draw = draw;

        return Json(gridData);
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
    #endregion

    #endregion

    #region "Modelos"

    #endregion

    #region "Funciones Auxiliares"

    #endregion
}
