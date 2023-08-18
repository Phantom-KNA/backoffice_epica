using Epica.Web.Operacion.Config;
using Epica.Web.Operacion.Models.Request;
using Epica.Web.Operacion.Services.UserResolver;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Epica.Web.Operacion.Services.Log
{
    public class LogsApiClient: ApiClientBase, ILogsApiClient
    {
        public LogsApiClient(HttpClient httpClient,
            ILogger<LogsApiClient> logger,
            IOptions<UrlsConfig> config,
            IHttpContextAccessor httpContext,
            IConfiguration configuration,
            IUserResolver userResolver) : base(httpClient, logger, config, httpContext, configuration, userResolver)
        {
        }

        public async Task<MensajeResponse> InsertarLogAsync(LogRequest logRequest)
        {

            MensajeResponse mensajeResponse = new MensajeResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.LogOperations.InsertarLog();
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    mensajeResponse = JsonConvert.DeserializeObject<MensajeResponse>(jsonResponse);
                }

            }
            catch (Exception ex)
            {
                return mensajeResponse;
            }

            return mensajeResponse;
        }
    }
}
