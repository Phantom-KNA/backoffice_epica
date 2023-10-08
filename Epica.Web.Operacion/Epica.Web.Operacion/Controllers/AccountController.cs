using Epica.Web.Operacion.Extensions;
using Epica.Web.Operacion.Helpers;
using Epica.Web.Operacion.Models.Request;
using Epica.Web.Operacion.Services.Login;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Principal;

namespace Epica.Web.Operacion.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserContextService _userContextService;
        private readonly ILoginApiClient _loginApiClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(ILoginApiClient loginApiClient,
                    UserContextService userContextService,
                    IHttpContextAccessor httpContextAccessor
            ) 
        {
            _userContextService = userContextService;
            _loginApiClient = loginApiClient;
            _httpContextAccessor = httpContextAccessor;

        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Account/Login.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> Login(string email, string password, string nombreDispositivo, string ipAddress)
        {
            var loginRequest = new LoginRequest()
            {
                Email = email,
                Password = password,
                Ip = ipAddress,
                DispositivoAcceso = nombreDispositivo ?? ""
            };

            var loginResponse = await _loginApiClient.GetCredentialsAsync(loginRequest, _userContextService);

            if (loginResponse.IsAuthenticated)
            {
                HttpContext.Session.SetObject("LoginResponse", loginResponse);
                HttpContext.Session.SetString("CurrentSession", "Ok");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Nombre de usuario o contraseña inválidos.";
                return View("~/Views/Account/Login.cshtml");
            }

        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Response.Clear();

            HttpContext.Response.Cookies.Delete(".AspNetCore.Session");
            HttpContext.Response.Cookies.Delete(".AspNetCore.Cookies");
            
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);


            return RedirectToAction("Login", "Account");
        }
    }
}
