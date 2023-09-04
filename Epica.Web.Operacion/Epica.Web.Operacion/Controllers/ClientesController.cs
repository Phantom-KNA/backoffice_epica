﻿using Epica.Web.Operacion.Config;
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

namespace Epica.Web.Operacion.Controllers;

public class ClientesController : Controller
{
    private readonly ILogsApiClient _logsApiClient;
    #region "Locales"
    private readonly IClientesApiClient _clientesApiClient;
    private readonly ICuentaApiClient _cuentaApiClient;
    private readonly ITarjetasApiClient _tarjetasApiClient;
    private readonly UserContextService _userContextService;
    private readonly ICatalogosApiClient _catalogosApiClient;
    #endregion

    #region "Constructores"
    public ClientesController(IClientesApiClient clientesApiClient,
        ICuentaApiClient cuentaApiClient,
        ICatalogosApiClient catalogosApiClient,
        ITarjetasApiClient tarjetasApiClient,
        UserContextService userContextService,
        ILogsApiClient logsApiClient
        )
    {
        _logsApiClient = logsApiClient;
        _clientesApiClient = clientesApiClient;
        _cuentaApiClient = cuentaApiClient;
        _userContextService = userContextService;
        _catalogosApiClient = catalogosApiClient;
        _tarjetasApiClient = tarjetasApiClient;
    }
    #endregion

    #region "Funciones"

    #region Consulta Clientes

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

        var draw = Request.Form["draw"].FirstOrDefault();
        int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
        int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

        request.Pagina = skip / pageSize + 1;
        request.Registros = pageSize;
        request.Busqueda = Request.Form["search[value]"].FirstOrDefault();
        request.ColumnaOrdenamiento = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
        request.Ordenamiento = Request.Form["order[0][dir]"].FirstOrDefault();

        var gridData = new ResponseGrid<ClienteResponseGrid>();
        List<ClienteResponse> ListPF = new List<ClienteResponse>();

        var filtronombreCliente = filters.FirstOrDefault(x => x.Key == "nombreCliente");

        if(filtronombreCliente.Value != null)
        {
            string nombre = filtronombreCliente.Value;
            var listPF2 = await _clientesApiClient.GetDetallesClientesByNombresAsync(nombre);

            foreach (var cliente in listPF2)
            {
                ClienteResponse clienteResponse = new ClienteResponse
                {
                    nombreCompleto = cliente.nombre +" "+ cliente.apellido_paterno + " " +cliente.apellido_materno,
                    sexo = cliente.sexo,
                    CURP = cliente.curp,
                    email = cliente.email,
                    estatus = cliente.active ?? 0,
                    id = cliente.id_cliente,
                    membresia = cliente.membresia,
                    organizacion = cliente.organizacion,
                    telefono = cliente.telefono,
                    estatusweb = cliente.estatusWeb
                };

                ListPF.Add(clienteResponse);
            }
        }
        else
        {
            ListPF = await _clientesApiClient.GetClientesAsync(1, 200);

        }


        //Entorno local de pruebas
        //ListPF = GetList();

        var List = new List<ClienteResponseGrid>();
        foreach (var row in ListPF)
        {
            
            List.Add(new ClienteResponseGrid
            {
                id = row.id,
                nombreCompleto = (row.nombreCompleto + "|" + row.id.ToString()).ToUpper(),
                telefono = row.telefono,
                email = row.email,
                CURP = row.CURP,
                organizacion = row.organizacion,
                membresia = row.membresia,
                sexo = row.sexo,
                estatus = row.estatus,
                estatusweb = row.estatusweb,
                Acciones = await this.RenderViewToStringAsync("~/Views/Clientes/_Acciones.cshtml", row)
            });
        }
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

        //Aplicacion de Filtros temporal, 
        var filtroTelefono = filters.FirstOrDefault(x => x.Key == "telefono");
        var filtroCorreoElectronico = filters.FirstOrDefault(x => x.Key == "correoElectronico");
        var filtroCurp = filters.FirstOrDefault(x => x.Key == "curp");
        var filtroOrganizacion = filters.FirstOrDefault(x => x.Key == "organizacion");

        if (filtronombreCliente.Value != null)
        {
            List = List.Where(x => x.nombreCompleto.Contains(Convert.ToString(filtronombreCliente.Value.ToUpper()))).ToList();
        }

