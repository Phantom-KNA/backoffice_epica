using Epica.Web.Operacion.Helpers;
using Epica.Web.Operacion.Models.Request;
using Epica.Web.Operacion.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Epica.Web.Operacion.Controllers
{
    public class MedioPagoController: Controller
    {
        private readonly UserContextService _userContextService;

        public MedioPagoController(UserContextService userContextService)
        {
            _userContextService = userContextService;
        }

        [Authorize]
        public IActionResult Index(string index)
        {
            var loginResponse = _userContextService.GetLoginResponse();
            var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Tarjetas" && modulo.Ver == 0);
            if (validacion == true)
            {
                if (index == null)
                {
                    index = "Tarjetas";
                    ViewBag.medioP = index;
                    ViewBag.Vista = $"~/Views/mediopago/{index}/_Index.cshtml";
                }
                else
                {
                    ViewBag.medioP = index;
                    ViewBag.Vista = $"~/Views/mediopago/{index}/_Index.cshtml";
                }
                return View(loginResponse);

            }

            return NotFound();
        }
    }
}
