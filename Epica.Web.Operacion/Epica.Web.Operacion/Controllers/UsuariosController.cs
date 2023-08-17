using Epica.Web.Operacion.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Epica.Web.Operacion.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly UserContextService _userContextService;

        public UsuariosController(UserContextService userContextService) 
        {
            _userContextService = userContextService;
        }

        [Authorize]
        public IActionResult GestionarPermisos()
        {
            var loginResponse = _userContextService.GetLoginResponse();
            return View(loginResponse);
        }
    }
}
