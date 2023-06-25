using Microsoft.AspNetCore.Mvc;

namespace Epica.Web.Operacion.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (HttpContext.Session.GetString("CurrentSession") == null)
        {
            return RedirectToAction("Login", "Account");
        }
        else
        {
            return RedirectToAction("Index", "MyAccounts");
        }
    }

    public IActionResult Account()
    {
        return View();
    }
}