        if (filtroTelefono.Value != null)
        {
            List = List.Where(x => x.telefono == Convert.ToString(filtroTelefono.Value)).ToList();
        }

        if (filtroCorreoElectronico.Value != null)
        {
            List = List.Where(x => x.email == Convert.ToString(filtroCorreoElectronico.Value)).ToList();
        }

        if (filtroCurp.Value != null)
        {
            List = List.Where(x => x.CURP == Convert.ToString(filtroCurp.Value)).ToList();
        }

        if (filtroOrganizacion.Value != null)
        {
            List = List.Where(x => x.organizacion == Convert.ToString(filtroOrganizacion.Value)).ToList();
        }

        gridData.Data = List;
        gridData.RecordsTotal = List.Count;
        gridData.Data = gridData.Data.Skip(skip).Take(pageSize).ToList();
        filterRecord = string.IsNullOrEmpty(request.Busqueda) ? gridData.RecordsTotal ?? 0 : gridData.Data.Count;
        gridData.RecordsFiltered = filterRecord;
        gridData.Draw = draw;

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

    #region Detalles Cliente
    [Authorize]
    [Route("Clientes/Detalle/DatosGenerales")]
    public async Task<IActionResult> DatosGenerales(int id)
    {
        var loginResponse = _userContextService.GetLoginResponse();
        var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Clientes" && modulo.Ver == 0);
        if (validacion == true)
        {
            ClienteDetailsResponse user = await _clientesApiClient.GetDetallesCliente(id);

            if (user.value == null)
            {
                return RedirectToAction("Index");
            }

            ClientesDetallesViewModel clientesDetallesViewModel = new ClientesDetallesViewModel
            {
                IdCliente = user.value.IdCliente,
                Nombre = user.value.Nombre + " " + user.value.ApellidoPaterno + " " + user.value.ApellidoMaterno,
                Telefono = user.value.Telefono,
                Email = user.value.Email,
                CURP = user.value.CURP,
                Organizacion = user.value.Organizacion,
                Sexo = user.value.Sexo,
                RFC = user.value.RFC,
                INE = user.value.INE,
                FechaNacimiento = user.value.FechaNacimiento,
                Observaciones = user.value.Observaciones,
                PaisNacimiento = user.value.PaisNacimiento,
                Nacionalidad = user.value.Nacionalidad,
                Fiel = user.value.Fiel,
                SalarioNetoMensual =user.value.SalarioMensual,
                MontoMaximo = user.value.MontoMaximo,
                Calle = user.value.Calle,
                NoInt = user.value.NoInt,
                CalleSecundaria = user.value.CalleSecundaria,
                CalleSecundaria2 = user.value.CalleSecundaria2,
                Colonia = user.value.Colonia,
                CodigoPostal = user.value.CodigoPostal,
                Puesto = user.value.Puesto,
                AntiguedadLaboral = user.value.AntiguedadLaboral,
                Estado = user.value.Estado,
                Rol = user.value.Rol,
                Municipio = user.value.Municipio,
                NSS = user.value.NSS,
                Membresia = user.value.Membresia,
                TipoTrabajador = user.value.TipoTrabajador,
                EntidadNacimiento = user.value.EntidadNacimiento,
                CalleNumero = user.value.CalleNumero
            };

            ClientesHeaderViewModel header = new ClientesHeaderViewModel
            {
                Id = user.value.IdCliente,
                NombreCompleto = user.value.Nombre + " " + user.value.ApellidoPaterno + " " + user.value.ApellidoMaterno,
                Telefono = user.value.Telefono,
                Correo = user.value.Email,
                Curp = user.value.CURP,
                Organizacion = user.value.Organizacion,
                Rfc = user.value.RFC,
                Sexo = user.value.Sexo
            };
            ViewBag.Info = header;
            ViewBag.UrlView = "DatosGenerales";

            var validacionEdicion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Clientes" && modulo.Editar == 0);
            if (validacionEdicion == true)
            {
                ViewBag.ValEdicion = true;
            } else {
                ViewBag.ValEdicion = false;
            }

            return View("~/Views/Clientes/Detalles/DatosGenerales/DetalleCliente.cshtml", clientesDetallesViewModel);
        }
        return NotFound();
    }

