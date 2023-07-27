using Microsoft.AspNetCore.Mvc;

namespace Epica.Web.Operacion.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Account/Login.cshtml");
        }

        [HttpPost]
        public IActionResult Login(string username = "", string password = "")
        {
            HttpContext.Session.SetString("CurrentSession", "Ok");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}
