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

        public async Task<List<CuentasResponse>> GetCuentasAsync()
        {
            //URL de pruebas
            HttpClient ApiClient = new HttpClient();
            List<CuentasResponse>? ListaCuentas = new List<CuentasResponse>();

            try
            {
                ApiClient.BaseAddress = new Uri("https://localhost:44308/");
                //ApiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserResolver.GetToken());
                var url = "https://localhost:44308/" + "api/cuentas";
                var response = await ApiClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    ListaCuentas = JsonConvert.DeserializeObject<List<CuentasResponse>>(jsonResponse);
                }

            } catch (Exception ex) {
                return ListaCuentas;
            }

            return ListaCuentas;
        }
    }
}
