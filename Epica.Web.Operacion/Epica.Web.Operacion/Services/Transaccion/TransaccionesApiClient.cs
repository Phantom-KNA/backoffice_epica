using Epica.Web.Operacion.Config;
using Epica.Web.Operacion.Models.Request;
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

        public async Task<RegistrarModificarTransaccionResponse> GetModificarTransaccion(RegistrarTransaccionRequest request)
        {
            RegistrarModificarTransaccionResponse respuesta = new RegistrarModificarTransaccionResponse();

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
                    respuesta = JsonConvert.DeserializeObject<RegistrarModificarTransaccionResponse>(jsonResponse);
                }

            }
            catch (Exception ex)
            {
                respuesta.error = true;
                respuesta.detalle = ex.Message;
                return respuesta;
            }

            return respuesta;
        }

        public async Task<List<TransaccionesResponse>> GetTransaccionesCuentaAsync(int idCuenta)
        {
            List<TransaccionesResponse>? ListaTransacciones = new List<TransaccionesResponse>();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.TransaccionesOperations.GetTransaccionesPorCuenta(idCuenta);
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    ListaTransacciones = JsonConvert.DeserializeObject<List<TransaccionesResponse>>(jsonResponse);
                }

            }
            catch (Exception ex)
            {
                return ListaTransacciones;
            }

            return ListaTransacciones;
        }

        public async Task<RegistrarModificarTransaccionResponse> GetRegistroTransaccion(RegistrarTransaccionRequest request)
        {
            RegistrarModificarTransaccionResponse respuesta = new RegistrarModificarTransaccionResponse();

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
                    respuesta = JsonConvert.DeserializeObject<RegistrarModificarTransaccionResponse>(jsonResponse);
                }

            }
            catch (Exception ex)
            {
                respuesta.error = true;
                respuesta.detalle = ex.Message;
                return respuesta;
            }

            return respuesta;
        }

    }
}
