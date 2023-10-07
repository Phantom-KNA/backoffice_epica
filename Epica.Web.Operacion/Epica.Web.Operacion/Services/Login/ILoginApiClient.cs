using Epica.Web.Operacion.Helpers;
using Epica.Web.Operacion.Models.Request;

namespace Epica.Web.Operacion.Services.Login
{
    public interface ILoginApiClient
    {
        Task<LoginResponse> GetCredentialsAsync(LoginRequest loginRequest, UserContextService userContextService);
        Task LogoutAsync(HttpContext httpContext);


    }
}
