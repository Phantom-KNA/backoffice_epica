namespace Epica.Web.Operacion.Services.Authentication;

public interface IServiceAuth
{
    public Task<TokenResponse> Auth(string username, string password);
    string CreacionTokenWebSite(IConfiguration configuration, string token);
}
