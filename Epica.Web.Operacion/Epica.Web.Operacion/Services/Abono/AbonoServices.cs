using Epica.Web.Operacion.Config;
using Epica.Web.Operacion.Helpers;
using Epica.Web.Operacion.Models.Common;
using Epica.Web.Operacion.Models.Request;
using Epica.Web.Operacion.Services.UserResolver;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Immutable;
using System.Net;
using System.Net.Http;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Epica.Web.Operacion.Services.Transaccion
{
    public class AbonoServices : ApiClientBase, IAbonoServices
    {
        private readonly UserContextService _userContextService;
        private readonly HttpClient _apiClient;

        public AbonoServices(
            HttpClient httpClient, 
            ILogger<AbonoServices> logger, 
            IOptions<UrlsConfig> config, 
            IHttpContextAccessor httpContext, 
            IConfiguration configuration,
            UserContextService userContextService,
            IUserResolver userResolver) : base(httpClient, logger, config, httpContext, configuration, userResolver)
        {
            _userContextService = userContextService;
            _apiClient = httpClient;
        }

        public async Task<(List<TransaccionSPEIINResponse>, int)> GetAbonosAsync(int pageNumber, int recordsTotal, int columna, bool ascendente)
        {
            List<TransaccionSPEIINResponse>? ListaAbonos = new List<TransaccionSPEIINResponse>();
            int paginacion = 0;
            ResponseSpeiINConsulta responseDatos = new ResponseSpeiINConsulta();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.AbonosOperations.GetAbonosSpeiIN(pageNumber, recordsTotal);

                if (columna != 0) {
                    uri += string.Format("&columna={0}&ascendente={1}", Convert.ToString(columna), ascendente);
                }

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
                    responseDatos = JsonConvert.DeserializeObject<ResponseSpeiINConsulta>(jsonResponse, settings);
                    paginacion = responseDatos.cantidadEncontrada;
                    ListaAbonos = responseDatos.listaResultado;
                    return (ListaAbonos, paginacion)!;
                }

            }
            catch (Exception)
            {
                return (ListaAbonos, paginacion)!;
            }

            return (ListaAbonos, paginacion);
        }

        public async Task<(List<TransaccionSPEIINResponse>, int)> GetAbonosFilterAsync(int pageNumber, int recordsTotal, int columna, bool ascendente, List<RequestListFilters> filters)
        {
            List<TransaccionSPEIINResponse>? ListaTransacciones = new List<TransaccionSPEIINResponse>();
            ResponseSpeiINConsulta responseConsulta = new ResponseSpeiINConsulta();
            int paginacion = 0;

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.AbonosOperations.GetAbonosSpeiINFiltro(pageNumber, recordsTotal);

                var filtroCuentaOrdenante = filters.FirstOrDefault(x => x.Key == "cuentaOrdenante");
                var filtroclaveRastreo = filters.FirstOrDefault(x => x.Key == "claveRastreo");
                var filtronombreOrdenante = filters.FirstOrDefault(x => x.Key == "nombreOrdenante");
                var filtromonto = filters.FirstOrDefault(x => x.Key == "monto");
                var filtroestatusAutorizacion = filters.FirstOrDefault(x => x.Key == "estatusAutorizacion");
                var filtroestatusTransaccion = filters.FirstOrDefault(x => x.Key == "estatusTransaccion");

                if (filtroCuentaOrdenante!.Value != null)
                {
                    uri += string.Format("&cuentaOrdenante={0}", Convert.ToString(filtroCuentaOrdenante.Value));
                }

                if (filtroclaveRastreo!.Value != null)
                {
                    uri += string.Format("&claveRastreo={0}", Convert.ToString(filtroclaveRastreo.Value));
                }

                if (filtronombreOrdenante!.Value != null)
                {
                    uri += string.Format("&nombreOrdenante={0}", Convert.ToString(filtronombreOrdenante.Value));
                }

                if (filtromonto!.Value != null)
                {
                    uri += string.Format("&monto={0}", Convert.ToString(filtromonto.Value));
                }

                if (filtroestatusAutorizacion!.Value != null)
                {
                    uri += string.Format("&estatusAutizacion={0}", Convert.ToString(filtroestatusAutorizacion.Value));
                }

                if (filtroestatusTransaccion!.Value != null)
                {
                    uri += string.Format("&estatusTransaccion={0}", Convert.ToString(filtroestatusTransaccion.Value));
                }

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

                    responseConsulta = JsonConvert.DeserializeObject<ResponseSpeiINConsulta>(jsonResponse, settings);

                    ListaTransacciones = responseConsulta.listaResultado;
                    paginacion = Convert.ToInt32(responseConsulta.cantidadEncontrada);

                    return (ListaTransacciones, paginacion)!;
                }

            }
            catch (Exception)
            {
                return (ListaTransacciones, paginacion)!;
            }

            return (ListaTransacciones, paginacion);
        }

        public async Task<MensajeResponse> PatchAutorizadorSpeiInAsync(string claveRastreo, bool rechazar)
        {
            MensajeResponse respuesta = new MensajeResponse();
            string payload = "";
            try
            {
                var userResponse = _userContextService.GetLoginResponse();
                var uri = Urls.Abonos + UrlsConfig.AbonosOperations.PatchAutorizadorSpeiIn(claveRastreo);
                var accessToken = userResponse.Token;

                _apiClient.DefaultRequestHeaders.Clear();
                _apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                if (rechazar == true)
                {
                    payload = "rechazar=1";
                } 

                var response = await _apiClient.PatchAsync(uri, new StringContent(payload));

                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    respuesta = JsonConvert.DeserializeObject<MensajeResponse>(jsonResponse);
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        respuesta.Error = true;
                        respuesta.message = "No se encontró la clave de rastreo";
                    }
                    else
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<MensajeResponse>(jsonResponse);
                    }
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

