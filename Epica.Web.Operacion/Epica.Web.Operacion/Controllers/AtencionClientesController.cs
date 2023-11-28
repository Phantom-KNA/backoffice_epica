using Epica.Web.Operacion.Config;
using Epica.Web.Operacion.Models.Common;
using Epica.Web.Operacion.Models.ViewModels;
using Epica.Web.Operacion.Models.Entities;
using Epica.Web.Operacion.Services.Transaccion;
using Epica.Web.Operacion.Services.UserResolver;
using Epica.Web.Operacion.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using static Epica.Web.Operacion.Controllers.CuentaController;
using Epica.Web.Operacion.Models.Request;
using Epica.Web.Operacion.Helpers;
using Epica.Web.Operacion.Services.Catalogos;
using Epica.Web.Operacion.Services.Log;
using System.Globalization;
using Epica.Web.Operacion.Models.Response;
using System.IO;
using Microsoft.IdentityModel.Tokens;
using static Epica.Web.Operacion.Controllers.TransaccionesController;

namespace Epica.Web.Operacion.Controllers;

public class AtencionClientesController : Controller
{
    #region "Locales"
    private readonly IClientesApiClient _clientesApiClient;
    private readonly ICuentaApiClient _cuentaApiClient;
    private readonly ITarjetasApiClient _tarjetasApiClient;
    private readonly UserContextService _userContextService;
    private readonly ICatalogosApiClient _catalogosApiClient;
    private readonly ILogsApiClient _logsApiClient;
    private readonly IPersonaMoralServices _personaMoralServices;
    private readonly ITransaccionesApiClient _transaccionesApiClient;
    #endregion

    #region "Constructores"
    public AtencionClientesController(IClientesApiClient clientesApiClient,
        ICuentaApiClient cuentaApiClient,
        ICatalogosApiClient catalogosApiClient,
        ITarjetasApiClient tarjetasApiClient,
        UserContextService userContextService,
        ILogsApiClient logsApiClient,
        IPersonaMoralServices personaMoralServices,
        ITransaccionesApiClient transaccionesApiClient
        )
    {
        _logsApiClient = logsApiClient;
        _clientesApiClient = clientesApiClient;
        _cuentaApiClient = cuentaApiClient;
        _userContextService = userContextService;
        _catalogosApiClient = catalogosApiClient;
        _tarjetasApiClient = tarjetasApiClient;
        _personaMoralServices = personaMoralServices;
        _transaccionesApiClient = transaccionesApiClient;
    }
    #endregion

    #region "Funciones"

    #region Consulta Personas Fisicas

