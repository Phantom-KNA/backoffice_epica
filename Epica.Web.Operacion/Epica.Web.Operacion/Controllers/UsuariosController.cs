using Epica.Web.Operacion.Helpers;
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
    }
}
