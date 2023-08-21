using Epica.Web.Operacion.Helpers;
using Epica.Web.Operacion.Models.Common;
using Epica.Web.Operacion.Services.Usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Epica.Web.Operacion.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly UserContextService _userContextService;
        private readonly IUsuariosApiClient _usuariosApiClient;

        public UsuariosController(UserContextService userContextService,
            IUsuariosApiClient usuariosApiClient) 
        {
            _userContextService = userContextService;
            _usuariosApiClient = usuariosApiClient;
        }

        [Authorize]
        public async Task<ActionResult> GestionarPermisos()
        {
            var loginResponse = _userContextService.GetLoginResponse();
            if (loginResponse.Rol == "Administrador")
            {
                //var listaUsuarios =  await _usuariosApiClient.GetUsuariosRolesAsync();
                return View();
            }
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
            //ListPF = await _usuariosApiClient.GetUsuariosRolesAsync();

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

    }
}