    [Authorize]
    public IActionResult Index()
    {
        var loginResponse = _userContextService.GetLoginResponse();
        var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Clientes" && modulo.Ver == 0);
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
            if (request.ColumnaOrdenamiento == "NombreCompleto") {
                columna = 1;
            } else if (request.ColumnaOrdenamiento == "Telefono") {
                columna = 2;
            } else if (request.ColumnaOrdenamiento == "Email") {
                columna = 3;
            } else if (request.ColumnaOrdenamiento == "Curp") {
                columna = 4;
            } else if (request.ColumnaOrdenamiento == "Organizacion") {
                columna = 5;
            } else if (request.ColumnaOrdenamiento == "estatusweb") {
                columna = 6;
            } else if (request.ColumnaOrdenamiento == "Estatus") {
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

        var gridData = new ResponseGrid<ClienteResponseGrid>();
        List<ClienteResponse> ListPF = new List<ClienteResponse>();

        if (!string.IsNullOrEmpty(request.Busqueda))
        {
            filters.RemoveAll(x => x.Key == "nombreCliente");

            filters.Add(new RequestListFilters
            {
                Key = "nombreCliente",
                Value = request.Busqueda
            });
        }

        //Validar si hay algun filtro con valor ingresado
        var validaFiltro = filters.Where(x => x.Value != null).ToList();

        if (validaFiltro.Count != 0) {
            (ListPF, paginacion) = await _clientesApiClient.GetClientesFilterAsync(Convert.ToInt32(request.Pagina), Convert.ToInt32(request.Registros), columna, tipoFiltro, filters);
        } else {
            (ListPF, paginacion) = await _clientesApiClient.GetClientesAsync(Convert.ToInt32(request.Pagina), Convert.ToInt32(request.Registros),columna, tipoFiltro);
        }

        //var discs = ListPF.DistinctBy(x => x.id).ToList();

        var List = new List<ClienteResponseGrid>();
        foreach (var row in ListPF)
        {

            //var Organizaciones = "";

            //if (!string.IsNullOrWhiteSpace(row.organizacion)) {

            //    var OrganizacionesCliente = (from n in ListPF where n.id == row.id select n.organizacion).ToList();

            //    foreach (var org in OrganizacionesCliente) {

            //        if (!string.IsNullOrWhiteSpace(org)){
            //            if (org == OrganizacionesCliente.Last()) {
            //                Organizaciones += org;
            //            } else {
            //                Organizaciones += org + " / ";
            //            }

            //        }
            //    }

            //} else {
            //    Organizaciones = "-";
            //}
            
            List.Add(new ClienteResponseGrid
            {
                id = row.id,
                nombreCompleto = row.nombreCompleto == null ? "-" : row.nombreCompleto,
                vinculo = !string.IsNullOrWhiteSpace(row.nombreCompleto) ? (row.nombreCompleto + "|" + row.id.ToString()).ToUpper() : "-",
                telefono = !string.IsNullOrWhiteSpace(row.telefono) ? row.telefono: "-",
                email = !string.IsNullOrWhiteSpace(row.email) ? row.email: "-",
                CURP = !string.IsNullOrWhiteSpace(row.CURP) ? row.CURP : "-",
                organizacion = !string.IsNullOrWhiteSpace(row.organizacion) ? row.organizacion : "-",
                membresia = !string.IsNullOrWhiteSpace(row.membresia) ? row.membresia: "-",
                sexo = !string.IsNullOrWhiteSpace(row.sexo) ? row.sexo: "-",
                estatus = row.estatus,
                estatusweb = !string.IsNullOrWhiteSpace(row.estatusweb) ? row.estatusweb: "-",
                Acciones = await this.RenderViewToStringAsync("~/Views/Clientes/_Acciones.cshtml", row)
            });
        }

        //Filtro TextBox Busqueda Rapida
        if (!string.IsNullOrEmpty(request.Busqueda))
        {
            List = List.Where(x =>
            (x.nombreCompleto?.ToUpper() ?? "").Contains(request.Busqueda.ToUpper()) ||
            (x.telefono?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.email?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.CURP?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.organizacion?.ToLower() ?? "").Contains(request.Busqueda.ToLower())
            ).ToList();
        }

        gridData.Data = List;
        gridData.RecordsTotal = paginacion;
        filterRecord = string.IsNullOrEmpty(request.Busqueda) ? gridData.RecordsTotal ?? 0 : gridData.RecordsTotal ?? 0;
        gridData.RecordsFiltered = filterRecord;
        gridData.Draw = draw;

        request.Busqueda = "";
        return Json(gridData);
    }

    [HttpPost]
    public async Task<JsonResult> GestionarEstatusClienteWeb(int id, string Estatus)
    {
        var loginResponse = _userContextService.GetLoginResponse();
        BloqueoWebResponse bloqueoWebResponse = new BloqueoWebResponse();
        try
        {
            if (Estatus == "True")
            {

                BloqueoWebClienteRequest bloqueoWebRequest = new BloqueoWebClienteRequest();
                bloqueoWebRequest.idCliente = id;
                bloqueoWebRequest.estatus = 1;

                bloqueoWebResponse = await _clientesApiClient.GetBloqueoWeb(bloqueoWebRequest);

                LogRequest logRequest = new LogRequest
                {
                    IdUser = loginResponse.IdUsuario.ToString(),
                    Modulo = "Clientes",
                    Fecha = HoraHelper.GetHoraCiudadMexico(),
                    NombreEquipo = loginResponse.NombreDispositivo,
                    Accion = "Desbloqueo web",
                    Ip = loginResponse.DireccionIp,
                    Envio = JsonConvert.SerializeObject(bloqueoWebRequest),
                    Respuesta = bloqueoWebResponse.error.ToString(),
                    Error = bloqueoWebResponse.error ? JsonConvert.SerializeObject(bloqueoWebResponse.detalle) : string.Empty,
                    IdRegistro = id
                };
                await _logsApiClient.InsertarLogAsync(logRequest);

            }
            else if (Estatus == "False")
            {

                BloqueoWebClienteRequest bloqueoWebRequest = new BloqueoWebClienteRequest();
                bloqueoWebRequest.idCliente = id;
                bloqueoWebRequest.estatus = 2;

                bloqueoWebResponse = await _clientesApiClient.GetBloqueoWeb(bloqueoWebRequest);

                LogRequest logRequest = new LogRequest
                {
                    IdUser = loginResponse.IdUsuario.ToString(),
                    Modulo = "Clientes",
                    Fecha = HoraHelper.GetHoraCiudadMexico(),
                    NombreEquipo = loginResponse.NombreDispositivo,
                    Accion = "Bloqueo web",
                    Ip = loginResponse.DireccionIp,
                    Envio = JsonConvert.SerializeObject(bloqueoWebRequest),
                    Respuesta = bloqueoWebResponse.error.ToString(),
                    Error = bloqueoWebResponse.error ? JsonConvert.SerializeObject(bloqueoWebResponse.detalle) : string.Empty,
                    IdRegistro = id
                };
                await _logsApiClient.InsertarLogAsync(logRequest);

            }
            else
            {
                //Deteccion de posible cambio en codigo HTML a través de inspeccionar elemento
                return Json(BadRequest());
            }

            if (bloqueoWebResponse.error == false)
            {
                return Json(Ok());
            }
            else
            {
                return Json(BadRequest());
            }


        }
        catch (Exception ex)
        {
            return Json(BadRequest());
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<JsonResult> GestionarEstatusClienteTotal(int id, string Estatus)
    {
        var loginResponse = _userContextService.GetLoginResponse();

        BloqueoTotalResponse bloqueototalResponse = new BloqueoTotalResponse();
        try
        {
            if (Estatus == "True")
            {

                BloqueoTotalClienteRequest bloqueoTotalRequest = new BloqueoTotalClienteRequest();
                bloqueoTotalRequest.idCliente = id;
                bloqueoTotalRequest.estatus = 1;

                bloqueototalResponse = await _clientesApiClient.GetBloqueoTotal(bloqueoTotalRequest);

                LogRequest logRequest = new LogRequest
                {
                    IdUser = loginResponse.IdUsuario.ToString(),
                    Modulo = "Clientes",
                    Fecha = HoraHelper.GetHoraCiudadMexico(),
                    NombreEquipo = loginResponse.NombreDispositivo,
                    Accion = "Desbloqueo total",
                    Ip = loginResponse.DireccionIp,
                    Envio = JsonConvert.SerializeObject(bloqueoTotalRequest),
                    Respuesta = bloqueototalResponse.error.ToString(),
                    Error = bloqueototalResponse.error ? JsonConvert.SerializeObject(bloqueototalResponse.detalle) : string.Empty,
                    IdRegistro = id
                };
                await _logsApiClient.InsertarLogAsync(logRequest);

            }
            else if (Estatus == "False")
            {

                BloqueoTotalClienteRequest bloqueoTotalRequest = new BloqueoTotalClienteRequest();
                bloqueoTotalRequest.idCliente = id;
                bloqueoTotalRequest.estatus = 2;

                bloqueototalResponse = await _clientesApiClient.GetBloqueoTotal(bloqueoTotalRequest);

                LogRequest logRequest = new LogRequest
                {
                    IdUser = loginResponse.IdUsuario.ToString(),
                    Modulo = "Clientes",
                    Fecha = HoraHelper.GetHoraCiudadMexico(),
                    NombreEquipo = loginResponse.NombreDispositivo,
                    Accion = "Bloqueo total",
                    Ip = loginResponse.DireccionIp,
                    Envio = JsonConvert.SerializeObject(bloqueoTotalRequest),
                    Respuesta = bloqueototalResponse.error.ToString(),
                    Error = bloqueototalResponse.error ? JsonConvert.SerializeObject(bloqueototalResponse.detalle) : string.Empty,
                    IdRegistro = id
                };
                await _logsApiClient.InsertarLogAsync(logRequest);

            }
            else
            {
                //Deteccion de posible cambio en codigo HTML a través de inspeccionar elemento
                return Json(BadRequest());
            }

            if (bloqueototalResponse.error == false)
            {
                return Json(Ok());
            }
            else
            {
                return Json(BadRequest());
            }

        }
        catch (Exception ex)
        {
            return Json(BadRequest());
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<JsonResult> EnvioContrasenaCorreo(string correo, int id)
    {

        MensajeResponse response = new MensajeResponse();

        try
        {
            var loginResponse = _userContextService.GetLoginResponse();
            response = await _clientesApiClient.GetRestablecerContraseñaCorreo(correo);

            LogRequest logRequest = new LogRequest
            {
                IdUser = loginResponse.IdUsuario.ToString(),
                Modulo = "Clientes",
                Fecha = HoraHelper.GetHoraCiudadMexico(),
                NombreEquipo = loginResponse.NombreDispositivo,
                Accion = "Restablecer contraseña Email",
                Ip = loginResponse.DireccionIp,
                Envio = JsonConvert.SerializeObject(correo),
                Respuesta = response.Error.ToString(),
                Error = response.Error ? JsonConvert.SerializeObject(response.message) : string.Empty,
                IdRegistro = id
            };
            await _logsApiClient.InsertarLogAsync(logRequest);

            if (response.Error == false)
            {
                return Json(Ok(response));
            }
            else
            {
                return Json(BadRequest(response));
            }

        }
        catch (Exception ex)
        {
            return Json(BadRequest());
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<JsonResult> EnvioContrasenaTelefono(string telefono, int id)
    {

        MensajeResponse response = new MensajeResponse();

        try
        {
            var loginResponse = _userContextService.GetLoginResponse();
            response = await _clientesApiClient.GetRestablecerContraseñaTelefono(telefono);

            LogRequest logRequest = new LogRequest
            {
                IdUser = loginResponse.IdUsuario.ToString(),
                Modulo = "Clientes",
                Fecha = HoraHelper.GetHoraCiudadMexico(),
                NombreEquipo = loginResponse.NombreDispositivo,
                Accion = "Restablecer contraseña SMS",
                Ip = loginResponse.DireccionIp,
                Envio = JsonConvert.SerializeObject(telefono),
                Respuesta = response.Error.ToString(),
                Error = response.Error ? JsonConvert.SerializeObject(response.message) : string.Empty,
                IdRegistro = id
            };
            await _logsApiClient.InsertarLogAsync(logRequest);

            if (response.Error == false)
            {
                return Json(Ok(response));
            }
            else
            {
                return Json(BadRequest(response));
            }

        }
        catch (Exception ex)
        {
            return Json(BadRequest());
        }
    }
    #endregion

    #region Detalles Personas fisicas

    #region Tap Datos Generales
    [Authorize]
    [Route("Clientes/Detalle/DatosGenerales")]
    public async Task<IActionResult> DatosGenerales(int id)
    {
        try
        {
            var loginResponse = _userContextService.GetLoginResponse();
            var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Clientes" && modulo.Ver == 0);
            if (validacion == true)
            {
                ClienteDetailsResponse user = await _clientesApiClient.GetDetallesCliente(id);
                List<EmpresaResponse> empresasResponse = await _clientesApiClient.GetEmpresasClienteAsync(id);

                if (user.value == null)
                {
                    return RedirectToAction("Index");
                }
                string empresasConcatenadas = string.Join("/", empresasResponse.Select(e => e.Empresa));

                ClientesDetallesViewModel clientesDetallesViewModel = new ClientesDetallesViewModel
                {
                    IdCliente = user.value.IdCliente,
                    Nombre = user.value.Nombre + " " + user.value.ApellidoPaterno + " " + user.value.ApellidoMaterno,
                    Telefono = user.value.Telefono.IsNullOrEmpty() ? "-" : user.value.Telefono,
                    Email = user.value.Email.IsNullOrEmpty() ? "-" : user.value.Email,
                    CURP = user.value.CURP.IsNullOrEmpty() ? "-" : user.value.CURP,
                    Organizacion = empresasConcatenadas ?? "",
                    Sexo = user.value.Sexo.IsNullOrEmpty() ? "-" : user.value.Sexo,
                    RFC = user.value.RFC.IsNullOrEmpty() ? "-" : user.value.RFC,
                    INE = user.value.INE.IsNullOrEmpty() ? "-" : user.value.INE,
                    FechaNacimiento = user.value.FechaNacimiento.IsNullOrEmpty() ? "-" : user.value.FechaNacimiento,
                    Observaciones = user.value.Observaciones.IsNullOrEmpty() ? "-" : user.value.Observaciones,
                    PaisNacimiento = user.value.PaisNacimiento.IsNullOrEmpty() ? "-" : user.value.PaisNacimiento,
                    Nacionalidad = user.value.Nacionalidad.IsNullOrEmpty() ? "-" : user.value.Nacionalidad,
                    Calle = user.value.Calle.IsNullOrEmpty() ? "-" : user.value.Calle,
                    NoInt = user.value.NoInt.IsNullOrEmpty() ? "-" : user.value.NoInt,
                    Colonia = user.value.Colonia.IsNullOrEmpty() ? "-" : user.value.Colonia,
                    CodigoPostal = user.value.CodigoPostal.IsNullOrEmpty() ? "-" : user.value.CodigoPostal,
                    Estado = user.value.Estado.IsNullOrEmpty() ? "-" : user.value.Estado,
                    Municipio = user.value.Municipio.IsNullOrEmpty() ? "-" : user.value.Municipio,
                    EntidadNacimiento = user.value.EntidadNacimiento.IsNullOrEmpty() ? "-" : user.value.EntidadNacimiento,
                    CalleNumero = user.value.CalleNumero.IsNullOrEmpty() ? "-" : user.value.CalleNumero,
                };

                ClientesHeaderViewModel header = new ClientesHeaderViewModel
                {
                    Id = user.value.IdCliente,
                    NombreCompleto = user.value.Nombre + " " + user.value.ApellidoPaterno + " " + user.value.ApellidoMaterno,
                    Telefono = user.value.Telefono ?? "",
                    Correo = user.value.Email ?? "",
                    Curp = user.value.CURP ?? "",
                    Organizacion = empresasConcatenadas ?? "",
                    Rfc = user.value.RFC ?? "",
                    Sexo = user.value.Sexo ?? ""
                };
                ViewBag.Info = header;
                ViewBag.UrlView = "DatosGenerales";
                ViewBag.NombrePascal = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(header.NombreCompleto.ToString().ToLower());

                var validacionEdicion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Clientes" && modulo.Editar == 0);
                if (validacionEdicion == true)
                {
                    ViewBag.ValEdicion = true;
                }
                else
                {
                    ViewBag.ValEdicion = false;
                }


                return View("~/Views/Clientes/Detalles/DatosGenerales/DetalleCliente.cshtml", clientesDetallesViewModel);
            }

            return RedirectToAction("Index");

        }
        catch (Exception ex)
        {
            return RedirectToAction("Index");
        }

    }
    #endregion

    #region Tap Cuentas
    [Authorize]
    [Route("Clientes/Detalle/Cuentas")]
    public async Task<IActionResult> Cuentas(int id)
    {
        var loginResponse = _userContextService.GetLoginResponse();
        var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Clientes" && modulo.Ver == 0);
        if (validacion == true)
        {
            ClienteDetailsResponse user = await _clientesApiClient.GetDetallesGeneralesClienteAsync(id);
            List<EmpresaResponse> empresasResponse = await _clientesApiClient.GetEmpresasClienteAsync(id);
            var listaRoles = await _catalogosApiClient.GetRolClienteAsync();
            var listaEmpresa = await _catalogosApiClient.GetEmpresasAsync();

            if (user.value == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.UrlView = "Cuentas";
            string empresasConcatenadas = string.Join("/", empresasResponse.Select(e => e.Empresa));

            ClientesHeaderViewModel header = new ClientesHeaderViewModel
            {
                Id = user.value.IdCliente,
                NombreCompleto = user.value.Nombre + " " + user.value.ApellidoPaterno + " " + user.value.ApellidoMaterno,
                Telefono = user.value.Telefono ?? "",
                Correo = user.value.Email ?? "",
                Curp = user.value.CURP ?? "",
                Organizacion = empresasConcatenadas ?? "",
                Rfc = user.value.RFC ?? "",
                Sexo = user.value.Sexo ?? ""
            };

            AsignarCuentaDetailsResponse asign = new AsignarCuentaDetailsResponse
            {
                IdCliente = user.value.IdCliente,
                ListaRoles = listaRoles,
                ListaEmpresa = listaEmpresa
            };

            ViewBag.Info = header;
            ViewBag.Nombre = header.NombreCompleto;
            ViewBag.AccountID = id;
            ViewBag.AsignData = asign;
            ViewBag.NombrePascal = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(header.NombreCompleto.ToLower());

            var validacionEdicion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Cuentas" && modulo.Editar == 0);
            if (validacionEdicion == true)
            {
                ViewBag.ValEdicion = true;
            }
            else
            {
                ViewBag.ValEdicion = false;
            }

            return View("~/Views/Clientes/Detalles/Cuentas/DetalleCuentas.cshtml");
        }

        return NotFound();
    }

    [Authorize]
    [HttpPost]
    public async Task<JsonResult> ConsultaCuentas(string id)
    {
        var loginResponse = _userContextService.GetLoginResponse();
        JsonResult result = new JsonResult("");
        //Invoca al método que se encarga de realizar la petición Api
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

        ListPF = await _cuentaApiClient.GetCuentasByClienteAsync(Convert.ToInt32(id));

        ////Almacena el subtotal de registro en caso de obtener un parámetro de búsqueda
        var List = new List<CuentasResponseGrid>();
        foreach (var row in ListPF)
        {
            var validacionEdicion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Clientes" && modulo.Editar == 0);
            if (validacionEdicion == true)
            {
                row.validarPermiso = true;
            }
            else
            {
                row.validarPermiso = false;
            }

            List.Add(new CuentasResponseGrid
            {
                idCuenta = row.idCuenta,
                nombrePersona = row.nombrePersona,
                concatLabel = row.noCuenta + "|" + row.idCuenta.ToString() + "|" + id,
                noCuenta = row.noCuenta,
                saldo = row.saldo,
                estatus = row.estatus,
                tipoPersona = row.tipoPersona ?? "-",
                alias = !string.IsNullOrEmpty(row.alias) ? row.alias : "-",
                fechaActualizacion = row.fechaActualizacion ?? "-",
                fechaAlta = row.fechaAlta ?? "-",
                email = row.email ?? "-",
                telefono = row.telefono ?? "-",
                fechaActualizacionformat = row.fechaActualizacion?.ToString(),
                fechaAltaFormat = row.fechaAlta?.ToString(),
                Acciones = await this.RenderViewToStringAsync("~/Views/Clientes/Detalles/Cuentas/_Acciones.cshtml", row)
            });
        }

        if (!string.IsNullOrEmpty(request.Busqueda))
        {
            if (request.Busqueda.ToLower() == "activo")
            {
                request.Busqueda = "2";
            }
            else if (request.Busqueda.ToLower() == "desactivado")
            {
                request.Busqueda = "1";
            }

            List = List.Where(x =>
            (x.nombrePersona?.ToUpper() ?? "").Contains(request.Busqueda.ToUpper()) ||
            (x.noCuenta?.ToLower() ?? "").Contains(request.Busqueda.ToUpper()) ||
            (x.alias?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.saldo.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.fechaActualizacion.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.fechaAlta.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower())
            ).ToList();
        }

        gridData.RecordsTotal = List.Count;
        gridData.Data = List.Skip(skip).Take(pageSize).ToList();
        filterRecord = string.IsNullOrEmpty(request.Busqueda) ? gridData.RecordsTotal ?? 0 : gridData.Data.Count;
        gridData.RecordsFiltered = filterRecord;
        gridData.Draw = draw;

        return Json(gridData);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult> BuscarCuenta(string NoCuenta)
    {
        var listaClientes = await _cuentaApiClient.GetDetalleCuentasSinAsignarAsync(NoCuenta);

        return Json(listaClientes.First());
    }

    [Authorize]
    [Route("Clientes/Detalle/Cuentas/RegistrarAsignacionCuenta")]
    [HttpPost]
    public async Task<JsonResult> RegistrarAsignacionCuenta(AsignarCuentaDetailsResponse model)
    {
        var loginResponse = _userContextService.GetLoginResponse();
        MensajeResponse response = new MensajeResponse();

        try
        {
            AsignarCuentaResponse asignarCuentaResponse = new AsignarCuentaResponse
            {
                idCliente = model.IdCliente,
                idCuenta = model.IdCuenta,
                descripcionRol = model.Rol,
                idEmpresa = model.IdEmpresa
            };

            response = await _clientesApiClient.GetRegistroAsignacionCuentaCliente(asignarCuentaResponse);

            LogRequest logRequest = new LogRequest
            {
                IdUser = loginResponse.IdUsuario.ToString(),
                Modulo = "Clientes",
                Fecha = HoraHelper.GetHoraCiudadMexico(),
                NombreEquipo = loginResponse.NombreDispositivo,
                Accion = "Asignar Cuenta",
                Ip = loginResponse.DireccionIp,
                Envio = JsonConvert.SerializeObject(asignarCuentaResponse),
                Respuesta = response.Error.ToString(),
                Error = response.Error ? JsonConvert.SerializeObject(response.Detalle) : string.Empty,
                IdRegistro = model.IdCliente
            };
            await _logsApiClient.InsertarLogAsync(logRequest);
        }
        catch (Exception ex)
        {
            response.Detalle = ex.Message;
        }

        return Json(response);
    }

    [Authorize]
    [Route("Clientes/Detalle/Cuentas/DesvincularCuenta")]
    [HttpPost]
    public async Task<JsonResult> DesvincularCuentaCliente(int idCuenta, int idCliente)
    {
        var loginResponse = _userContextService.GetLoginResponse();
        MensajeResponse response = new MensajeResponse();

        try
        {
            DesvincularCuentaResponse asignarCuentaResponse = new DesvincularCuentaResponse
            {
                idCliente = idCliente,
                idCuenta = idCuenta,
                descripcionRol = "admin", //---> se deja por defecto para desvincular
            };

            response = await _clientesApiClient.GetRegistroDesvincularCuentaCliente(asignarCuentaResponse);

            LogRequest logRequest = new LogRequest
            {
                IdUser = loginResponse.IdUsuario.ToString(),
                Modulo = "Clientes",
                Fecha = HoraHelper.GetHoraCiudadMexico(),
                NombreEquipo = loginResponse.NombreDispositivo,
                Accion = "Desasignar Cuenta",
                Ip = loginResponse.DireccionIp,
                Envio = JsonConvert.SerializeObject(asignarCuentaResponse),
                Respuesta = response.Error.ToString(),
                Error = response.Error ? JsonConvert.SerializeObject(response.Detalle) : string.Empty,
                IdRegistro = idCliente
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

    #region Tap Transacciones
    [Authorize]
    [Route("Clientes/Detalle/Movimientos")]
    public async Task<IActionResult> Transacciones(int id, int cliente, string noCuenta)
    {
        var loginResponse = _userContextService.GetLoginResponse();
        var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Clientes" && modulo.Ver == 0);
        if (validacion == true)
        {
            ClienteDetailsResponse user = await _clientesApiClient.GetDetallesGeneralesClienteAsync(cliente);
            List<EmpresaResponse> empresasResponse = await _clientesApiClient.GetEmpresasClienteAsync(cliente);

            if (user.value == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.UrlView = "Movimientos";
            string empresasConcatenadas = string.Join("/", empresasResponse.Select(e => e.Empresa));

            ClientesHeaderViewModel header = new ClientesHeaderViewModel
            {
                Id = user.value.IdCliente,
                NombreCompleto = user.value.Nombre + " " + user.value.ApellidoPaterno + " " + user.value.ApellidoMaterno,
                Telefono = user.value.Telefono ?? "",
                Correo = user.value.Email ?? "",
                Curp = user.value.CURP ?? "",
                Organizacion = empresasConcatenadas ?? "",
                Rfc = user.value.RFC ?? "",
                Sexo = user.value.Sexo ?? "",
                NoCuenta = noCuenta
            };
            ViewBag.Info = header;
            ViewBag.Nombre = header.NombreCompleto;
            ViewBag.NombrePascal = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(header.NombreCompleto.ToLower());


            ViewBag.NumCuenta = noCuenta;

            if ((id == 0) && (noCuenta == null))
            {
                ViewBag.TipoConsulta = "IsGeneral";
                ViewBag.AccountID = cliente;
            }
            else
            {
                ViewBag.TipoConsulta = "IsEspecific";
                ViewBag.AccountID = id;
            }

            RegistrarTransaccionRequest renderInfo = new RegistrarTransaccionRequest
            {
                //NombreOrdenante = header.NombreCompleto,
                ClaveRastreo = string.Format("AQPAY1000000{0}", DateTime.Now.ToString("yyyymmddhhmmss"))
            };
            ViewBag.DatosRef = renderInfo;
            return View("~/Views/Clientes/Detalles/Transacciones/DetalleMovimientos.cshtml");
        }

        return RedirectToAction("Index", "Home");
    }

    #endregion

    #region Tap Tarjetas
    [Authorize]
    [Route("Clientes/Detalle/Tarjetas")]
    public async Task<IActionResult> Tarjetas(int id)
    {
        var loginResponse = _userContextService.GetLoginResponse();
        var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Clientes" && modulo.Ver == 0);
        if (validacion == true)
        {
            ClienteDetailsResponse user = await _clientesApiClient.GetDetallesGeneralesClienteAsync(id);
            List<EmpresaResponse> empresasResponse = await _clientesApiClient.GetEmpresasClienteAsync(id);

            if (user.value == null)
            {
                return RedirectToAction("Index");
            }

            string empresasConcatenadas = string.Join("/", empresasResponse.Select(e => e.Empresa));

            ViewBag.UrlView = "Tarjetas";
            ClientesHeaderViewModel header = new ClientesHeaderViewModel
            {
                Id = user.value.IdCliente,
                NombreCompleto = user.value.Nombre + " " + user.value.ApellidoPaterno + " " + user.value.ApellidoMaterno,
                Telefono = user.value.Telefono ?? "",
                Correo = user.value.Email ?? "",
                Curp = user.value.CURP ?? "",
                Organizacion = empresasConcatenadas ?? "",
                Rfc = user.value.RFC ?? "",
                Sexo = user.value.Sexo ?? ""
            };

            RegistrarTarjetaRequest renderRef = new RegistrarTarjetaRequest
            {
                idCliente = id
            };

            ViewBag.DatosRef = renderRef;
            ViewBag.Info = header;
            ViewBag.Nombre = header.NombreCompleto;
            ViewBag.AccountID = id;
            ViewBag.NombrePascal = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(header.NombreCompleto.ToLower());

            return View("~/Views/Clientes/Detalles/Tarjetas/DetalleTarjetas.cshtml", loginResponse);
        }

        return RedirectToAction("Index", "Tarjetas");
    }

    [Authorize]
    [HttpPost]
    public async Task<JsonResult> ConsultaTarjetas(string id)
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

        ListPF = await _tarjetasApiClient.GetTarjetasClientesAsync(Convert.ToInt32(id));

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
                tipoProducto = row.tipoProducto ?? "-",
                //Estatus = row.Estatus
                //Acciones = await this.RenderViewToStringAsync("~/Views/Tarjetas/_Acciones.cshtml", row)
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
            ).ToList();
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

    #region Tap Documentos

    [Authorize]
    [Route("Clientes/Detalle/Documentos")]
    public async Task<IActionResult> DocumentosCliente(int id)
    {
        var loginResponse = _userContextService.GetLoginResponse();
        var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Clientes" && modulo.Ver == 0);
        if (validacion == true)
        {
            ClienteDetailsResponse user = await _clientesApiClient.GetDetallesGeneralesClienteAsync(id);
            var listaDocumentos = await _catalogosApiClient.GetTipoDocumentosAsync();
            List<EmpresaResponse> empresasResponse = await _clientesApiClient.GetEmpresasClienteAsync(id);

            if (user.value == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.UrlView = "Documentos";
            string empresasConcatenadas = string.Join("/", empresasResponse.Select(e => e.Empresa));

            ClientesHeaderViewModel header = new ClientesHeaderViewModel
            {
                Id = user.value.IdCliente,
                NombreCompleto = user.value.Nombre + " " + user.value.ApellidoPaterno + " " + user.value.ApellidoMaterno,
                Telefono = user.value.Telefono ?? "",
                Correo = user.value.Email ?? "",
                Curp = user.value.CURP ?? "",
                Organizacion = empresasConcatenadas ?? "",
                Rfc = user.value.RFC ?? "",
                Sexo = user.value.Sexo ?? ""
            };

            DocumentosClienteRegistro renderRef = new DocumentosClienteRegistro
            {
                idCliente = id,
                listaDocs = listaDocumentos
            };

            ViewBag.DatosRef = renderRef;
            ViewBag.Info = header;
            ViewBag.Nombre = header.NombreCompleto;
            ViewBag.AccountID = id;
            ViewBag.NombrePascal = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(header.NombreCompleto.ToLower());

            return View("~/Views/Clientes/Detalles/Documentos/DetalleDocumentos.cshtml", loginResponse);
        }

        return NotFound();
    }

    [Authorize]
    [Route("Clientes/Detalle/Documentos/CargarDocumentoCliente")]
    [HttpPost]
    public async Task<ActionResult> CargarDocumentoCliente(DocumentosClienteRegistro model)
    {
        DocumentoClienteRequest? sendDocumento = new DocumentoClienteRequest();
        MensajeResponse? response = new MensajeResponse();

        try
        {
            if (model.documento.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    model.documento.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    var extension = System.IO.Path.GetExtension(model.documento.FileName);

                    //if (extension.ToLower() == ".pdf") {
                    //    sendDocumento.documento = "data:application/pdf;base64,";
                    //} else {
                    //    sendDocumento.documento = string.Format("data:image/{0};base64,", extension.Replace(".", ""));
                    //}

                    sendDocumento.documento = Convert.ToBase64String(fileBytes);
                }
            }

            sendDocumento.idCliente = model.idCliente;
            sendDocumento.tipoDocumento = model.tipoDocumento;
            //sendDocumento.Observaciones = model.Observaciones;
            //sendDocumento.NombreDocumento = model.documento.FileName;
            sendDocumento.documento = sendDocumento.documento.Trim();

            response = await _clientesApiClient.GetInsertaDocumentoClienteAsync(sendDocumento);

        }
        catch (Exception ex)
        {
            response.Error = true;
        }

        return Json(response);
    }

    [Authorize]
    [HttpPost]
    public async Task<JsonResult> ConsultarListadoDocumentos(string idAccount)
    {

        var gridData = new ResponseGrid<DocumentosClienteResponseGrid>();

        try
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

            DocumentosClienteDetailsResponse ListPF = new DocumentosClienteDetailsResponse();
            ListPF = await _clientesApiClient.GetDocumentosClienteAsync(Convert.ToInt32(idAccount));

            var List = new List<DocumentosClienteResponseGrid>();

            if (ListPF.value != null)
            {
                foreach (var row in ListPF.value)
                {
                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(row.ruta_documento);
                    var ally = System.Convert.ToBase64String(plainTextBytes);

                    row.urlAlly = ally;

                    List.Add(new DocumentosClienteResponseGrid
                    {
                        IdCliente = row.IdCliente,
                        DescripcionDocumento = row.DescripcionDocumento,
                        tipo_documento = row.tipo_documento,
                        fechaalta = row.fecha_alta.ToString(),
                        fechaactualizacion = row.fecha_actualizacion.ToString(),
                        Observaciones = row.Observaciones,
                        urlAlly = ally,
                        Acciones = await this.RenderViewToStringAsync("~/Views/Clientes/_AccionesDocumentos.cshtml", row)
                    });
                }
            }

            if (!string.IsNullOrEmpty(request.Busqueda))
            {
                List = List.Where(x =>
                (x.DescripcionDocumento?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
                (x.fechaalta.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
                (x.fechaactualizacion.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
                (x.Observaciones?.ToLower() ?? "").Contains(request.Busqueda.ToLower())
                ).ToList();
            }

            gridData.RecordsTotal = List.Count;
            gridData.Data = List.Skip(skip).Take(pageSize).ToList();
            filterRecord = string.IsNullOrEmpty(request.Busqueda) ? gridData.RecordsTotal ?? 0 : gridData.Data.Count;
            gridData.RecordsFiltered = filterRecord;
            gridData.Draw = draw;

        }
        catch (Exception ex)
        {
            return Json(gridData);
        }

        return Json(gridData);
    }

    [Route("Clientes/Detalle/Documentos/VerDocumentoCliente")]
    [HttpPost]
    public async Task<JsonResult> VerDocumentoCliente(string DocAlly)
    {

        MensajeArchivoResponse? result = new MensajeArchivoResponse();

        try
        {
            var base64EncodedBytes = System.Convert.FromBase64String(DocAlly);
            var url = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

            DocumentoShowResponse response = await _clientesApiClient.GetVisualizarDocumentosClienteAsync(url);

            if (response.Documento == null) {
                result.Error = true;
                return Json(result);
            }

            var ArchivoUsuario = File(response.Documento, response.MimeType, response.Nombre);

            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                ArchivoUsuario.FileStream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            if (response.MimeType == "application/pdf")
            {
                result.Codigo = "PDF";
            }
            else
            {
                result.Codigo = "image";
            }

            result.Error = false;
            result.Archivo64 = Convert.ToBase64String(bytes);

        }
        catch (Exception ex)
        {
            result.Error = true;
        }

        return Json(result);

    }

    #endregion

    #endregion

    #region Consulta Personas Morales

    [Authorize]
    public IActionResult PersonasMorales()
    {
        var loginResponse = _userContextService.GetLoginResponse();
        var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Clientes" && modulo.Ver == 0);
        if (validacion == true)
            return View(loginResponse);

        return NotFound();
    }

    [Authorize]
    [HttpPost]
    public async Task<JsonResult> ConsultaPersonasMorales(List<RequestListFilters> filters)
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
            if (request.ColumnaOrdenamiento == "Nombre")
            {
                columna = 1;
            }
            else if (request.ColumnaOrdenamiento == "Telefono")
            {
                columna = 2;
            }
            else if (request.ColumnaOrdenamiento == "Email")
            {
                columna = 3;
            }
            else if (request.ColumnaOrdenamiento == "RFC")
            {
                columna = 4;
            }
            else if (request.ColumnaOrdenamiento == "Giro")
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

        var gridData = new ResponseGrid<DatosClienteMoralGrid>();
        List<DatosClienteMoralResponse> ListPF = new List<DatosClienteMoralResponse>();

        if (!string.IsNullOrEmpty(request.Busqueda))
        {
            filters.RemoveAll(x => x.Key == "nombreCliente");

            filters.Add(new RequestListFilters
            {
                Key = "nombreCliente",
                Value = request.Busqueda
            });
        }

        //Validar si hay algun filtro con valor ingresado
        var validaFiltro = filters.Where(x => x.Value != null).ToList();

        if (validaFiltro.Count != 0)
        {
            (ListPF, paginacion) = await _personaMoralServices.GetPersonasMoralesFilterAsync(Convert.ToInt32(request.Pagina), Convert.ToInt32(request.Registros), columna, tipoFiltro, filters);
        }
        else
        {
            (ListPF, paginacion) = await _personaMoralServices.GetPersonasMoralesAsync(Convert.ToInt32(request.Pagina), Convert.ToInt32(request.Registros), columna, tipoFiltro);
        }

        var List = new List<DatosClienteMoralGrid>();
        foreach (var row in ListPF)
        {
            List.Add(new DatosClienteMoralGrid
            {
                Id = row.Id,
                Nombre = row.Nombre == null ? "-" : row.Nombre,
                vinculo = !string.IsNullOrWhiteSpace(row.Nombre) ? (row.Nombre + "|" + row.Id.ToString()).ToUpper() : "-",
                Telefono = !string.IsNullOrWhiteSpace(row.Telefono) ? row.Telefono : "-",
                Email = !string.IsNullOrWhiteSpace(row.Email) ? row.Email : "-",
                RFC = !string.IsNullOrWhiteSpace(row.RFC) ? row.RFC : "-",
                Giro = !string.IsNullOrWhiteSpace(row.Giro) ? row.Giro : "-",
                Acciones = await this.RenderViewToStringAsync("~/Views/Clientes/_AccionesMoral.cshtml", row)
            });
        }

        gridData.Data = List;
        gridData.RecordsTotal = paginacion;
        filterRecord = string.IsNullOrEmpty(request.Busqueda) ? gridData.RecordsTotal ?? 0 : gridData.RecordsTotal ?? 0;
        gridData.RecordsFiltered = filterRecord;
        gridData.Draw = draw;

        request.Busqueda = "";
        return Json(gridData);
    }


    #endregion

    #region Detalles Personas Morales

    #region Tap Datos Generales
    [Authorize]
    [Route("Clientes/DetallePersonaMoral/DatosGeneralesPersonaMoral")]
    public async Task<IActionResult> DatosGeneralesPersonaMoral(int id)
    {
        try
        {
            var loginResponse = _userContextService.GetLoginResponse();
            var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Clientes" && modulo.Ver == 0);
            if (validacion == true)
            {
                DatosClienteMoralResponse user = await _personaMoralServices.GetDetallesPersonaMoral(id);

                if (user == null)
                {
                    return RedirectToAction("Index");
                }

                PersonaMoralDetallesViewModel pMoralDetallesViewModel = new PersonaMoralDetallesViewModel
                {
                    Id = user.Id,
                    Nombre = user.Nombre.IsNullOrEmpty() ? "-" : user.Nombre,
                    Telefono = user.Telefono.IsNullOrEmpty() ? "-" : user.Telefono,
                    Email = user.Email.IsNullOrEmpty() ? "-" : user.Email,
                    TipoEmpresa = user.TipoEmpresa.IsNullOrEmpty() ? "-" : user.TipoEmpresa,
                    RFC = user.RFC.IsNullOrEmpty() ? "-" : user.RFC,
                    NumeroIdentificacion = user.NumeroIdentificacion.IsNullOrEmpty() ? "-" : user.NumeroIdentificacion,
                    FechaCreacion = user.FechaCreacion.IsNullOrEmpty() ? "-" : user.FechaCreacion,
                    Giro = user.Giro.IsNullOrEmpty() ? "-" : user.Giro,
                    Direccion = user.Direccion.IsNullOrEmpty() ? "-" : user.Direccion,
                    Estado = user.Estado.IsNullOrEmpty() ? "-" : user.Estado,
                    Municipio = user.Municipio.IsNullOrEmpty() ? "-" : user.Municipio,
                    Colonia = user.Colonia.IsNullOrEmpty() ? "-" : user.Colonia,
                    Calle = user.Calle.IsNullOrEmpty() ? "-" : user.Calle,
                    PrimeraCalle = user.PrimeraCalle.IsNullOrEmpty() ? "-" : user.PrimeraCalle,
                    SegundaCalle = user.SegundaCalle.IsNullOrEmpty() ? "-" : user.SegundaCalle,
                    NumeroExterior = user.NumeroExterior.IsNullOrEmpty() ? "-" : user.NumeroExterior,
                    NumeroInterior = user.NumeroInterior.IsNullOrEmpty() ? "-" : user.NumeroInterior,
                    CodigoPostal = user.CodigoPostal.IsNullOrEmpty() ? "-" : user.CodigoPostal,
                    RegistroIMSS = user.RegistroIMSS.IsNullOrEmpty() ? "-" : user.RegistroIMSS,
                    RegimenFiscal = user.RegimenFiscal.IsNullOrEmpty() ? "-" : user.RegimenFiscal,
                };

                PersonaMoralHeaderViewModel header = new PersonaMoralHeaderViewModel
                {
                    Id = user.Id,
                    Nombre = user.Nombre.IsNullOrEmpty() ? "-" : user.Nombre,
                    Telefono = user.Telefono.IsNullOrEmpty() ? "-" : user.Telefono,
                    Correo = user.Email.IsNullOrEmpty() ? "-" : user.Email,
                    Rfc = user.RFC.IsNullOrEmpty() ? "-" : user.RFC,
                    RegimenFiscal = user.RegimenFiscal.IsNullOrEmpty() ? "-" : user.RegimenFiscal
                };

                ViewBag.Info = header;
                ViewBag.UrlView = "DatosGenerales";
                ViewBag.NombrePascal = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(header.Nombre.ToString().ToLower());

                var validacionEdicion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Clientes" && modulo.Editar == 0);
                if (validacionEdicion == true)
                {
                    ViewBag.ValEdicion = true;
                }
                else
                {
                    ViewBag.ValEdicion = false;
                }


                return View("~/Views/Clientes/DetallesPersonaMoral/DatosGenerales/DetallePersonaMoral.cshtml", pMoralDetallesViewModel);
            }

            return RedirectToAction("Index");

        }
        catch (Exception ex)
        {
            return RedirectToAction("Index");
        }

    }
    #endregion

    #region Tap Cuentas

    [Authorize]
    [Route("Clientes/DetallePersonaMoral/Cuentas")]
    public async Task<IActionResult> CuentasPersonaMoral(int id)
    {
        var loginResponse = _userContextService.GetLoginResponse();
        var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Clientes" && modulo.Ver == 0);
        if (validacion == true)
        {
            DatosClienteMoralResponse user = await _personaMoralServices.GetDetallesPersonaMoral(id);
            var listaRoles = await _catalogosApiClient.GetRolClienteAsync();
            var listaEmpresa = await _catalogosApiClient.GetEmpresasAsync();

            if (user == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.UrlView = "Cuentas";

            PersonaMoralHeaderViewModel header = new PersonaMoralHeaderViewModel
            {
                Id = user.Id,
                Nombre = user.Nombre.IsNullOrEmpty() ? "-" : user.Nombre,
                Telefono = user.Telefono.IsNullOrEmpty() ? "-" : user.Telefono,
                Correo = user.Email.IsNullOrEmpty() ? "-" : user.Email,
                Rfc = user.RFC.IsNullOrEmpty() ? "-" : user.RFC,
                RegimenFiscal = user.RegimenFiscal.IsNullOrEmpty() ? "-" : user.RegimenFiscal
            };

            AsignarCuentaDetailsResponse asign = new AsignarCuentaDetailsResponse
            {
                IdCliente = user.Id,
                ListaRoles = listaRoles,
                ListaEmpresa = listaEmpresa
            };

            ViewBag.Info = header;
            ViewBag.Nombre = header.Nombre;
            ViewBag.AccountID = id;
            ViewBag.AsignData = asign;
            ViewBag.NombrePascal = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(header.Nombre.ToLower());

            var validacionEdicion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Cuentas" && modulo.Editar == 0);
            if (validacionEdicion == true)
            {
                ViewBag.ValEdicion = true;
            }
            else
            {
                ViewBag.ValEdicion = false;
            }

            return View("~/Views/Clientes/DetallesPersonaMoral/Cuentas/DetalleCuentas.cshtml");
        }

        return NotFound();
    }

    [Authorize]
    [HttpPost]
    public async Task<JsonResult> ConsultaCuentasPersonaMorales(string id)
    {
        var loginResponse = _userContextService.GetLoginResponse();
        JsonResult result = new JsonResult("");
        //Invoca al método que se encarga de realizar la petición Api
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

        var gridData = new ResponseGrid<CuentasPersonaMoralResponseGrid>();
        List<CuentasPersonaMoralResponse> ListPF = new List<CuentasPersonaMoralResponse>();

        ListPF = await _cuentaApiClient.GetCuentasByEmpresasAsync(Convert.ToInt32(request.Pagina), Convert.ToInt32(request.Registros), Convert.ToInt32(id));

        ////Almacena el subtotal de registro en caso de obtener un parámetro de búsqueda
        var List = new List<CuentasPersonaMoralResponseGrid>();
        foreach (var row in ListPF)
        {
            var validacionEdicion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Clientes" && modulo.Editar == 0);
            if (validacionEdicion == true)
            {
                row.validarPermiso = true;
            }
            else
            {
                row.validarPermiso = false;
            }

            List.Add(new CuentasPersonaMoralResponseGrid
            {
                IdCuenta = row.IdCuenta,
                NombrePersona = row.NombrePersona,
                vinculo = row.NoCuenta + "|" + row.IdCuenta.ToString() + "|" + id,
                NoCuenta = row.NoCuenta,
                Saldo = row.Saldo,
                Estatus = row.Estatus,
                TipoPersona = row.TipoPersona ?? "-",
                Alias = !string.IsNullOrEmpty(row.Alias) ? row.Alias : "-",
                FechaActualizacion = row.FechaActualizacion ?? "-",
                FechaAlta = row.FechaAlta ?? "-",
                Acciones = await this.RenderViewToStringAsync("~/Views/Clientes/DetallesPersonaMoral/Cuentas/_Acciones.cshtml", row)
            });
        }

        //if (!string.IsNullOrEmpty(request.Busqueda))
        //{
        //    if (request.Busqueda.ToLower() == "activo")
        //    {
        //        request.Busqueda = "2";
        //    }
        //    else if (request.Busqueda.ToLower() == "desactivado")
        //    {
        //        request.Busqueda = "1";
        //    }

        //    List = List.Where(x =>
        //    (x.nombrePersona?.ToUpper() ?? "").Contains(request.Busqueda.ToUpper()) ||
        //    (x.noCuenta?.ToLower() ?? "").Contains(request.Busqueda.ToUpper()) ||
        //    (x.alias?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
        //    (x.saldo.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
        //    (x.fechaActualizacion.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
        //    (x.fechaAlta.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower())
        //    ).ToList();
        //}

        gridData.RecordsTotal = List.Count;
        gridData.Data = List.Skip(skip).Take(pageSize).ToList();
        filterRecord = string.IsNullOrEmpty(request.Busqueda) ? gridData.RecordsTotal ?? 0 : gridData.Data.Count;
        gridData.RecordsFiltered = filterRecord;
        gridData.Draw = draw;

        return Json(gridData);
    }
    #endregion

    #region Tap Movimientos 
    [Authorize]
    [Route("Clientes/DetallePersonaMoral/Movimientos")]
    public async Task<IActionResult> TransaccionesPersonaMoral(int id, int cliente, string noCuenta)
    {
        var loginResponse = _userContextService.GetLoginResponse();
        var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Clientes" && modulo.Ver == 0);
        if (validacion == true)
        {
            DatosClienteMoralResponse user = await _personaMoralServices.GetDetallesPersonaMoral(cliente);

            if (user == null) {
                return RedirectToAction("Index");
            }

            ViewBag.UrlView = "Movimientos";

            PersonaMoralHeaderViewModel header = new PersonaMoralHeaderViewModel
            {
                Id = user.Id,
                Nombre = user.Nombre.IsNullOrEmpty() ? "-" : user.Nombre,
                Telefono = user.Telefono.IsNullOrEmpty() ? "-" : user.Telefono,
                Correo = user.Email.IsNullOrEmpty() ? "-" : user.Email,
                Rfc = user.RFC.IsNullOrEmpty() ? "-" : user.RFC,
                RegimenFiscal = user.RegimenFiscal.IsNullOrEmpty() ? "-" : user.RegimenFiscal
            };
            ViewBag.Info = header;
            ViewBag.Nombre = header.Nombre;
            ViewBag.NombrePascal = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(header.Nombre.ToLower());


            ViewBag.NumCuenta = noCuenta;

            if ((id == 0) && (noCuenta == null)) {
                ViewBag.TipoConsulta = "IsGeneral";
                ViewBag.AccountID = cliente;
            } else {
                ViewBag.TipoConsulta = "IsEspecific";
                ViewBag.AccountID = id;
            }

            return View("~/Views/Clientes/DetallesPersonaMoral/Transacciones/DetalleMovimientos.cshtml");
        }

        return NotFound();
    }

    [HttpPost]
    public async Task<JsonResult> ConsultaMovimientosPersonaMoral(string id, string TipoConsulta)
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

        if (ListPF != null)
        {
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
    #endregion

    #region Tap Tarjetas
    [Authorize]
    [Route("Clientes/DetallePersonaMoral/Tarjetas")]
    public async Task<IActionResult> TarjetasPersonaMoral(int id)
    {
        var loginResponse = _userContextService.GetLoginResponse();
        var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Clientes" && modulo.Ver == 0);
        if (validacion == true)
        {
            DatosClienteMoralResponse user = await _personaMoralServices.GetDetallesPersonaMoral(id);

            if (user == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.UrlView = "Tarjetas";
            PersonaMoralHeaderViewModel header = new PersonaMoralHeaderViewModel
            {
                Id = user.Id,
                Nombre = user.Nombre.IsNullOrEmpty() ? "-" : user.Nombre,
                Telefono = user.Telefono.IsNullOrEmpty() ? "-" : user.Telefono,
                Correo = user.Email.IsNullOrEmpty() ? "-" : user.Email,
                Rfc = user.RFC.IsNullOrEmpty() ? "-" : user.RFC,
                RegimenFiscal = user.RegimenFiscal.IsNullOrEmpty() ? "-" : user.RegimenFiscal
            };

            RegistrarTarjetaRequest renderRef = new RegistrarTarjetaRequest
            {
                idCliente = id
            };

            ViewBag.DatosRef = renderRef;
            ViewBag.Info = header;
            ViewBag.Nombre = header.Nombre;
            ViewBag.AccountID = id;
            ViewBag.NombrePascal = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(header.Nombre.ToLower());

            return View("~/Views/Clientes/DetallesPersonaMoral/Tarjetas/DetalleTarjetas.cshtml", loginResponse);
        }

        return RedirectToAction("Index", "Tarjetas");
    }
    #endregion

    #region Tap Empleados
    [Authorize]
    [Route("Clientes/DetallePersonaMoral/Empleados")]
    public async Task<IActionResult> EmpleadosPersonaMoral(int id)
    {
        var loginResponse = _userContextService.GetLoginResponse();
        var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Clientes" && modulo.Ver == 0);
        if (validacion == true)
        {
            DatosClienteMoralResponse user = await _personaMoralServices.GetDetallesPersonaMoral(id);

            if (user == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.UrlView = "Empleados";
            PersonaMoralHeaderViewModel header = new PersonaMoralHeaderViewModel
            {
                Id = user.Id,
                Nombre = user.Nombre.IsNullOrEmpty() ? "-" : user.Nombre,
                Telefono = user.Telefono.IsNullOrEmpty() ? "-" : user.Telefono,
                Correo = user.Email.IsNullOrEmpty() ? "-" : user.Email,
                Rfc = user.RFC.IsNullOrEmpty() ? "-" : user.RFC,
                RegimenFiscal = user.RegimenFiscal.IsNullOrEmpty() ? "-" : user.RegimenFiscal
            };

            ViewBag.AccountID = id;
            ViewBag.Info = header;
            ViewBag.Nombre = header.Nombre;
            ViewBag.NombrePascal = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(header.Nombre.ToLower());

            return View("~/Views/Clientes/DetallesPersonaMoral/Empleados/DetalleEmpleados.cshtml");
        }

        return RedirectToAction("Index");
    }

    [Authorize]
    [HttpPost]
    public async Task<JsonResult> ConsultaEmpleados(string id)
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

        var gridData = new ResponseGrid<EmpresaClienteResponseGrid>();
        List<EmpresaClienteResponse> ListPF = new List<EmpresaClienteResponse>();

        (ListPF, paginacion) = await _personaMoralServices.GetEmpleadosPersonaMoralAsync(Convert.ToInt32(request.Pagina), Convert.ToInt32(request.Registros), Convert.ToInt32(id));

        var List = new List<EmpresaClienteResponseGrid>();
        foreach (var row in ListPF)
        {
            List.Add(new EmpresaClienteResponseGrid
            {
                NombreCompleto = !string.IsNullOrWhiteSpace(row.NombreCompleto) ? row.NombreCompleto : "-",
                Email = !string.IsNullOrWhiteSpace(row.Email) ? row.Email : "-",
                Curp = !string.IsNullOrWhiteSpace(row.Curp) ? row.Curp : "-",
                Puesto = !string.IsNullOrWhiteSpace(row.Puesto) ? row.Puesto : "-",
                Area = !string.IsNullOrWhiteSpace(row.Area) ? row.Area : "-",
                Roles = !string.IsNullOrWhiteSpace(row.Roles) ? row.Roles : "-",
                vinculo = !string.IsNullOrWhiteSpace(row.NombreCompleto) ? (row.NombreCompleto + "|" + row.IdCliente.ToString()).ToUpper() : "-",
                active = row.active,
                EstatusWeb = row.EstatusWeb
            });
        }

        //Filtro TextBox Busqueda Rapida
        if (!string.IsNullOrEmpty(request.Busqueda))
        {
            List = List.Where(x =>
            (x.NombreCompleto?.ToUpper() ?? "").Contains(request.Busqueda.ToUpper()) ||
            (x.Email?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.Curp?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.Puesto?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.Area?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.Roles?.ToLower() ?? "").Contains(request.Busqueda.ToLower())
            ).ToList();
        }

        gridData.Data = List;
        gridData.RecordsTotal = paginacion;
        filterRecord = string.IsNullOrEmpty(request.Busqueda) ? gridData.RecordsTotal ?? 0 : gridData.RecordsTotal ?? 0;
        gridData.RecordsFiltered = filterRecord;
        gridData.Draw = draw;

        request.Busqueda = "";
        return Json(gridData);
    }
    #endregion

    #endregion

    #region Registro Clientes
    [Authorize]
    public async Task<ActionResult> Registro()
    {
        var loginResponse = _userContextService.GetLoginResponse();
        var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Clientes" && modulo.Insertar == 0);
        if (validacion == true)
        {
            var listaEmpresas = await _catalogosApiClient.GetEmpresasAsync();
            var listaRoles = await _catalogosApiClient.GetRolClienteAsync();
            var listaOcupaciones = await _catalogosApiClient.GetOcupacionesAsync();
            //var listaPaises = await _catalogosApiClient.GetPaisesAsync();
            var listaPaises = Paises.ListaDePaises;
            var listaNacionalidades = await _catalogosApiClient.GetNacionalidadesAsync();

            ClientesRegistroViewModel clientesRegistroViewModel = new ClientesRegistroViewModel
            {
                ClientesDetalles = new RegistroModificacionClienteRequest(),
                ListaEmpresas = listaEmpresas,
                ListaRoles = listaRoles,
                ListaOcupaciones = listaOcupaciones,
                ListaPaises = listaPaises,
                ListaNacionalidades = listaNacionalidades,
                ApoderadoLegalOpciones = ApoderadoLegalOpciones.Opciones
            };

            ViewBag.Accion = "RegistrarCliente";
            ViewBag.TituloForm = "Registrar Persona Física";
            return View(clientesRegistroViewModel);
        }

        return NotFound();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> RegistrarCliente(ClientesRegistroViewModel model)
    {
        var loginResponse = _userContextService.GetLoginResponse();
        var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Clientes" && modulo.Insertar == 0);
        if (validacion == true)
        {
            MensajeResponse response = new MensajeResponse();
            string detalle = "";

            try
            {
                response = await _clientesApiClient.GetRegistroCliente(model.ClientesDetalles);
                detalle = response.Detalle;
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
                    Modulo = "Clientes",
                    Fecha = HoraHelper.GetHoraCiudadMexico(),
                    NombreEquipo = loginResponse.NombreDispositivo,
                    Accion = "Insertar",
                    Ip = loginResponse.DireccionIp,
                    Envio = JsonConvert.SerializeObject(model.ClientesDetalles),
                    Respuesta = response.Error.ToString() ?? "Ya existe un Correo y/o Teléfono",
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
                    return Ok(new { success = false, response.Detalle});
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        return NotFound();
    }
    #endregion

    #region Modificar Cliente
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ModificarCliente(ClientesRegistroViewModel model)
    {
        var loginResponse = _userContextService.GetLoginResponse();
        var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Clientes" && modulo.Editar == 0);
        if (validacion == true)
        {
            try
            {
                var response = await _clientesApiClient.GetModificarCliente(model.ClientesDetalles);

                LogRequest logRequest = new LogRequest
                {
                    IdUser = loginResponse.IdUsuario.ToString(),
                    Modulo = "Clientes",
                    Fecha = HoraHelper.GetHoraCiudadMexico(),
                    NombreEquipo = loginResponse.NombreDispositivo,
                    Accion = "Editar",
                    Ip = loginResponse.DireccionIp,
                    Envio = JsonConvert.SerializeObject(model.ClientesDetalles),
                    Respuesta = response.Error.ToString(),
                    Error = response.Error ? JsonConvert.SerializeObject(response.Detalle) : string.Empty,
                    IdRegistro = model.ClientesDetalles.IdCliente
                };
                await _logsApiClient.InsertarLogAsync(logRequest);

                if (response.Codigo == "200")
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Registro");
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
    [Route("Clientes/Detalle/Modificar")]
    public async Task<ActionResult> Modificar(int id)
    {
        ViewData["IsEdit"] = true;
        var loginResponse = _userContextService.GetLoginResponse();
        var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Clientes" && modulo.Editar == 0);
        if (validacion == true)
        {
            try
            {
                ClienteDetailsResponse cliente = await _clientesApiClient.GetDetallesCliente(id);
                RegistroModificacionClienteRequest clientesDetalles = new RegistroModificacionClienteRequest
                {
                    IdCliente = cliente.value.IdCliente,
                    Nombre = cliente.value.Nombre,
                    Telefono = cliente.value.Telefono,
                    Email = cliente.value.Email,
                    Curp = cliente.value.CURP,
                    Sexo = cliente.value.Sexo,
                    EntreCalleSegunda = cliente.value.CalleSecundaria2,
                    EntreCallePrimera = cliente.value.CalleSecundaria,
                    INE = cliente.value.INE,
                    Calle = cliente.value.Calle,
                    ApellidoMaterno = cliente.value.ApellidoMaterno,
                    ApellidoPaterno = cliente.value.ApellidoPaterno,
                    CodigoPostal = cliente.value.CodigoPostal,
                    Colonia = cliente.value.Colonia,
                    Observaciones = cliente.value.Observaciones,
                    RFC = cliente.value.RFC,
                    Fiel = cliente.value.Fiel,
                    PaisNacimiento = cliente.value.PaisNacimiento,
                    IngresoMensual = cliente.value.SalarioMensual.ToString(),
                    ApoderadoLegal = cliente.value.ApoderadoLegal ?? 0,
                    NoInterior = cliente.value.NoInt,
                    Puesto = cliente.value.Puesto,
                    FechaNacimiento = cliente.value.FechaNacimiento != null
                    ? DateTime.ParseExact(cliente.value.FechaNacimiento, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                    : null,
                    DelegacionMunicipio = cliente.value.Municipio,
                    TelefonoTipo = cliente.value.TelefonoRecado,
                    IdNacionalidad = cliente.value.IdNacionalidad ?? 0,
                    IdOcupacion = cliente.value.IdOcupacion ?? 0,
                    CiudadEstado = cliente.value.Estado,
                    Rol = cliente.value.Rol,
                    Empresa = cliente.value.IdEmpresa ?? 0,
                    MontoMaximo = cliente.value.MontoMaximo?.ToString(),
                    CalleNumero = cliente.value.CalleNumero,
                    IdPais = cliente.value.IdPais ?? 0
                };
                var listaEmpresas = await _catalogosApiClient.GetEmpresasAsync();
                var listaRoles = await _catalogosApiClient.GetRolClienteAsync();
                var listaOcupaciones = await _catalogosApiClient.GetOcupacionesAsync();
                //var listaPaises = await _catalogosApiClient.GetPaisesAsync();
                var listaPaises = Paises.ListaDePaises;
                var listaNacionalidades = await _catalogosApiClient.GetNacionalidadesAsync();

                if (clientesDetalles == null)
                {
                    return RedirectToAction("Error404", "Error");
                }

                ClientesRegistroViewModel clientesRegistroViewModel = new ClientesRegistroViewModel
                {
                    ClientesDetalles = clientesDetalles,
                    ListaEmpresas = listaEmpresas,
                    ListaNacionalidades = listaNacionalidades,
                    ListaOcupaciones = listaOcupaciones,
                    ListaPaises = listaPaises,
                    ListaRoles = listaRoles,
                    ApoderadoLegalOpciones = ApoderadoLegalOpciones.Opciones
                };

                ViewBag.Accion = "ModificarCliente";
                ViewBag.TituloForm = "Modificar Cliente";
                return View("~/Views/Clientes/Registro.cshtml", clientesRegistroViewModel);
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Error");
            }
        }

        return NotFound();

    }
    #endregion

    #region Auxiliares
    [Authorize]
    [HttpPost]
    public async Task<ActionResult> BuscarClientes(string nombreCliente)
    {
        var listaClientes = await _clientesApiClient.GetClientesbyNombreAsync(nombreCliente);
        if (listaClientes.Count == 0)
        {
            return BadRequest();
        }
        return Json(listaClientes);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult> BuscarCoincidenciaNombre(string nombreCompleto)
    {
        var listaClientes = await _clientesApiClient.GetDetallesClientesByNombresAsync(nombreCompleto);
        return Json(listaClientes);
    }

    #endregion

    #endregion

    #region "Modelos"

    #endregion

    #region "Funciones Auxiliares"

    public async Task<IActionResult> ValidarCorreo(string correo)
    {
        try
        {
            var response =  await _clientesApiClient.GetClienteExisteCorreoAsync(correo);
            if (response.Codigo == "200")
            {
                return Ok(new { success = true });
            }
            else
            {
                return Ok(new { success = false, response.Detalle });
            }
        }
        catch (Exception)
        {
            return BadRequest(new { success = false, mensaje = "Error de comunicación" });
        }
    }

    public async Task<IActionResult> ValidarTelefono(string telefono)
    {
        try
        {
            var response = await _clientesApiClient.GetClienteExisteTelefonoAsync(telefono);
            if (response.Codigo == "200")
            {
                return Ok(new { success = true });
            }
            else
            {
                return Ok(new { success = false, response.Detalle });
            }
        }
        catch (Exception)
        {
            return BadRequest(new { success = false, mensaje = "Error de comunicación" });
        }
    }
    #endregion
}
