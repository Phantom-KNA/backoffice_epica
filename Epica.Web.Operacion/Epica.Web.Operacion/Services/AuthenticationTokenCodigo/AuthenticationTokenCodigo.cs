using Newtonsoft.Json.Linq;

namespace Epica.Web.Operacion.Services.AuthenticationTokenCodigo
{
    public class AuthenticationTokenCodigo: IAuthenticationTokenCodigo
    {
        public AuthenticationTokenCodigo() 
        {

        }      

        public bool GetValidarToken(string token)
        {
            if (token == "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool GetValidarCodigo(string codigo)
        {
            if (codigo == "123456")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
