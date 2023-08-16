using Epica.Web.Operacion.Models;
using Epica.Web.Operacion.Models.ViewModels;
using Epica.Web.Operacion.Services.Transaccion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Collections.Generic;

namespace Epica.Web.Operacion.Controllers;
/// <summary>
/// Controlador para la página de inicio.
/// </summary>

public class HomeController : Controller
{
    private readonly IUsuariosApiClient _usuariosApiClient;
    private readonly ITransaccionesApiClient _transaccionesApiClient;
    private readonly ICuentaApiClient _cuentaApiClient;

    /// <summary>
    /// Acción para mostrar la pagina de inicio.
    /// </summary>
    /// <returns>Vista de la pagina de inicio.</returns>

    public HomeController(
        IUsuariosApiClient usuariosApiClient,
        ITransaccionesApiClient transaccionesApiClient,
        ICuentaApiClient cuentaApiClient
        )
    {
        _usuariosApiClient = usuariosApiClient;
        _transaccionesApiClient = transaccionesApiClient;
        _cuentaApiClient = cuentaApiClient;
    }

    public async Task<ActionResult> Index()
    {
        if (HttpContext.Session.GetString("CurrentSession") == null)
        {
            return RedirectToAction("Login", "Account");
        }
        else
        {

            HomeIndexViewModel indexViewModel = new HomeIndexViewModel()
            {
                UsuariosTotal = await _usuariosApiClient.GetTotalUsuariosAsync(),
                TransaccionesTotal = await _transaccionesApiClient.GetTotalTransaccionesAsync(),
                CuentasTotal = await _cuentaApiClient.GetTotalCuentasAsync(),
                FechaActual = DateTime.Now

        };

            return View(indexViewModel);
        }
    }

    public IActionResult Account()
    {
        return View();
    }
}
