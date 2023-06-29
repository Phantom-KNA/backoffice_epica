using Epica.Web.Operacion.Services.Transaccion;
using Microsoft.AspNetCore.Mvc;

namespace Epica.Web.Operacion.Controllers;

public class HomeController : Controller
{
    private readonly ITransaccionesApiClient _transaccionesApiClient;//Transacciones

    public HomeController(ITransaccionesApiClient transaccionesApiClient)
    {
        _transaccionesApiClient = transaccionesApiClient;
    }

    public async Task<IActionResult> Transacciones()
    {
        var recibir = await _transaccionesApiClient.GetTransaccionesAsync();
        return Json(recibir);
    }
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