    [Authorize]
    [Route("Clientes/Detalle/Cuentas")]
    public async Task<IActionResult> Cuentas(int id)
    {
        var loginResponse = _userContextService.GetLoginResponse();
        var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Clientes" && modulo.Ver == 0);
        if (validacion == true)
        {
            ClienteDetailsResponse user = await _clientesApiClient.GetDetallesCliente(id);
            var listaRoles = await _catalogosApiClient.GetRolClienteAsync();
            var listaEmpresa = await _catalogosApiClient.GetEmpresasAsync();

            if (user.value == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.UrlView = "Cuentas";
            ClientesHeaderViewModel header = new ClientesHeaderViewModel
            {
                Id = user.value.IdCliente,
                NombreCompleto = user.value.Nombre + " " + user.value.ApellidoPaterno + " " + user.value.ApellidoMaterno,
                Telefono = user.value.Telefono,
                Correo = user.value.Email,
                Curp = user.value.CURP,
                Organizacion = user.value.Organizacion,
                Rfc = user.value.RFC,
                Sexo = user.value.Sexo
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
            return View("~/Views/Clientes/Detalles/Cuentas/DetalleCuentas.cshtml");
        }

        return NotFound();
    }

    [Authorize]
    [HttpPost]
    public async Task<JsonResult> ConsultaCuentas(string id)
    {
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
            List.Add(new CuentasResponseGrid
            {
                idCuenta = row.idCuenta,
                nombrePersona = row.nombrePersona,
                noCuenta = row.noCuenta + "|" + row.idCuenta.ToString() + "|" + id,
                saldo = row.saldo,
                estatus = row.estatus,
                tipoPersona = row.tipoPersona,
                alias = row.alias,
                fechaActualizacion = row.fechaActualizacion,
                fechaAlta = row.fechaAlta,
                email = row.email,
                telefono = row.telefono,
                fechaActualizacionformat = row.fechaActualizacion.ToString(),
                fechaAltaFormat = row.fechaAlta.ToString(),
                Acciones = await this.RenderViewToStringAsync("~/Views/Clientes/Detalles/Cuentas/_Acciones.cshtml", row)
            });
        }
        //if (!string.IsNullOrEmpty(request.Busqueda))
        //{
        //    List = List.Where(
        //        x =>
        //        x.NoCuenta.ToLower().Contains(request.Busqueda.ToLower()) ||
        //        x.Saldo.ToLower().Contains(request.Busqueda.ToLower()) ||
        //        x.Alias.ToLower().Contains(request.Busqueda.ToLower()) ||
        //        x.NombrePersona.ToLower().Contains(request.Busqueda.ToLower()) ||
        //        x.Tipo.ToLower().Contains(request.Busqueda.ToLower())
        //        ).ToList();
        //}
        //if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
        //{
        //    var quer = List.AsQueryable();
        //    List = quer.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento).ToList();

        //}


        //List.Add(new EstadisiticaUsoFormaValoradaResponse() { }, new EstadisiticaUsoFormaValoradaResponse() { });
        gridData.Data = List;
        gridData.RecordsTotal = List.Count;
        gridData.Data = gridData.Data.Skip(skip).Take(pageSize).ToList();
        filterRecord = string.IsNullOrEmpty(request.Busqueda) ? gridData.RecordsTotal ?? 0 : gridData.Data.Count;
        gridData.RecordsFiltered = filterRecord;
        gridData.Draw = draw;

        //var returnObj = new
        //{
        //    draw,
        //    recordsTotal = totalRecord,
        //    recordsFiltered = filterRecord,
        //    data = List
        //};

        return Json(gridData);
    }

    [Authorize]
    [Route("Clientes/Detalle/Movimientos")]
    public async Task<IActionResult> Transacciones(int id, int cliente, string noCuenta)
    {
        var loginResponse = _userContextService.GetLoginResponse();
        var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Clientes" && modulo.Ver == 0);
        if (validacion == true)
        {
            ClienteDetailsResponse user = await _clientesApiClient.GetDetallesCliente(cliente);

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
            
            ViewBag.NumCuenta = noCuenta;

            if ((id == 0) && (noCuenta == null)) {
                ViewBag.TipoConsulta = "IsGeneral";
                ViewBag.AccountID = cliente;
            } else {
                ViewBag.TipoConsulta = "IsEspecific";
                ViewBag.AccountID = id;
            }

            RegistrarTransaccionRequest renderInfo = new RegistrarTransaccionRequest
            {
                NombreOrdenante = header.NombreCompleto,
                ClaveRastreo = string.Format("AQPAY1000000{0}", DateTime.Now.ToString("yyyymmddhhmmss"))
            };
            ViewBag.DatosRef = renderInfo;
            return View("~/Views/Clientes/Detalles/Transacciones/DetalleMovimientos.cshtml");
        }

        return RedirectToAction("Index", "Transacciones");
    }

    [Authorize]
    [Route("Clientes/Detalle/Tarjetas")]
    public async Task<IActionResult> Tarjetas(int id)
    {
        var loginResponse = _userContextService.GetLoginResponse();
        var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Clientes" && modulo.Ver == 0);
        if (validacion == true)
        {
            ClienteDetailsResponse user = await _clientesApiClient.GetDetallesCliente(id);

            if (user.value == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.UrlView = "Tarjetas";
            ClientesHeaderViewModel header = new ClientesHeaderViewModel
            {
                Id = user.value.IdCliente,
                NombreCompleto = user.value.Nombre + " " + user.value.ApellidoPaterno + " " + user.value.ApellidoMaterno,
                Telefono = user.value.Telefono,
                Correo = user.value.Email,
                Curp = user.value.CURP,
                Organizacion = user.value.Organizacion,
                Rfc = user.value.RFC,
                Sexo = user.value.Sexo
            };

            RegistrarTarjetaRequest renderRef = new RegistrarTarjetaRequest
            {
                idCliente = id
            };

            ViewBag.DatosRef = renderRef;
            ViewBag.Info = header;
            ViewBag.Nombre = header.NombreCompleto;
            ViewBag.AccountID = id;

            return View("~/Views/Clientes/Detalles/Tarjetas/DetalleTarjetas.cshtml", loginResponse);
        }

        return NotFound();
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

    [Authorize]
    [Route("Clientes/Detalle/Documentos")]
    public async Task<IActionResult> DocumentosCliente(int id)
    {
        var loginResponse = _userContextService.GetLoginResponse();
        var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Clientes" && modulo.Ver == 0);
        if (validacion == true)
        {
            ClienteDetailsResponse user = await _clientesApiClient.GetDetallesCliente(id);
            var listaDocumentos = await _catalogosApiClient.GetTipoDocumentosAsync();

            if (user.value == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.UrlView = "Documentos";
            ClientesHeaderViewModel header = new ClientesHeaderViewModel
            {
                Id = user.value.IdCliente,
                NombreCompleto = user.value.Nombre + " " + user.value.ApellidoPaterno + " " + user.value.ApellidoMaterno,
                Telefono = user.value.Telefono,
                Correo = user.value.Email,
                Curp = user.value.CURP,
                Organizacion = user.value.Organizacion,
                Rfc = user.value.RFC,
                Sexo = user.value.Sexo
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

            return View("~/Views/Clientes/Detalles/Documentos/DetalleDocumentos.cshtml", loginResponse);
        }

        return NotFound();
    }

    [Authorize]
    [Route("Clientes/Detalle/Documentos/CargarDocumentoCliente")]
    [HttpPost]
    public async Task<ActionResult> CargarDocumentoCliente(DocumentosClienteRegistro model, IFormFile documento)
    {
        var archivo = Request.Form["dropzonejs1"];
        return Json(model);
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
                NombreEquipo = Environment.MachineName,
                Accion = "Asignar Cuenta",
                Ip = await PublicIpHelper.GetPublicIp() ?? "0.0.0.0",
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

        return Json(model);
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
                NombreEquipo = Environment.MachineName,
                Accion = "Desasignar Cuenta",
                Ip = await PublicIpHelper.GetPublicIp() ?? "0.0.0.0",
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
            ViewBag.TituloForm = "Registrar Cliente";
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
            try
            {
                var response = await _clientesApiClient.GetRegistroCliente(model.ClientesDetalles);

                string detalle = response.Detalle;
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
                    Respuesta = response.Error.ToString(),
                    Error = response.Error ? JsonConvert.SerializeObject(response.Detalle) : string.Empty,
                    IdRegistro = idRegistro
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
    #endregion

    [Authorize]
    [HttpPost]
    public async Task<ActionResult> BuscarClientes(string nombreCliente)
    {
        var listaClientes = await _clientesApiClient.GetClientesbyNombreAsync(nombreCliente);
        return Json(listaClientes);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult> BuscarCoincidenciaNombre(string nombreCompleto)
    {
        var listaClientes = await _clientesApiClient.GetDetallesClientesByNombresAsync(nombreCompleto);
        return Json(listaClientes);
    }

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
                    Accion = "Insertar",
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
   
    #endregion
    //[Authorize]
    //public async Task<IActionResult> GestionarDocumentos(string AccountID = "")
    //{
    //    var loginResponse = _userContextService.GetLoginResponse();
    //    if (loginResponse?.AccionesPorModulo.Any(modulo => modulo.Modulo == "Clientes" && modulo.Acciones.Contains("Editar")) == true)
    //    {
    //        if (AccountID == "")
    //        {
    //            return RedirectToAction("Index");
    //        }

    //public async Task<IActionResult> GestionarDocumentos(string AccountID = "")
    //{
    //    var loginResponse = _userContextService.GetLoginResponse();
    //    var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Clientes" && modulo.Editar == 0);
    //    {
    //        if (AccountID == "")
    //        {
    //            return RedirectToAction("Index");
    //        }

    //        var GetDatosCliente = await _clientesApiClient.GetClienteAsync(Convert.ToInt32(AccountID));

    //        ViewBag.AccountID = AccountID;

    //        if (GetDatosCliente.NombreCompleto == null)
    //        {
    //            ViewBag.Nombre = "S/N";
    //        }
    //        else
    //        {
    //            ViewBag.Nombre = GetDatosCliente.NombreCompleto;
    //        }

            //string nombreCompleto = GetDatosCliente.Nombre + " " + GetDatosCliente.ApellidoPaterno ?? "" + " " + GetDatosCliente.ApellidoMaterno ?? "";

            //if (nombreCompleto == null)
            //{
            //    ViewBag.Nombre = "S/N";
            //}
            //else
            //{
            //    ViewBag.Nombre = nombreCompleto;
            //}

    //    return NotFound();
    //}

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
                    Colonia= cliente.value.Colonia,
                    Observaciones = cliente.value.Observaciones,
                    RFC = cliente.value.RFC,
                    Fiel = cliente.value.Fiel,
                    PaisNacimiento = cliente.value.PaisNacimiento,
                    IngresoMensual = cliente.value.SalarioMensual.ToString(),
                    ApoderadoLegal = cliente.value.ApoderadoLegal,
                    NoInterior = cliente.value.NoInt,
                    Puesto = cliente.value.Puesto,
                    FechaNacimiento = cliente.value.FechaNacimiento != null
? DateTime.ParseExact(cliente.value.FechaNacimiento, "dd/MM/yyyy", CultureInfo.InvariantCulture)
    : null,
                    DelegacionMunicipio = cliente.value.Municipio,
                    TelefonoTipo = cliente.value.TelefonoRecado,
                    IdNacionalidad = cliente.value.IdNacionalidad,
                    IdOcupacion = cliente.value.IdOcupacion,
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

    //[Authorize]
    //[HttpPost]
    //public async Task<IActionResult> ModificarCliente(ClientesRegistroViewModel model)
    //{
    //    var loginResponse = _userContextService.GetLoginResponse();
    //    if (loginResponse?.AccionesPorModulo.Any(modulo => modulo.Modulo == "Clientes" && modulo.Acciones.Contains("Insertar")) == true)
    //    {
    //        RegistrarModificarClienteResponse response = new RegistrarModificarClienteResponse();

    //        try
    //        {
    //            response = await _clientesApiClient.GetModificaCliente(model.ClientesDetalles);

    //            if (response.codigo == "200")
    //            {
    //                return RedirectToAction("Index");
    //            }
    //            else
    //            {
    //                return RedirectToAction("Registro");
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            return View();
    //        }
    //    }

    //    return NotFound();
    //}

    [Authorize]
    [HttpPost]
    public async Task<JsonResult> ConsultarSubCuentas()
    {
        var ListPF = await _clientesApiClient.GetClientesAsync(1,200);
        //var ListPF = GetList();
        return Json(ListPF);
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
            foreach (var row in ListPF.value)
            {
                List.Add(new DocumentosClienteResponseGrid
                {
                    idCliente = row.idCliente,
                    idDocApodLeg = row.idDocApodLeg,
                    tipoDocumento = row.tipoDocumento,
                    documento = row.documento,
                    numeroIdentificacion = row.numeroIdentificacion,
                    nombreDocumento = row.nombreDocumento,
                    Acciones = await this.RenderViewToStringAsync("~/Views/Clientes/_AccionesDocumentos.cshtml", row)
                });
            }

            gridData.Data = List;
            gridData.RecordsTotal = List.Count;
            gridData.Data = gridData.Data.Skip(skip).Take(pageSize).ToList();
            filterRecord = string.IsNullOrEmpty(request.Busqueda) ? gridData.RecordsTotal ?? 0 : gridData.Data.Count;
            gridData.RecordsFiltered = filterRecord;
            gridData.Draw = draw;

        } catch (Exception ex) {
            return Json(gridData);
        }

        return Json(gridData);
    }

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

    private string generarNombreArchivo(int i)
    {
        Random rnd = new Random((int)DateTime.Now.Ticks);

        i = i - 1;

        String[] nombres = { "INE", "Comprobante de Domicilio", "RFC", "CURP", "CARNET" };
        var genNombre = nombres[i];

        String gen = genNombre;

        return gen;
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

    public List<ClienteResponse> GetList()
    {


        var List = new List<ClienteResponse>();
        Random rnd = new Random();
        const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        String[] characters2 = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };

        try
        {
            for (int i = 1; i <= 10; i++)
            {
                var gen = generarnombre(i);

                var pf = new ClienteResponse();
                pf.id = i;
                pf.nombreCompleto = gen.NombreCompleto;
                pf.telefono = GenNumeroTelefono(10);
                pf.email = GenerarEmail();
                pf.CURP = "DOOM982834HMCRNS07";
                pf.organizacion = "Doom Organizacion";
                pf.membresia = "Negocios";
                pf.sexo = "Masculino";
                pf.estatus = 1;

                List.Add(pf);
            }
        }
        catch (Exception e)
        {
            List = new List<ClienteResponse>();
        }

        return List;
    }

    public async Task<List<DocumentosClienteResponse>> GetListArchivos(int idUser)
    {
        var List = new List<DocumentosClienteResponse>();

        try
        {
            for (int i = 1; i <= 4; i++)
            {
                var gen = generarNombreArchivo(i);

                var pf = new DocumentosClienteResponse();
                pf.idCliente = i;
                pf.idDocApodLeg = 100 + i;
                pf.tipoDocumento = i;
                pf.documento = GenNumeroTelefono(5);
                pf.numeroIdentificacion = GenNumeroTelefono(20);
                pf.nombreDocumento = generarNombreArchivo(i);

                List.Add(pf);
            }
        }
        catch (Exception e)
        {
            List = new List<DocumentosClienteResponse>();
        }

        return List;
    }

    public async Task<List<UserPermissionResponse>> GetListUser()
    {
        var List = new List<UserPermissionResponse>();
        Random rnd = new Random();
        try
        {
            for (int i = 1; i <= 5; i++)
            {
                var gen = generarnombre(i);

                var pf = new UserPermissionResponse();
                pf.id = i;
                pf.nombreRol = gen.NombreCompleto;
                pf.listaGen = generarListaPermisos();
                List.Add(pf);
            }
        }
        catch (Exception e)
        {
            List = new List<UserPermissionResponse>();
        }

        return List;
    }

    private List<ListUserPermissionResponse> generarListaPermisos()
    {
        var res = new List<ListUserPermissionResponse>();
        String[] catalogoVista = { "Clientes", "Transacciones", "Tarjetas", "Cuentas"};

        for (int i = 0; i <= 3; i++)
        {
            var genPermisos = catalogoVista[i];

            var pf = new ListUserPermissionResponse();
            pf.IdPermiso = i;
            pf.vista = genPermisos;
            pf.Escritura = false;
            pf.Lectura = true;
            pf.Eliminar = false;
            pf.Actualizar = true;

            res.Add(pf);
        }

        return res;
    }
    #endregion
}
