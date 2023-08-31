using Epica.Web.Operacion.Config;
using Epica.Web.Operacion.Models.Entities;
using Epica.Web.Operacion.Models.Request;
using Epica.Web.Operacion.Services.UserResolver;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using static Epica.Web.Operacion.Controllers.CuentaController;

namespace Epica.Web.Operacion.Services.Transaccion
{
    public class TarjetasApiClient : ApiClientBase, ITarjetasApiClient
    {
        public TarjetasApiClient(
            HttpClient httpClient, 
            ILogger<CuentaApiClient> logger, 
            IOptions<UrlsConfig> config, 
            IHttpContextAccessor httpContext, 
            IConfiguration configuration, 
            IUserResolver userResolver) : base(httpClient, logger, config, httpContext, configuration, userResolver)
        {
        }

        public async Task<ResumenTarjetasResponse> GetBuscarTarjetasasync(string noTarjeta)
        {
            ResumenTarjetasResponse tarjetaResponse = new ResumenTarjetasResponse();
            try
            {
                var uri = Urls.Transaccion + UrlsConfig.TarjetasOperations.GetBuscarTarjeta(noTarjeta);
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    tarjetaResponse = JsonConvert.DeserializeObject<ResumenTarjetasResponse>(jsonResponse);
                }

            }
            catch (Exception ex)
            {
                return tarjetaResponse;
            }

            return tarjetaResponse;
        }
        public async Task<List<TarjetasResponse>> GetTarjetasClientesAsync(int idCliente)
        {

            List<TarjetasResponse>? listaClientes = new List<TarjetasResponse>();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.TarjetasOperations.GetTarjetasCliente(idCliente);
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    listaClientes = JsonConvert.DeserializeObject<List<TarjetasResponse>>(jsonResponse,settings);
                }

            }
            catch (Exception ex)
            {
                return listaClientes;
            }

            return listaClientes;
        }

        public async Task<List<TarjetasResponse>> GetTarjetasAsync(int pageNumber, int recordsTotal)
        {

            List<TarjetasResponse>? listaClientes = new List<TarjetasResponse>();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.TarjetasOperations.GetTarjetas(pageNumber, recordsTotal);
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    listaClientes = JsonConvert.DeserializeObject<List<TarjetasResponse>>(jsonResponse,settings);
                }

            }
            catch (Exception ex)
            {
                return listaClientes;
            }

            return listaClientes;
        }

        public async Task<MensajeResponse> GetRegistroTarjetaAsync(RegistrarTarjetaRequest request)
        {
            MensajeResponse respuesta = new MensajeResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.TarjetasOperations.GetTarjetasRegistra();
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await ApiClient.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    respuesta = JsonConvert.DeserializeObject<MensajeResponse>(jsonResponse);
                }

            }
            catch (Exception ex)
            {
                return respuesta;
            }

            return respuesta;
        }


    }
}
