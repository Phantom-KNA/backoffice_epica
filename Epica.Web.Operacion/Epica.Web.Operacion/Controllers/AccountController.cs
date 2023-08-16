using Epica.Web.Operacion.Extensions;
using Epica.Web.Operacion.Models.Request;
using Epica.Web.Operacion.Services.Login;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Epica.Web.Operacion.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILoginApiClient _loginApiClient;

        public AccountController(ILoginApiClient loginApiClient) 
        {
            _loginApiClient = loginApiClient;
        }
        
        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Account/Login.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> Login(string username, string password)
        {
            var loginRequest = new LoginRequest
            {
                User = username,
                Password = password
            };

            var loginResponse = await _loginApiClient.GetCredentialsAsync(loginRequest);

            if (loginResponse.IsAuthenticated)
            {
                HttpContext.Session.SetString("UserSession", JsonConvert.SerializeObject(loginResponse));
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

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}
