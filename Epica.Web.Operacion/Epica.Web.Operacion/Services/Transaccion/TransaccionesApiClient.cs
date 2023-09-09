using Epica.Web.Operacion.Config;
using Epica.Web.Operacion.Models.Request;
using Epica.Web.Operacion.Models.Response;
using Epica.Web.Operacion.Services.UserResolver;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
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

        public async Task<TransaccionDetailsResponse> GetTransaccionRastreoCobranzaAsync(string rastreoCobranza)
        {
            TransaccionDetailsResponse transaccionResponse = new TransaccionDetailsResponse();
            try
            {
                var uri = Urls.Transaccion + UrlsConfig.TransaccionesOperations.GetTransaccionRastreoCobranza(rastreoCobranza);
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    transaccionResponse = JsonConvert.DeserializeObject<TransaccionDetailsResponse>(jsonResponse);
                }

            }
            catch (Exception ex)
            {
                return transaccionResponse;
            }

            return transaccionResponse;
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
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    ListaTransacciones = JsonConvert.DeserializeObject<List<TransaccionesResponse>>(jsonResponse, settings);
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
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    result = int.Parse(jsonResponse);
                }

            }
            catch (Exception)
            {
                return result;
            }

            return result;
        }

        public async Task<MensajeResponse> GetModificarTransaccion(ModificarTransaccionRequest request)
        {
            MensajeResponse respuesta = new MensajeResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.TransaccionesOperations.ModificarTransaccion();
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
                respuesta.Error = true;
                respuesta.Detalle = ex.Message;
                return respuesta;
            }

            return respuesta;
        }

        public async Task<TransaccionesDetailsgeneralResponse> GetTransaccionesCuentaAsync(int idCuenta)
        {
            TransaccionesDetailsgeneralResponse? ListaTransacciones = new TransaccionesDetailsgeneralResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.TransaccionesOperations.GetTransaccionesPorCuenta(idCuenta);
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
                    ListaTransacciones = JsonConvert.DeserializeObject<TransaccionesDetailsgeneralResponse>(jsonResponse, settings);
                }

            }
            catch (Exception ex)
            {
                return ListaTransacciones;
            }

            return ListaTransacciones;
        }

        public async Task<MensajeResponse> GetRegistroTransaccion(RegistrarTransaccionRequest request)
        {
            MensajeResponse respuesta = new MensajeResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.TransaccionesOperations.GetRegistraTransaccion();
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
                respuesta.Error = true;
                respuesta.Detalle = ex.Message;
                return respuesta;
            }

            return respuesta;
        }

        public async Task<TransaccionDetailsResponse> GetTransaccionDetalleAsync(int idCuenta)
        {
            TransaccionDetailsResponse? TransaccionDetalle = new TransaccionDetailsResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.TransaccionesOperations.DetalleTransaccion(idCuenta);
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    TransaccionDetalle = JsonConvert.DeserializeObject<TransaccionDetailsResponse>(jsonResponse);
                }

            }
            catch (Exception ex)
            {
                return TransaccionDetalle;
            }

            return TransaccionDetalle;
        }

        public async Task<TransaccionDetailsResponse> GetTransaccionDetalleByCobranzaAsync(string cobranzaRef)
        {
            TransaccionDetailsResponse? TransaccionDetalle = new TransaccionDetailsResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.TransaccionesOperations.DetalleTransaccionClaveCobranza(cobranzaRef);
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    TransaccionDetalle = JsonConvert.DeserializeObject<TransaccionDetailsResponse>(jsonResponse);
                }

            }
            catch (Exception ex)
            {
                return TransaccionDetalle;
            }

            return TransaccionDetalle;
        }

        public async Task<TransaccionesDetailsgeneralResponse> GetTransaccionesClienteAsync(int idCliente)
        {
            TransaccionesDetailsgeneralResponse? ListaTransacciones = new TransaccionesDetailsgeneralResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.TransaccionesOperations.GetTransaccionesPorCliente(idCliente);
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
                    ListaTransacciones = JsonConvert.DeserializeObject<TransaccionesDetailsgeneralResponse>(jsonResponse, settings);
                }

            }
            catch (Exception ex)
            {
                return ListaTransacciones;
            }

            return ListaTransacciones;
        }

        public async Task<MensajeResponse> GetInsertaTransaccionesBatchAsync(List<CargaBachRequest> request)
        {
            MensajeResponse? respuesta = new MensajeResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.TransaccionesOperations.InsertarBatchTransaccion();
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
