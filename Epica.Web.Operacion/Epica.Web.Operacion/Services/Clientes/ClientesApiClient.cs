using Epica.Web.Operacion.Config;
using Epica.Web.Operacion.Helpers;
using Epica.Web.Operacion.Models.Common;
using Epica.Web.Operacion.Models.Entities;
using Epica.Web.Operacion.Models.Request;
using Epica.Web.Operacion.Services.UserResolver;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace Epica.Web.Operacion.Services.Transaccion
{
    public class ClientesApiClient : ApiClientBase, IClientesApiClient
    {
        private readonly UserContextService _userContextService;

        public ClientesApiClient(
            HttpClient httpClient, 
            ILogger<CuentaApiClient> logger, 
            IOptions<UrlsConfig> config, 
            IHttpContextAccessor httpContext, 
            IConfiguration configuration, 
            UserContextService userContextService,
            IUserResolver userResolver) : base(httpClient, logger, config, httpContext, configuration, userResolver)
        {
            _userContextService = userContextService;
        }

        public async Task<(List<ClienteResponse>, int)> GetClientesAsync(int pageNumber, int recordsTotal, int columna, bool ascendente)
        {

            List<ClienteResponse>? listaClientes = new List<ClienteResponse>();
            int paginacion = 0;

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.ClientesOperations.GetClienteInfo(pageNumber, recordsTotal);

                if (columna != 0) {
                    uri += string.Format("&columna={0}&ascendente={1}", Convert.ToString(columna), ascendente);
                }

                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    listaClientes = JsonConvert.DeserializeObject<List<ClienteResponse>>(jsonResponse, settings);
                    paginacion = int.Parse(response.Headers.GetValues("x-totalRecord").FirstOrDefault()!.ToString());
                    return (listaClientes, paginacion)!;
                }

            } catch (Exception) {
                return (listaClientes, paginacion)!; ;
            }

            return (listaClientes, paginacion);
        }

        public async Task<(List<ClienteResponse>, int)> GetClientesFilterAsync(int pageNumber, int recordsTotal, int columna, bool ascendente, List<RequestListFilters> filters)
        {
            FiltroDatosResponse? responseDatos = new FiltroDatosResponse();
            List<ClienteResponse>? listaClientes = new List<ClienteResponse>();
            int paginacion = 0;

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.ClientesOperations.GetClienteFiltroInfo(pageNumber, recordsTotal);

                var filtroNombreCliente = filters.FirstOrDefault(x => x.Key == "nombreCliente");
                var filtroTelefono = filters.FirstOrDefault(x => x.Key == "telefono");
                var filtroCorreoElectronico = filters.FirstOrDefault(x => x.Key == "correoElectronico");
                var filtroCurp = filters.FirstOrDefault(x => x.Key == "curp");
                var filtroOrganizacion = filters.FirstOrDefault(x => x.Key == "organizacion");
                var filtroEstatusWeb = filters.FirstOrDefault(x => x.Key == "estatusWeb");
                var filtroEstatusTotal = filters.FirstOrDefault(x => x.Key == "estatusTotal");

                if (filtroNombreCliente.Value != null) {
                    uri += string.Format("&nombre={0}", Convert.ToString(filtroNombreCliente.Value));
                }

                if (filtroTelefono.Value != null)
                {
                    uri += string.Format("&telefono={0}", Convert.ToString(filtroTelefono.Value));
                }

                if (filtroCorreoElectronico.Value != null)
                {
                    uri += string.Format("&correo={0}", Convert.ToString(filtroCorreoElectronico.Value));
                }

                if (filtroCurp.Value != null)
                {
                    uri += string.Format("&curp={0}", Convert.ToString(filtroCurp.Value));
                }

                if (filtroOrganizacion.Value != null)
                {
                    uri += string.Format("&organizacion={0}", Convert.ToString(filtroOrganizacion.Value));
                }

                if (filtroEstatusWeb.Value != null)
                {
                    uri += string.Format("&estatusWeb={0}", Convert.ToString(filtroEstatusWeb.Value));
                }

                if (filtroEstatusTotal.Value != null)
                {
                    uri += string.Format("&estatusTotal={0}", Convert.ToString(filtroEstatusWeb.Value));
                }

                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    responseDatos = JsonConvert.DeserializeObject<FiltroDatosResponse>(jsonResponse, settings);

                    listaClientes = responseDatos.listaResultado;
                    paginacion = Convert.ToInt32(responseDatos.cantidadEncontrada);

                    return (listaClientes, paginacion)!;
                }

            }
            catch (Exception)
            {
                return (listaClientes, paginacion)!; ;
            }

            return (listaClientes, paginacion);
        }

        public async Task<List<DatosCatalogoResponse>> GetClientesbyNombreAsync(string nombreCliente)
        {
            List<DatosCatalogoResponse> listaClientes = new List<DatosCatalogoResponse>();
            try
            {
                var uri = Urls.Transaccion + UrlsConfig.ClientesOperations.GetClientesByNombre(nombreCliente);
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    listaClientes = JsonConvert.DeserializeObject<List<DatosCatalogoResponse>>(jsonResponse)!;
                }

            }
            catch (Exception ex)
            {
                return listaClientes;
            }

            return listaClientes;
        }

        public async Task<List<DatosClienteEntity>> GetDetallesClientesByNombresAsync(string NombresApellidos)
        {
            List<DatosClienteEntity> listaClientes = new List<DatosClienteEntity>();
            try
            {
                var uri = Urls.Transaccion + UrlsConfig.ClientesOperations.GetDetallesClientesByNombres(NombresApellidos);
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    listaClientes = JsonConvert.DeserializeObject<List<DatosClienteEntity>>(jsonResponse)!;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return listaClientes;
        }

        public async Task<int> GetTotalClientesAsync()
        {
            int result = 0;
            try
            {
                var uri = Urls.Transaccion + UrlsConfig.ClientesOperations.GetClientesTotal();
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

        public async Task<ClienteDetailsResponse> GetClienteAsync(int id)
        {
            ClienteDetailsResponse? cliente = new ClienteDetailsResponse();
            try
            {
                var uri = Urls.Transaccion + UrlsConfig.ClientesOperations.GetCliente(id);
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
                    cliente = JsonConvert.DeserializeObject<ClienteDetailsResponse>(jsonResponse, settings);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return cliente!;
        }

        public async Task<DocumentosClienteDetailsResponse> GetDocumentosClienteAsync(int id)
        {
            DocumentosClienteDetailsResponse? listaDocumentosCliente = new DocumentosClienteDetailsResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.ClientesOperations.GetClienteDocumentos(id);
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
                    listaDocumentosCliente = JsonConvert.DeserializeObject<DocumentosClienteDetailsResponse>(jsonResponse, settings);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return listaDocumentosCliente!;
        }

        public async Task<BloqueoWebResponse> GetBloqueoWeb(BloqueoWebClienteRequest request)
        {
            BloqueoWebResponse respuesta = new BloqueoWebResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.ClientesOperations.GetBloqueaWebCliente();
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await ApiClient.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    respuesta = JsonConvert.DeserializeObject<BloqueoWebResponse>(jsonResponse)!;
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

        public async Task<BloqueoTotalResponse> GetBloqueoTotal(BloqueoTotalClienteRequest request)
        {
            BloqueoTotalResponse respuesta = new BloqueoTotalResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.ClientesOperations.GetBloqueaTotalCliente();
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await ApiClient.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    respuesta = JsonConvert.DeserializeObject<BloqueoTotalResponse>(jsonResponse)!;
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

        public async Task<MensajeResponse> GetRegistroCliente(RegistroModificacionClienteRequest request)
        {
            MensajeResponse respuesta = new MensajeResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.ClientesOperations.InsertarClienteNuevo();
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

        public async Task<MensajeResponse> GetModificarCliente(RegistroModificacionClienteRequest request)
        {
            MensajeResponse respuesta = new MensajeResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.ClientesOperations.ModificarCliente();
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

        public async Task<ClienteDetailsResponse> GetDetallesCliente(int id)
        {
            ClienteDetailsResponse? cliente = new ClienteDetailsResponse();
            try
            {
                var uri = Urls.Transaccion + UrlsConfig.ClientesOperations.GetCliente(id);
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
                    cliente = JsonConvert.DeserializeObject<ClienteDetailsResponse>(jsonResponse, settings);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return cliente!;
        }
        public async Task<MensajeResponse> GetRegistroAsignacionCuentaCliente(AsignarCuentaResponse request)
        {
            MensajeResponse respuesta = new MensajeResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.ClientesOperations.AsignarCuentaCliente();
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

        public async Task<MensajeResponse> GetRegistroDesvincularCuentaCliente(DesvincularCuentaResponse request)
        {
            MensajeResponse respuesta = new MensajeResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.ClientesOperations.DesvincularCuentaCliente();
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

        public async Task<MensajeResponse> GetRestablecerContraseñaCorreo(string correo)
        {
            MensajeResponse respuesta = new MensajeResponse();

            try
            {
                var uri = UrlApi + UrlsConfig.AuthenticateOperations.RestableceContraseñaCorreo(correo);
                var json = JsonConvert.SerializeObject(correo);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var responseToken = _userContextService.GetLoginResponse();
                ApiClient.DefaultRequestHeaders.Clear();
                ApiClient.DefaultRequestHeaders.Add("token_type", "Bearer");
                ApiClient.DefaultRequestHeaders.Add("access_token", responseToken.Token);

                var response = await ApiClient.PostAsync(uri,content);
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

        public async Task<MensajeResponse> GetRestablecerContraseñaTelefono(string telefono)
        {
            MensajeResponse respuesta = new MensajeResponse();

            try
            {
                var uri = UrlApi + UrlsConfig.AuthenticateOperations.RestableceContraseñaTelefono(telefono);
                var json = JsonConvert.SerializeObject(telefono);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var responseToken = _userContextService.GetLoginResponse();
                ApiClient.DefaultRequestHeaders.Clear();
                ApiClient.DefaultRequestHeaders.Add("token_type", "Bearer");
                ApiClient.DefaultRequestHeaders.Add("access_token", responseToken.Token);

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

        public async Task<DocumentoShowResponse> GetVisualizarDocumentosClienteAsync(string url)
        {
            Stream? docFile = null;
            DocumentoShowResponse? documentoRecibido = new DocumentoShowResponse(); 

            try
            {
                var responseToken = _userContextService.GetLoginResponse();
                ApiClient.DefaultRequestHeaders.Clear();

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Authorization", "Bearer " + responseToken.Token);
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    System.Net.Http.HttpContent content = response.Content;
                    var h = content.Headers;

                    documentoRecibido.MimeType = response.Content.Headers.ContentType!.ToString();
                    documentoRecibido.Nombre = response.Content.Headers.ContentDisposition!.ToString().Substring(response.Content.Headers.ContentDisposition.ToString().IndexOf("filename=") + 10).Replace("\"", "");
                    documentoRecibido.Documento = await content.ReadAsStreamAsync();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return documentoRecibido;
        }

        public async Task<MensajeResponse> GetInsertaDocumentoClienteAsync(DocumentoClienteRequest request)
        {
            MensajeResponse respuesta = new MensajeResponse();

            try
            {
                var responseToken = _userContextService.GetLoginResponse();
                ApiClient.DefaultRequestHeaders.Clear();

                var uri = UrlApi + UrlsConfig.ClientesOperations.GetInsertaDocumentoCliente();
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                ApiClient.DefaultRequestHeaders.Clear();
                ApiClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + responseToken.Token);

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
    }
}
