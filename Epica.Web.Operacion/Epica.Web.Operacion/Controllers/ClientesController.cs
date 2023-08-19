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

namespace Epica.Web.Operacion.Controllers;

public class ClientesController : Controller
{
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
        UserContextService userContextService,
        ICatalogosApiClient catalogosApiClient
        ITarjetasApiClient tarjetasApiClient,
        UserContextService userContextService
        )
    {
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
        if (loginResponse?.AccionesPorModulo.Any(modulo => modulo.Modulo == "Clientes" && modulo.Acciones.Contains("Ver")) == true)
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

        ListPF = await _clientesApiClient.GetClientesAsync(1, 200);

        //Entorno local de pruebas
        //ListPF = GetList();

        var List = new List<ClienteResponseGrid>();
        foreach (var row in ListPF)
        {
            List.Add(new ClienteResponseGrid
            {
                id = row.id,
                nombreCompleto = row.nombreCompleto + "|" + row.id.ToString(),
                telefono = row.telefono,
                email = row.email,
                CURP = row.CURP,
                organizacion = row.organizacion,
                membresia = row.membresia,
                sexo = row.sexo,
                estatus = row.estatus,
                Acciones = await this.RenderViewToStringAsync("~/Views/Clientes/_Acciones.cshtml", row)
            });
        }
        if (!string.IsNullOrEmpty(request.Busqueda))
        {
            List = List.Where(x =>
            (x.id.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.nombreCompleto?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.telefono?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.email?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.CURP?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.organizacion?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.membresia?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.sexo?.ToLower() ?? "").Contains(request.Busqueda.ToLower())
            ).ToList();
        }

        //Aplicacion de Filtros temporal, 
        //var filtroid = filters.FirstOrDefault(x => x.Key == "Id");
        //var filtronombreCliente = filters.FirstOrDefault(x => x.Key == "NombreCliente");
        //var filtroNoCuenta = filters.FirstOrDefault(x => x.Key == "noCuenta");
        //var filtroEstatus = filters.FirstOrDefault(x => x.Key == "estatus");
        //var filtroSaldo = filters.FirstOrDefault(x => x.Key == "saldo");
        //var filtroTipo = filters.FirstOrDefault(x => x.Key == "tipo");

        //if (filtroid.Value != null)
        //{
        //    List = List.Where(x => x.Id == Convert.ToInt32(filtroid.Value)).ToList();
        //}

        //if (filtronombreCliente.Value != null)
        //{
        //    List = List.Where(x => x.cliente.Contains(Convert.ToString(filtronombreCliente.Value))).ToList();
        //}

        //if (filtroNoCuenta.Value != null)
        //{
        //    List = List.Where(x => x.noCuenta == Convert.ToString(filtroNoCuenta.Value)).ToList();
        //}

        //if (filtroEstatus.Value != null)
        //{
        //    List = List.Where(x => x.estatus == Convert.ToString(filtroEstatus.Value)).ToList();
        //}

        //if (filtroSaldo.Value != null)
        //{
        //    List = List.Where(x => x.saldo == Convert.ToString(filtroSaldo.Value)).ToList();
        //}

        //if (filtroTipo.Value != null)
        //{
        //    List = List.Where(x => x.tipo == Convert.ToString(filtroTipo.Value)).ToList();
        //}

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

        BloqueoWebResponse bloqueoWebResponse = new BloqueoWebResponse();
        try
        {
            if (Estatus == "True")
            {

                BloqueoWebClienteRequest bloqueoWebRequest = new BloqueoWebClienteRequest();
                bloqueoWebRequest.idCliente = id;
                bloqueoWebRequest.estatus = 1;

                bloqueoWebResponse = await _clientesApiClient.GetBloqueoWeb(bloqueoWebRequest);

            }
            else if (Estatus == "False")
            {

                BloqueoWebClienteRequest bloqueoWebRequest = new BloqueoWebClienteRequest();
                bloqueoWebRequest.idCliente = id;
                bloqueoWebRequest.estatus = 2;

                bloqueoWebResponse = await _clientesApiClient.GetBloqueoWeb(bloqueoWebRequest);

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

        BloqueoTotalResponse bloqueototalResponse = new BloqueoTotalResponse();
        try
        {
            if (Estatus == "True")
            {

                BloqueoTotalClienteRequest bloqueoTotalRequest = new BloqueoTotalClienteRequest();
                bloqueoTotalRequest.idCliente = id;
                bloqueoTotalRequest.estatus = 1;

                bloqueototalResponse = await _clientesApiClient.GetBloqueoTotal(bloqueoTotalRequest);

            }
            else if (Estatus == "False")
            {

                BloqueoTotalClienteRequest bloqueoTotalRequest = new BloqueoTotalClienteRequest();
                bloqueoTotalRequest.idCliente = id;
                bloqueoTotalRequest.estatus = 2;

                bloqueototalResponse = await _clientesApiClient.GetBloqueoTotal(bloqueoTotalRequest);

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
    #endregion

    #region Detalles Cliente
    [Authorize]
    [Route("Clientes/Detalle/DatosGenerales")]
    public async Task<IActionResult> DatosGenerales(int id)
    {
        var loginResponse = _userContextService.GetLoginResponse();
        if (loginResponse?.AccionesPorModulo.Any(modulo => modulo.Modulo == "Clientes" && modulo.Acciones.Contains("Ver")) == true)
        {
            ClienteDetailsResponse user = await _clientesApiClient.GetDetallesCliente(id);

            if (user.value == null)
            {
                return RedirectToAction("Index");
            }

            ClientesDetallesViewModel clientesDetallesViewModel = new ClientesDetallesViewModel
            {
                Id = user.value.IdCliente,
                Nombre = user.value.NombreCompleto,
                Telefono = user.value.Telefono,
                Email = user.value.Email,
                Curp = user.value.CURP,
                Empresa = user.value.Organizacion,
                Sexo = user.value.Sexo,
                Rfc = user.value.RFC,
                Ine = user.value.INE,
                FechaNacimiento = user.value.FechaNacimiento,
                Observaciones = user.value.Observaciones,
                PaisNacimiento = user.value.PaisNacimiento,
                Ocupacion = user.value.IdOcupacion.ToString(),
                Nacionalidad = user.value.Nacionalidad,
                Fiel = user.value.Fiel,
                Pais = user.value.PaisNacimiento,
                IngresoMensual = Convert.ToDecimal(user.value.SalarioNetoMensual),
                MontoMaximo = Convert.ToDecimal(user.value.SalarioNetoMensual),
                Calle = user.value.Calle,
                CalleNumero = user.value.NoIntExt,
                PrimeraCalle = user.value.CalleSecundaria,
                SegundaCalle = user.value.CalleSecundaria2,
                Colonia = user.value.Colonia,
                CodigoPostal = user.value.CodigoPostal,
                NoInterior = user.value.NoIntExt,
                Puesto = user.value.Puesto
                //Rol = user.value.rol,
                //    DelegacionMunicipio = user.value.del;
                //CiudadEstado = user.value.CiudadEstado
                //Empresa = Convert.ToInt32(user.value.em),
                //ApoderadoLegal = Convert.ToInt32(user.value.);
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
            return View("~/Views/Clientes/Detalles/DatosGenerales/DetalleCliente.cshtml", clientesDetallesViewModel);
        }
        return NotFound();
    }

    [Authorize]
    [Route("Clientes/Detalle/Cuentas")]
    public async Task<IActionResult> Cuentas(int id)
    {
        var loginResponse = _userContextService.GetLoginResponse();
        if (loginResponse?.AccionesPorModulo.Any(modulo => modulo.Modulo == "Clientes" && modulo.Acciones.Contains("Ver")) == true)
        {
            ClienteDetailsResponse user = await _clientesApiClient.GetDetallesCliente(id);

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
            ViewBag.Info = header;
            ViewBag.Nombre = header.NombreCompleto;
            ViewBag.AccountID = id;
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
                fechaActualizacionformat = row.fechaActualizacion.ToString(),
                fechaAltaFormat = row.fechaAlta.ToString()
                //Acciones = await this.RenderViewToStringAsync("~/Views/Cuenta/_Acciones.cshtml", row)
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
        if (loginResponse?.AccionesPorModulo.Any(modulo => modulo.Modulo == "Cuentas" && modulo.Acciones.Contains("Ver")) == true)
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
            ViewBag.AccountID = id;
            ViewBag.NumCuenta = noCuenta;

            RegistrarTransaccionRequest renderInfo = new RegistrarTransaccionRequest
            {
                NombreOrdenante = header.NombreCompleto,
                NoCuentaOrdenante = noCuenta,
                ClaveRastreo = string.Format("AQPAY1000000{0}", DateTime.Now.ToString("yyyymmddhhmmss"))
            };
            ViewBag.DatosRef = renderInfo;
            return View("~/Views/Clientes/Detalles/Transacciones/DetalleMovimientos.cshtml");
        }

        return NotFound();
    }

    [Authorize]
    [Route("Clientes/Detalle/Tarjetas")]
    public async Task<IActionResult> Tarjetas(int id)
    {
        var loginResponse = _userContextService.GetLoginResponse();
        if (loginResponse?.AccionesPorModulo.Any(modulo => modulo.Modulo == "Tarjetas" && modulo.Acciones.Contains("Ver")) == true)
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

            return View("~/Views/Clientes/Detalles/Tarjetas/DetalleTarjetas.cshtml");
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
    #endregion

    #region Registro Clientes
    [Authorize]
    public async Task<ActionResult> Registro()
    {
        var loginResponse = _userContextService.GetLoginResponse();
        if (loginResponse?.AccionesPorModulo.Any(modulo => modulo.Modulo == "Clientes" && modulo.Acciones.Contains("Insertar")) == true)
        {
            var listaEmpresas = await _catalogosApiClient.GetEmpresasAsync();
            var listaRoles = await _catalogosApiClient.GetRolClienteAsync();
            var listaOcupaciones = await _catalogosApiClient.GetOcupacionesAsync();
            var listaPaises = await _catalogosApiClient.GetPaisesAsync();
            var listaNacionalidades = await _catalogosApiClient.GetNacionalidadesAsync();

            ClientesRegistroViewModel clientesRegistroViewModel = new ClientesRegistroViewModel
            {
                ClientesDetalles = new RegistroModificacionClienteRequest(),
                ListaEmpresas = listaEmpresas,
                ListaRoles = listaRoles,
                ListaOcupaciones = listaOcupaciones,
                ListaPaises = listaPaises,
                ListaNacionalidades = listaNacionalidades
            };

            ViewData["IsEdit"] = false;
            ViewBag.TituloForm = "Crear nuevo cliente";
            return View(clientesRegistroViewModel);
        }

        return NotFound();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> RegistrarCliente(ClientesRegistroViewModel model)
    {
        var loginResponse = _userContextService.GetLoginResponse();
        if (loginResponse?.AccionesPorModulo.Any(modulo => modulo.Modulo == "Clientes" && modulo.Acciones.Contains("Insertar")) == true)
        {
            RegistrarModificarClienteResponse response = new RegistrarModificarClienteResponse();

            try
            {
                response = await _clientesApiClient.GetRegistroCliente(model.ClientesDetalles);

                if (response.codigo == "200")
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
    public async Task<JsonResult> ConsultaPermisos(List<RequestListFilters> filters)
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

        var gridData = new ResponseGrid<UserPermissionResponseGrid>();
        List<UserPermissionResponse> ListPF = new List<UserPermissionResponse>();

        //ListPF = await _usuariosApiClient.GetUsuariosAsync(1, 200);

        //Entorno local de pruebas
        ListPF = await GetListUser();

        var List = new List<UserPermissionResponseGrid>();
        foreach (var row in ListPF)
        {
            List.Add(new UserPermissionResponseGrid
            {
                id = row.id,
                nombreCompleto = row.nombreCompleto,
                listaGen = row.listaGen
            });
        }
        //if (!string.IsNullOrEmpty(request.Busqueda))
        //{
        //    List = List.Where(x =>
        //    (x.id.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
        //    (x.nombreCompleto?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
        //    (x.telefono?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
        //    (x.email?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
        //    (x.CURP?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
        //    (x.organizacion?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
        //    (x.membresia?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
        //    (x.sexo?.ToLower() ?? "").Contains(request.Busqueda.ToLower())
        //    ).ToList();
        //}

        //Aplicacion de Filtros temporal, 
        //var filtroid = filters.FirstOrDefault(x => x.Key == "Id");
        //var filtronombreCliente = filters.FirstOrDefault(x => x.Key == "NombreCliente");
        //var filtroNoCuenta = filters.FirstOrDefault(x => x.Key == "noCuenta");
        //var filtroEstatus = filters.FirstOrDefault(x => x.Key == "estatus");
        //var filtroSaldo = filters.FirstOrDefault(x => x.Key == "saldo");
        //var filtroTipo = filters.FirstOrDefault(x => x.Key == "tipo");

        //if (filtroid.Value != null)
        //{
        //    List = List.Where(x => x.Id == Convert.ToInt32(filtroid.Value)).ToList();
        //}

        //if (filtronombreCliente.Value != null)
        //{
        //    List = List.Where(x => x.cliente.Contains(Convert.ToString(filtronombreCliente.Value))).ToList();
        //}

        //if (filtroNoCuenta.Value != null)
        //{
        //    List = List.Where(x => x.noCuenta == Convert.ToString(filtroNoCuenta.Value)).ToList();
        //}

        //if (filtroEstatus.Value != null)
        //{
        //    List = List.Where(x => x.estatus == Convert.ToString(filtroEstatus.Value)).ToList();
        //}

        //if (filtroSaldo.Value != null)
        //{
        //    List = List.Where(x => x.saldo == Convert.ToString(filtroSaldo.Value)).ToList();
        //}

        //if (filtroTipo.Value != null)
        //{
        //    List = List.Where(x => x.tipo == Convert.ToString(filtroTipo.Value)).ToList();
        //}

        gridData.Data = List;
        gridData.RecordsTotal = List.Count;
        gridData.Data = gridData.Data.Skip(skip).Take(pageSize).ToList();
        filterRecord = string.IsNullOrEmpty(request.Busqueda) ? gridData.RecordsTotal ?? 0 : gridData.Data.Count;
        gridData.RecordsFiltered = filterRecord;
        gridData.Draw = draw;

        return Json(gridData);
    }

    [Authorize]
    #endregion

    public async Task<IActionResult> GestionarDocumentos(string AccountID = "")
    {
        var loginResponse = _userContextService.GetLoginResponse();
        if (loginResponse?.AccionesPorModulo.Any(modulo => modulo.Modulo == "Clientes" && modulo.Acciones.Contains("Editar")) == true)
        {
            if (AccountID == "")
            {
                return RedirectToAction("Index");
            }

            var GetDatosCliente = await _clientesApiClient.GetClienteAsync(Convert.ToInt32(AccountID));

            ViewBag.AccountID = AccountID;

            if (GetDatosCliente.NombreCompleto == null)
            {
                ViewBag.Nombre = "S/N";
            }
            else
            {
                ViewBag.Nombre = GetDatosCliente.NombreCompleto;
            }

            return View();
        }

        return NotFound();
    }

    [Authorize]
    public async Task<ActionResult> Modificar(int id)
    {
        var loginResponse = _userContextService.GetLoginResponse();
        if (loginResponse?.AccionesPorModulo.Any(modulo => modulo.Modulo == "Clientes" && modulo.Acciones.Contains("Editar")) == true)
        {
            try
            {
                var cliente = await _clientesApiClient.GetClienteAsync(id);
                RegistroModificacionClienteRequest clientesDetalles = new RegistroModificacionClienteRequest
                {
                    Nombre = cliente.Nombre,
                    Telefono = cliente.Telefono,
                    Email = cliente.Email,
                    Curp = cliente.CURP,
                    Sexo = cliente.Sexo,
                    EntreCalleSegunda = cliente.CalleSecundaria2,
                    EntreCallePrimera = cliente.CalleSecundaria,
                    INE = cliente.INE,
                    Calle = cliente.Calle,
                    ApellidoMaterno = cliente.ApellidoMaterno,
                    ApellidoPaterno = cliente.ApellidoPaterno,
                    CodigoPostal = cliente.CodigoPostal,
                    Colonia= cliente.Colonia,
                    Observaciones = cliente.Observaciones,
                    RFC = cliente.RFC,
                    Fiel = cliente.Fiel,
                    PaisNacimiento = cliente.PaisNacimiento,
                    IngresoMensual = cliente.SalarioNetoMensual.ToString(),
                    ApoderadoLegal = (cliente.AntiguedadLaboral?.ToLower() == "si") ? 1 : 0,
                    NoInterior = cliente.NoIntExt,
                    Puesto = cliente.Puesto,
                    FechaNacimiento =cliente.FechaNacimiento,
                    DelegacionMunicipio = cliente.Municipio,
                    TelefonoTipo = cliente.TelefonoRecado,
                    IdNacionalidad = cliente.IdNacionalida,
                    IdOcupacion = cliente.IdOcupacion,
                    CiudadEstado = cliente.Estado
                };

                if (clientesDetalles == null)
                {
                    return RedirectToAction("Error404", "Error");
                }

                ClientesRegistroViewModel clientesRegistroViewModel = new ClientesRegistroViewModel
                {
                    ClientesDetalles = clientesDetalles
                };
                return View("~/Views/Clientes/Registro.cshtml", clientesRegistroViewModel);
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Error");
            }
        }

        return NotFound();

    }

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

            List<DocumentosClienteResponse> ListPF = new List<DocumentosClienteResponse>();
            ListPF = await _clientesApiClient.GetDocumentosClienteAsync(Convert.ToInt32(idAccount));

            var List = new List<DocumentosClienteResponseGrid>();
            foreach (var row in ListPF)
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
                pf.nombreCompleto = gen.NombreCompleto;
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
            pf.id = i;
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
