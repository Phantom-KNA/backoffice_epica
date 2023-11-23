using Epica.Web.Operacion.Helpers;
using Epica.Web.Operacion.Models.Common;
using Epica.Web.Operacion.Models.Request;
using Epica.Web.Operacion.Services.Catalogos;
using Epica.Web.Operacion.Services.Log;
using Epica.Web.Operacion.Services.Usuarios;
using Epica.Web.Operacion.Utilities;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace Epica.Web.Operacion.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly UserContextService _userContextService;
        private readonly IUsuariosApiClient _usuariosApiClient;
        private readonly ICatalogosApiClient _catalogosApiClient;
        private readonly ILogsApiClient _logsApiClient;

        public UsuariosController(UserContextService userContextService,
            IUsuariosApiClient usuariosApiClient,
            ICatalogosApiClient catalogosApiClient,
            ILogsApiClient logsApiClient) 
        {
            _userContextService = userContextService;
            _usuariosApiClient = usuariosApiClient;
            _catalogosApiClient = catalogosApiClient;
            _logsApiClient = logsApiClient;
        }

        #region Consulta Usuarios

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var loginResponse = _userContextService.GetLoginResponse();
            var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Configuracion" && modulo.Ver == 0);
            if (validacion == true)
            {
                var listaRoles = await _catalogosApiClient.GetRolesUsuarioAsync();

                AsignarRolUsuario renderAsign = new AsignarRolUsuario
                {
                    ListaRoles = listaRoles
                };

                ViewBag.AsignarRol = renderAsign;

                return View(loginResponse);
            }

            return NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<JsonResult> Consulta(List<RequestListFilters> filters)
        {
            var loginResponse = _userContextService.GetLoginResponse();
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

            var gridData = new ResponseGrid<UsuariosResponseGrid>();
            List<UsuariosVinculadosResponse> ListPF = new List<UsuariosVinculadosResponse>();

            ListPF = await _usuariosApiClient.GetAsignarRolPermisosAsync();

            var List = new List<UsuariosResponseGrid>();
            foreach (var row in ListPF)
            {
                var validacionEdicion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Configuracion" && modulo.Eliminar == 0);
                if (validacionEdicion == true)
                {
                    row.validarpermiso = true;
                }
                else
                {
                    row.validarpermiso = false;
                }
                List.Add(new UsuariosResponseGrid
                {
                    IdUser = row.IdUser,
                    Username = row.Username,
                    DescripcionRol = row.DescripcionRol,
                    FechaAlta = row.FechaAlta,
                    FechaUltimoAcceso = row.FechaUltimoAcceso,
                    estatus = row.Estatus,
                    email = row.email,
                    Acciones = await this.RenderViewToStringAsync("~/Views/Usuarios/_Acciones.cshtml", row)
                });
            }
            if (!string.IsNullOrEmpty(request.Busqueda))
            {
                List = List.Where(x =>
                (x.Username?.ToUpper() ?? "").Contains(request.Busqueda.ToUpper()) ||
                (x.DescripcionRol?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
                (x.FechaAlta?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
                (x.email?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
                (x.FechaUltimoAcceso?.ToLower() ?? "").Contains(request.Busqueda.ToLower())
                ).ToList();
            }

            //Aplicacion de Filtros temporal, 
            var filtroUser = filters.FirstOrDefault(x => x.Key == "username");
            var filtroRol = filters.FirstOrDefault(x => x.Key == "rol");
            var filtroFechaAlta = filters.FirstOrDefault(x => x.Key == "fechaAlta");
            var filtroFechaUltimoAcceso = filters.FirstOrDefault(x => x.Key == "fechaUltimoAcceso");
            var filtrocorreo = filters.FirstOrDefault(x => x.Key == "filtro_email");

            if (filtroUser?.Value != null)
            {
                List = List.Where(x => x.Username.ToUpper().Contains(Convert.ToString(filtroUser.Value.ToUpper()))).ToList();
            }

            if (filtroRol?.Value != null)
            {
                List = List.Where(x => x.DescripcionRol.ToUpper().Equals((Convert.ToString(filtroRol.Value.ToUpper())))).ToList();
            }

            if (filtroFechaAlta?.Value != null)
            {
                List = List.Where(x => x.FechaAlta.Contains(Convert.ToString(filtroFechaAlta.Value))).ToList();
            }

            if (filtroFechaUltimoAcceso?.Value != null)
            {
                List = List.Where(x => x.FechaUltimoAcceso.Contains(Convert.ToString(filtroFechaUltimoAcceso.Value))).ToList();
            }

            if (filtrocorreo?.Value != null)
            {
                List = List.Where(x => x.FechaUltimoAcceso.Contains(Convert.ToString(filtroFechaUltimoAcceso.Value))).ToList();
            }

            gridData.Data = List.Skip(skip).Take(pageSize).ToList(); 
            gridData.RecordsTotal = List.Count;
            filterRecord = string.IsNullOrEmpty(request.Busqueda) ? gridData.RecordsTotal ?? 0 : gridData.Data.Count;
            gridData.RecordsFiltered = filterRecord;
            gridData.Draw = draw;

            return Json(gridData);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> BuscarUsuario(string usuario)
        {
            var listaClientes = await _usuariosApiClient.GetUsuarioPorNombreAsync(usuario);

            listaClientes = listaClientes.Distinct().ToList();

            return Json(listaClientes);
        }

        [Authorize]
        [HttpPost]
        public async Task<JsonResult> RegistrarAsignacionUsuario(AsignarRolUsuario model)
        {
            var loginResponse = _userContextService.GetLoginResponse();
            MensajeResponse response = new MensajeResponse();

            try
            {

                response = await _usuariosApiClient.GetRegistroAsignacionUsuarioRol(model.idRol,model.idUsuario);

                LogRequest logRequest = new LogRequest
                {
                    IdUser = loginResponse.IdUsuario.ToString(),
                    Modulo = "Configuracion",
                    Fecha = HoraHelper.GetHoraCiudadMexico(),
                    NombreEquipo = loginResponse.NombreDispositivo,
                    Accion = "Asignar Rol Usuario",
                    Ip = loginResponse.DireccionIp,
                    Envio = JsonConvert.SerializeObject(model),
                    Respuesta = response.Error.ToString(),
                    Error = response.Error ? JsonConvert.SerializeObject(response.Detalle) : string.Empty,
                    IdRegistro = model.idUsuario
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
        [HttpPost]
        public async Task<JsonResult> DesasignarUsuario(int idUsuario)
        {
            var loginResponse = _userContextService.GetLoginResponse();
            MensajeResponse response = new MensajeResponse();

            try
            {

                response = await _usuariosApiClient.GetDesasignacionUsuarioRol(idUsuario);

                LogRequest logRequest = new LogRequest
                {
                    IdUser = loginResponse.IdUsuario.ToString(),
                    Modulo = "Configuracion",
                    Fecha = HoraHelper.GetHoraCiudadMexico(),
                    NombreEquipo = loginResponse.NombreDispositivo,
                    Accion = "Desasignar Rol Usuario",
                    Ip = loginResponse.DireccionIp,
                    Envio = JsonConvert.SerializeObject(idUsuario),
                    Respuesta = response.Error.ToString(),
                    Error = response.Error ? JsonConvert.SerializeObject(response.Detalle) : string.Empty,
                    IdRegistro = idUsuario
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

        [Authorize]
        public async Task<ActionResult> GestionarPermisos()
        {
            var loginResponse = _userContextService.GetLoginResponse();
            var validacion = loginResponse?.AccionesPorModulo!.Any(modulo => modulo.ModuloAcceso == "Configuracion" && modulo.Ver == 0);
            if (validacion == true)
                return View();
            return NotFound();
        }

        public async Task<IActionResult> ObtenerUsuariosConPermisos()
        {
            var listaUsuarios = await _usuariosApiClient.GetUsuariosRolesAsync();

            var data = listaUsuarios.Select(u => new
            {
                IdUsuario = u.IdUsuario,
                Usuario = u.Usuario,
                Email = u.Email,
                Rol = u.Rol,
                Activo = u.Activo,
                IsAuthenticated = u.IsAuthenticated
            }).ToList();

            return Json(new { data });
        }

        [Authorize]
        [HttpPost]
        public async Task<JsonResult> ConsultaPermisos()
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
            List<UserRolPermisosResponse> ListPF = new List<UserRolPermisosResponse>();

            ListPF = await _usuariosApiClient.GetUsuariosRolesVistaAsync();
            var List = new List<UserPermissionResponseGrid>();
            foreach (var row in ListPF)
            {
                List<ListUserPermissionResponse> listPermisos = new List<ListUserPermissionResponse>();

                if (row.AccionesPorModulo!.Count == 0) {
                    listPermisos = mapeoRolesVistaVacio(row.Id);
                } else if (row.AccionesPorModulo.Count == 7) {
                    listPermisos = mapeoRolesVistaParcial(row.AccionesPorModulo, row.Id);
                } else {
                    listPermisos = mapeoRolesVistaParcial(row.AccionesPorModulo, row.Id);
                }

                List.Add(new UserPermissionResponseGrid
                {
                    id = row.Id,
                    nombreRol = row.NombreRol!,
                    listaGen = listPermisos
                }); 
            }

            if (!string.IsNullOrEmpty(request.Busqueda))
            {
                List = List.Where(x =>
                (x.id.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
                (x.nombreRol.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower())
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
        [HttpPost]
        public async Task<JsonResult> ActualizarPermiso(UserPermisoPostRequest model)
        {

            MensajeResponse respuesta = new MensajeResponse();

            try
            {
                UserPermisoRequest registrarCambio = new UserPermisoRequest();

                List<UserPermisosResponse> generarListaPermisos = new List<UserPermisosResponse>();

                generarListaPermisos = await _usuariosApiClient.GetRolPermisosEspecificoAsync(model.rol, model.vista);

                if (generarListaPermisos.Count == 0) {

                    registrarCambio.IdPermiso = model.permiso;
                    registrarCambio.IdRol = model.rol;
                    registrarCambio.ModuloAcceso = model.vista;
                    registrarCambio.Insertar = 1;
                    registrarCambio.Ver = 1;
                    registrarCambio.Eliminar = 1;
                    registrarCambio.Editar = 1;

                    if (model.accion == "Escritura")
                    {
                        registrarCambio.Insertar = model.activo == true ? 0 : 1;

                    }
                    else if (model.accion == "Lectura")
                    {
                        registrarCambio.Ver = model.activo == true ? 0 : 1;

                    }
                    else if (model.accion == "Eliminar")
                    {
                        registrarCambio.Eliminar = model.activo == true ? 0 : 1;

                    }
                    else if (model.accion == "Actualizar")
                    {
                        registrarCambio.Editar = model.activo == true ? 0 : 1;
                    }

                } else {

                    registrarCambio.IdPermiso = model.permiso;
                    registrarCambio.IdRol = model.rol;
                    registrarCambio.ModuloAcceso = model.vista;
                    registrarCambio.Insertar = generarListaPermisos.First().Insertar == 0 ? 0 : 1;
                    registrarCambio.Ver = generarListaPermisos.First().Ver == 0 ? 0 : 1;
                    registrarCambio.Eliminar = generarListaPermisos.First().Eliminar == 0 ? 0 : 1;
                    registrarCambio.Editar = generarListaPermisos.First().Editar == 0 ? 0 : 1;

                    if (model.accion == "Escritura")
                    {
                        registrarCambio.Insertar = model.activo == true ? 0 : 1;

                    }
                    else if (model.accion == "Lectura")
                    {
                        registrarCambio.Ver = model.activo == true ? 0 : 1;

                    }
                    else if (model.accion == "Eliminar")
                    {
                        registrarCambio.Eliminar = model.activo == true ? 0 : 1;

                    }
                    else if (model.accion == "Actualizar")
                    {
                        registrarCambio.Editar = model.activo == true ? 0 : 1;
                    }

                }

                respuesta = await _usuariosApiClient.GetAsignarRolPermisos(registrarCambio);

            } catch (Exception ex) {
                return Json(respuesta);
            }

            return Json(respuesta);
        }

        [Authorize]
        [HttpPost]
        public async Task<JsonResult> CrearNuevoRol(string descripcion)
        {
            var loginResponse = _userContextService.GetLoginResponse();
            MensajeResponse response = new MensajeResponse();

            try
            {

                int Registro = 0;
                response = await _usuariosApiClient.GetCrearRol(descripcion);

                if (response.Error != true) {
                    if (response.Detalle != null) {

                        var value = response.Detalle.Substring(7);
                        Registro = Convert.ToInt32(value);
                    }
                }

                LogRequest logRequest = new LogRequest
                {
                    IdUser = loginResponse.IdUsuario.ToString(),
                    Modulo = "Configuracion",
                    Fecha = HoraHelper.GetHoraCiudadMexico(),
                    NombreEquipo = loginResponse.NombreDispositivo,
                    Accion = "Crear Rol",
                    Ip = loginResponse.DireccionIp,
                    Envio = JsonConvert.SerializeObject(descripcion),
                    Respuesta = response.Error.ToString(),
                    Error = response.Error ? JsonConvert.SerializeObject(response.Detalle) : string.Empty,
                    IdRegistro = Registro
                };

                await _logsApiClient.InsertarLogAsync(logRequest);
            }
            catch (Exception ex)
            {
                response.Detalle = ex.Message;
            }

            return Json(response);
        }


        public async Task<List<UserPermissionResponse>> GetListUser()
        {
            var List = new List<UserPermissionResponse>();
            Random rnd = new Random();
            try
            {
                for (int i = 1; i <= 5; i++)
                {
                    //var gen = generarnombre(i);

                    var pf = new UserPermissionResponse();
                    pf.id = i;
                    pf.nombreRol = "admin";
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
            String[] catalogoVista = { "Clientes", "Transacciones", "Tarjetas", "Cuentas" };

            for (int i = 0; i <= 4; i++)
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

        private List<ListUserPermissionResponse> mapeoRolesVistaParcial(List<UserPermisosResponse> listaPermisos, int id)
        {
            var res = new List<ListUserPermissionResponse>();
            String[] catalogoVista = { "Clientes", "Cuentas", "Tarjetas", "Transacciones", "Configuracion", "Autorizador", "Reintentador" };

            for (int i = 0; i <= 6; i++)
            {
                var genPermisos = catalogoVista[i];

                var validalist = (from n in listaPermisos where n.ModuloAcceso == genPermisos select n).ToList();

                if (validalist.Count == 0) {

                    var pf = new ListUserPermissionResponse();
                    pf.IdPermiso = i;
                    pf.IdRol = id;
                    pf.vista = genPermisos;
                    pf.Escritura = false;
                    pf.Lectura = false;
                    pf.Eliminar = false;
                    pf.Actualizar = false;

                    res.Add(pf);

                } else {

                    foreach (var row in validalist)
                    {

                        var pf = new ListUserPermissionResponse();
                        pf.IdPermiso = row.IdPermiso;
                        pf.IdRol = row.IdRol;
                        pf.vista = row.ModuloAcceso!;
                        pf.Escritura = row.Insertar == 0 ? true : false;
                        pf.Lectura = row.Ver == 0 ? true : false;
                        pf.Eliminar = row.Eliminar == 0 ? true : false;
                        pf.Actualizar = row.Editar == 0 ? true : false;

                        res.Add(pf);
                    }

                }

            }

            return res;
        }

        private List<ListUserPermissionResponse> mapeoRolesVista(List<UserPermisosResponse> listaPermisos)
        {
            var res = new List<ListUserPermissionResponse>();

            foreach (var row in listaPermisos)
            {

                var pf = new ListUserPermissionResponse();
                pf.IdPermiso = row.IdPermiso;
                pf.IdRol = row.IdRol;
                pf.vista = row.ModuloAcceso!;
                pf.Escritura = row.Insertar == 0 ? true : false;
                pf.Lectura = row.Ver == 0 ? true : false;
                pf.Eliminar = row.Eliminar == 0 ? true : false;
                pf.Actualizar = row.Editar == 0 ? true : false;

                res.Add(pf);
            }

            return res;
        }

        private List<ListUserPermissionResponse> mapeoRolesVistaVacio(int id)
        {
            var res = new List<ListUserPermissionResponse>();
            String[] catalogoVista = { "Clientes", "Cuentas", "Tarjetas", "Transacciones", "Configuracion", "Autorizador", "Reintentador" };

            for (int i = 0; i <= 6; i++)
            {
                var genPermisos = catalogoVista[i];

                var pf = new ListUserPermissionResponse();
                pf.IdPermiso = i;
                pf.IdRol = id;
                pf.vista = genPermisos;
                pf.Escritura = false;
                pf.Lectura = false;
                pf.Eliminar = false;
                pf.Actualizar = false;

                res.Add(pf);
            }

            return res;
        }
    }
}
