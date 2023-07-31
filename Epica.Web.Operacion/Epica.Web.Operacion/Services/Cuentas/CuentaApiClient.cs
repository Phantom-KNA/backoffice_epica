using Epica.Web.Operacion.Config;
using Epica.Web.Operacion.Models.Entities;
using Epica.Web.Operacion.Services.UserResolver;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json;
using static Epica.Web.Operacion.Controllers.CuentaController;

namespace Epica.Web.Operacion.Services.Transaccion
{
    public class CuentaApiClient : ApiClientBase, ICuentaApiClient
    {
        public CuentaApiClient(
            HttpClient httpClient, 
            ILogger<CuentaApiClient> logger, 
            IOptions<UrlsConfig> config, 
            IHttpContextAccessor httpContext, 
            IConfiguration configuration, 
            IUserResolver userResolver) : base(httpClient, logger, config, httpContext, configuration, userResolver)
        {
        }

        public async Task<List<CuentasResponse>> GetCuentasAsync(int pageNumber, int recordsTotal)
        {

            List<CuentasResponse>? ListaCuentas = new List<CuentasResponse>();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.CuentasOperations.GetCuentas(pageNumber, recordsTotal);
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    ListaCuentas = JsonConvert.DeserializeObject<List<CuentasResponse>>(jsonResponse);
                }

            }
            catch (Exception ex)
            {
                return ListaCuentas;
            }

            return ListaCuentas;
        }

        public async Task<List<CobranzaReferenciadaResponse>> GetCobranzaReferenciadaAsync(int id)
        {

            List<CobranzaReferenciadaResponse>? ListaCobranza = new List<CobranzaReferenciadaResponse>();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.CuentasOperations.GetCobranzaReferenciada(id);
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    ListaCobranza = JsonConvert.DeserializeObject<List<CobranzaReferenciadaResponse>>(jsonResponse, settings);
                }

            }
            catch (Exception ex)
            {
                return ListaCobranza;
            }

            return ListaCobranza;
        }
    }
}
