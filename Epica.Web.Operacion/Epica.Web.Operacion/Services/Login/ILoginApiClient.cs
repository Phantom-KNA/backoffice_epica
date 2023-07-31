using Epica.Web.Operacion.Models.Request;

namespace Epica.Web.Operacion.Services.Login
{
    public interface ILoginApiClient
    {
        Task<LoginResponse> GetCredentialsAsync(LoginRequest loginRequest);

    }
}
