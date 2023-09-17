using Epica.Web.Operacion.Helpers;
using Epica.Web.Operacion.Models.Request;
using Epica.Web.Operacion.Services.Authentication;
using Epica.Web.Operacion.Services.AuthenticationTokenCodigo;
using Epica.Web.Operacion.Services.Transaccion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;

namespace Epica.Web.Operacion.Controllers
{
    public class AutenticacionController : Controller
    {
        private readonly IServiceAuth _serviceAuth;

        public AutenticacionController(
            IServiceAuth serviceAuth
            )
        {
            _serviceAuth = serviceAuth; 
        }

        [HttpPost]
        public async Task<ActionResult> ValidarTokenYCodigo(string token, string codigo)
        {
            VerificarAccesoRequest request = new VerificarAccesoRequest()
            {
                Nip = codigo,
                Token = token
            };

            var response = await _serviceAuth.GetVerificarAccesoAsync(request);

            if(response.message == "Acceso correcto.")
            {
                return Ok(new { mensaje = true });

            }
            else
            {
                return Ok(new { mensaje = "Token o código de seguridad incorrectos" });
            }
        }
    }
}
