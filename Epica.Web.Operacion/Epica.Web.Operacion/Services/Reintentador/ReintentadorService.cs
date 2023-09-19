using Epica.Web.Operacion.Config;
using Epica.Web.Operacion.Models.Request;
using Epica.Web.Operacion.Services.UserResolver;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace Epica.Web.Operacion.Services.Log
{
    public class ReintentadorService : ApiClientBase, IReintentadorService
    {
        public ReintentadorService(HttpClient httpClient,
            ILogger<LogsApiClient> logger,
            IOptions<UrlsConfig> config,
            IHttpContextAccessor httpContext,
            IConfiguration configuration,
            IUserResolver userResolver) : base(httpClient, logger, config, httpContext, configuration, userResolver)
        {
        }

        public async Task<MensajeResponse> GetReenviarTransaccionesAsync(List<string> request)
        {
            MensajeResponse? respuesta = new MensajeResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.ReintentadorOperations.ReenviarTransacciones();

                string queryString = string.Join("&", request.ConvertAll(s => $"clavesRastreo={Uri.EscapeDataString(s)}"));

                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                uri = $"{uri}?{queryString}";

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
            }

            return respuesta;
        }

        public async Task<MensajeResponse> GetReenviarTransaccionAsync(string claveRastreo)
        {
            MensajeResponse? respuesta = new MensajeResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.ReintentadorOperations.ReenviarTransaccion(claveRastreo);
                var json = JsonConvert.SerializeObject("");
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

        public async Task<MensajeResponse> GetDevolverTransaccionesAsync(List<string> request)
        {
            MensajeResponse? respuesta = new MensajeResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.ReintentadorOperations.DevolverTransacciones();

                string queryString = string.Join("&", request.ConvertAll(s => $"clavesRastreo={Uri.EscapeDataString(s)}"));

                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                uri = $"{uri}?{queryString}";

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
            }

            return respuesta;
        }

        public async Task<MensajeResponse> GetDevolverTransaccionAsync(string claveRastreo)
        {
            MensajeResponse? respuesta = new MensajeResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.ReintentadorOperations.DevolverTransaccion(claveRastreo);
                var json = JsonConvert.SerializeObject("");
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

        public async Task<List<DevolucionesResponse>> GetTransaccionesDevolverReintentar()
        {
            List<DevolucionesResponse> listaTransacciones = new List<DevolucionesResponse>();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.ReintentadorOperations.GetTransaccionesDevolver();
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
                    listaTransacciones = JsonConvert.DeserializeObject<List<DevolucionesResponse>>(jsonResponse, settings);               
                }

            }
            catch (Exception ex)
            {
                return listaTransacciones;
            }

            return listaTransacciones;
        }
    }
}
