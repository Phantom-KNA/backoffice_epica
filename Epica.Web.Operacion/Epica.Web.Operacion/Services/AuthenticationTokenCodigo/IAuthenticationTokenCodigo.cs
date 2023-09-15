namespace Epica.Web.Operacion.Services.AuthenticationTokenCodigo
{
    public interface IAuthenticationTokenCodigo
    {
        bool GetValidarToken(string token);
        bool GetValidarCodigo(string codigo);

    }
}
