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
            var uri = Urls.Transaccion + UrlsConfig.TransaccionesOperations.GetTransacciones();
           
            var response = await ApiClient.GetAsync(uri);

            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<Transacciones>>(stringResponse, SerializerOptions);
        }

        public async Task<Transacciones> GetTransaccionAsync(int idInterno)
        {
            var uri = Urls.Transaccion + UrlsConfig.TransaccionesOperations.GetTransaccion(idInterno);

            var response = await ApiClient.GetAsync(uri);

            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<Transacciones>(stringResponse, SerializerOptions);
        }
    }
}
