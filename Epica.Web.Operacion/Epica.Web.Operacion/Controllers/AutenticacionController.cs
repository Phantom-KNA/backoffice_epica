using Epica.Web.Operacion.Services.AuthenticationTokenCodigo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;

namespace Epica.Web.Operacion.Controllers
{
    public class AutenticacionController : Controller
    {
        private readonly IAuthenticationTokenCodigo _authenticationTokenCodigo;

        public AutenticacionController(IAuthenticationTokenCodigo authenticationTokenCodigo)
        {
            _authenticationTokenCodigo = authenticationTokenCodigo;
        }

        [HttpPost]
        public ActionResult ValidarTokenYCodigo(string token, string codigo)
        {
            var respuestaToken = _authenticationTokenCodigo.GetValidarToken(token);
            var respuestaCodigo = _authenticationTokenCodigo.GetValidarCodigo(codigo);
            if (respuestaToken == true && respuestaCodigo == true)
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
