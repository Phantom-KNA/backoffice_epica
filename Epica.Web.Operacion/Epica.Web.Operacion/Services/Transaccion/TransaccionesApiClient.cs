﻿using Epica.Web.Operacion.Config;
using Epica.Web.Operacion.Helpers;
using Epica.Web.Operacion.Models.Request;
using Epica.Web.Operacion.Services.UserResolver;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace Epica.Web.Operacion.Services.Transaccion
{
    public class TransaccionesApiClient : ApiClientBase, ITransaccionesApiClient
    {
        private readonly UserContextService _userContextService;
        public TransaccionesApiClient(
            HttpClient httpClient, 
            ILogger<TransaccionesApiClient> logger, 
            IOptions<UrlsConfig> config, 
            IHttpContextAccessor httpContext, 
            IConfiguration configuration,
            UserContextService userContextService,
            IUserResolver userResolver) : base(httpClient, logger, config, httpContext, configuration, userResolver)
        {
            _userContextService = userContextService;
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
                    transaccionResponse = JsonConvert.DeserializeObject<TransaccionDetailsResponse>(jsonResponse)!;
                }

            }
            catch (Exception)
            {
                return transaccionResponse;
            }

            return transaccionResponse;
        }

        public async Task<(List<CargaBachRequest>, int)> GetTransaccionesMasivaAsync(int pageNumber, int recordsTotal, int idUsuario)
        {
            List<CargaBachRequest>? ListaTransacciones = new List<CargaBachRequest>();
            int paginacion = 0;

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.TransaccionesOperations.GetTransaccionesMasiva(pageNumber, recordsTotal, idUsuario);
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
                    ListaTransacciones = JsonConvert.DeserializeObject<List<CargaBachRequest>>(jsonResponse, settings);
                    paginacion = int.Parse(response.Headers.GetValues("x-totalRecord").FirstOrDefault()!.ToString());
                    return (JsonConvert.DeserializeObject<List<CargaBachRequest>>(jsonResponse), paginacion)!;
                }

            }
            catch (Exception)
            {
                return (ListaTransacciones, paginacion)!; ;
            }

            return (ListaTransacciones, paginacion);
        }

        public async Task<(List<TransaccionesResponse>, int)> GetTransaccionesAsync(int pageNumber, int recordsTotal)
        {
            List<TransaccionesResponse>? ListaTransacciones = new List<TransaccionesResponse>();
            int paginacion = 0;

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
                    paginacion = int.Parse(response.Headers.GetValues("x-totalRecord").FirstOrDefault()!.ToString());
                    return (JsonConvert.DeserializeObject<List<TransaccionesResponse>>(jsonResponse), paginacion)!;
                }

            }
            catch (Exception)
            {
                return (ListaTransacciones, paginacion)!;
            }

            return (ListaTransacciones, paginacion);
        }

        public async Task<TransaccionesResponse> GetTransaccionAsync(int idInterno)
        {
            var uri = Urls.Transaccion + UrlsConfig.TransaccionesOperations.GetTransaccion(idInterno);

            var response = await ApiClient.GetAsync(uri);

            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TransaccionesResponse>(stringResponse)!;
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
                    respuesta = JsonConvert.DeserializeObject<MensajeResponse>(jsonResponse)!;
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
            catch (Exception)
            {
                return ListaTransacciones!;
            }

            return ListaTransacciones!;
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
                    respuesta = JsonConvert.DeserializeObject<MensajeResponse>(jsonResponse)!;
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
            catch (Exception)
            {
                return TransaccionDetalle!;
            }

            return TransaccionDetalle!;
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
            catch (Exception)
            {
                return TransaccionDetalle!;
            }

            return TransaccionDetalle!;
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
            catch (Exception)
            {
                return ListaTransacciones!;
            }

            return ListaTransacciones!;
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
            catch (Exception)
            {
                return respuesta!;
            }

            return respuesta!;
        }

        public async Task<MensajeResponse> GetEliminarTransaccionBatchAsync(int idRegistro)
        {
            MensajeResponse respuesta = new MensajeResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.TransaccionesOperations.GetEliminarTransaccionBatch(idRegistro);
                var response = await ApiClient.DeleteAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    respuesta = JsonConvert.DeserializeObject<MensajeResponse>(jsonResponse)!;
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

        public async Task<CargaBachRequest> GetTransaccionBatchAsync(int idRegistro)
        {
            CargaBachRequest? TransaccionBatch = new CargaBachRequest();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.TransaccionesOperations.GetTransaccionBatch(idRegistro);
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    TransaccionBatch = JsonConvert.DeserializeObject<CargaBachRequest>(jsonResponse);
                }

            }
            catch (Exception)
            {
                return TransaccionBatch!;
            }

            return TransaccionBatch!;
        }

        public async Task<string> GetModificaTransaccionBatchAsync(CargaBachRequest request)
        {
            string respuesta = "";

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.TransaccionesOperations.GetTransaccionEditBatch();
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await ApiClient.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    respuesta = jsonResponse;
                }

            }
            catch (Exception ex)
            {
                return respuesta + ex.Message;
            }

            return respuesta;
        }

        public async Task<MensajeResponse> GetProcesaTransaccion(int idUsuario)
        {
            MensajeResponse respuesta = new MensajeResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.TransaccionesOperations.GetProcesarTransaccion(idUsuario);
                var json = JsonConvert.SerializeObject(idUsuario);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await ApiClient.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    respuesta = JsonConvert.DeserializeObject<MensajeResponse>(jsonResponse)!;
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

        public async Task<VoucherResponse> GetVoucherTransaccionAsync(int cuentaAhorro, int transaccion, string token)
        {
            VoucherResponse? docFile = new VoucherResponse();
            try
            {
                var uri = Urls.Transaccion + UrlsConfig.TransaccionesOperations.GetComprobanteTransaccion(cuentaAhorro, transaccion, token);
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    docFile = JsonConvert.DeserializeObject<VoucherResponse>(jsonResponse);
                }


            }
            catch (Exception ex)
            {
                throw new Exception("" + ex.Message);
            }

            return docFile!;
        }
        public async Task<int> GetTotalTransaccionesBatchAsync(int idUsuario)
        {
            int result = 0;
            try
            {
                var uri = Urls.Transaccion + UrlsConfig.TransaccionesOperations.GetTransaccionesBatchTotalPorUsuario(idUsuario);
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

        public async Task<MensajeResponse> GetEliminarTransaccionesBatchAsync(int idUsuario)
        {
            MensajeResponse respuesta = new MensajeResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.TransaccionesOperations.GetEliminarTransaccionBatchPorUsuario(idUsuario,0);
                var response = await ApiClient.DeleteAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    respuesta = JsonConvert.DeserializeObject<MensajeResponse>(jsonResponse)!;
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
    }
}
