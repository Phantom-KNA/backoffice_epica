using Epica.Web.Operacion.Config;
using Epica.Web.Operacion.Models.Request;
using Epica.Web.Operacion.Services.Transaccion;
using Epica.Web.Operacion.Services.UserResolver;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace Epica.Web.Operacion.Services.Login
{
    public class LoginApiClient: ApiClientBase, ILoginApiClient
    {
        public LoginApiClient(
            HttpClient httpClient,
            ILogger<CuentaApiClient> logger,
            IOptions<UrlsConfig> config,
            IHttpContextAccessor httpContext,
            IConfiguration configuration,
            IUserResolver userResolver) : base(httpClient, logger, config, httpContext, configuration, userResolver)
        {
        }

        public async Task<LoginResponse> GetCredentialsAsync(LoginRequest loginRequest)
        {
            LoginResponse loginResponse = new LoginResponse();

            try
            {
                //var uri = Urls.Transaccion + UrlsConfig.LoginOperations.GetCredentials(loginRequest.User ?? "", loginRequest.Password ?? "");
                var apiClient = new HttpClient();
                apiClient.DefaultRequestHeaders.Add("Api-Key", "tmfiiA3sCEe9Ybf4GL5D8gqlN0BOtWakmgvD1yHF6BhA");
                var jsonRequest = JsonConvert.SerializeObject(loginRequest);
                var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                var response = await apiClient.PostAsync(Urls.Transaccion + UrlsConfig.LoginOperations.GetCredentials(), httpContent);

                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    loginResponse = JsonConvert.DeserializeObject<LoginResponse>(jsonResponse);

                    loginResponse.IsAuthenticated = true;

                }
                else
                {
                    loginResponse.IsAuthenticated = false;

                }

            }
            catch (Exception ex)
            {
                loginResponse.IsAuthenticated = false;

            }

            return loginResponse;
        }
    }
}
