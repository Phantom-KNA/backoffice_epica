namespace Epica.Web.Operacion.Services.Authentication;
/// <summary>
/// Interfaz que define métodos para autenticación y generación de tokens.
/// </summary>
public interface IServiceAuth
{
    public Task<TokenResponse> Auth(string username, string password);
    string CreacionTokenWebSite(IConfiguration configuration, string token);
}
