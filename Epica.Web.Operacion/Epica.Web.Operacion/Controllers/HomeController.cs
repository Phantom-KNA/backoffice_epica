using Microsoft.AspNetCore.Mvc;

namespace Epica.Web.Operacion.Controllers;
/// <summary>
/// Controlador para la página de inicio.
/// </summary>

public class HomeController : Controller
{
    /// <summary>
    /// Acción para mostrar la pagina de inicio.
    /// </summary>
    /// <returns>Vista de la pagina de inicio.</returns>
    public IActionResult Index()
    {
        //if (HttpContext.Session.GetString("CurrentSession") == null)
        //{
        //    return RedirectToAction("Login", "Account");
        //}
        //else
        //{
        //    return RedirectToAction("Index", "MyAccounts");
        //}

        return View();
    }
}
