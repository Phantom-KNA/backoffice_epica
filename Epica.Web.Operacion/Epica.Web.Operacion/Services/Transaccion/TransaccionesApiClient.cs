using Epica.Web.Operacion.Config;
using Epica.Web.Operacion.Models.Entities;
using Epica.Web.Operacion.Services.UserResolver;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Epica.Web.Operacion.Services.Transaccion
{
    public class TransaccionesApiClient : ApiClientBase, ITransaccionesApiClient
    {
        public TransaccionesApiClient(
            HttpClient httpClient, 
            ILogger<TransaccionesApiClient> logger, 
            IOptions<UrlsConfig> config, 
            IHttpContextAccessor httpContext, 
            IConfiguration configuration, 
            IUserResolver userResolver) : base(httpClient, logger, config, httpContext, configuration, userResolver)
        {
        }

        public async Task<List<Transacciones>> GetTransaccionesAsync()
        {
            var uri = "https://localhost:44308/api/resumentransaccion\r\n";
                //Urls.Transaccion + UrlsConfig.TransaccionesOperations.GetTransacciones();
            //ApiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {UserResolver.GetToken()}");

            //ApiClient.DefaultRequestHeaders.Remove("Authorization");
            //ApiClient.DefaultRequestHeaders.Add("x-api-key", UserResolver.GetToken());

            var response = await ApiClient.GetAsync(uri);

            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();

            try
            {
                return JsonSerializer.Deserialize<List<Transacciones>>(stringResponse, SerializerOptions);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Processing failed: {e.Message}");
            }

            return JsonSerializer.Deserialize<List<Transacciones>>(stringResponse, SerializerOptions);
        }
    }
}
