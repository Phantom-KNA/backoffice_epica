using Epica.Web.Operacion.Config;
using Epica.Web.Operacion.Services.UserResolver;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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

        public async Task<List<TransaccionesResponse>> GetTransaccionesAsync(int pageNumber, int recordsTotal)
        {
            List<TransaccionesResponse>? ListaTransacciones = new List<TransaccionesResponse>();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.TransaccionesOperations.GetTransacciones(pageNumber, recordsTotal);
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    ListaTransacciones = JsonConvert.DeserializeObject<List<TransaccionesResponse>>(jsonResponse);
                }

            } catch (Exception ex) {
                return ListaTransacciones;
            }

            return ListaTransacciones;
        }

        public async Task<TransaccionesResponse> GetTransaccionAsync(int idInterno)
        {
            var uri = Urls.Transaccion + UrlsConfig.TransaccionesOperations.GetTransaccion(idInterno);

            var response = await ApiClient.GetAsync(uri);

            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TransaccionesResponse>(stringResponse);
        }

        public async Task<int> GetTotalTransaccionesAsync()
        {
            int result = 0;
            try
            {
                var uri = Urls.Transaccion + UrlsConfig.TransaccionesOperations.GetTransaccionesTotal();
                var apiClient = new HttpClient();
                apiClient.DefaultRequestHeaders.Add("Api-Key", "tmfiiA3sCEe9Ybf4GL5D8gqlN0BOtWakmgvD1yHF6BhA");

                var response = await apiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    result = int.Parse(jsonResponse);
                }

            }
            catch (Exception ex)
            {
                return result;
            }

            return result;
        }
    }
}
