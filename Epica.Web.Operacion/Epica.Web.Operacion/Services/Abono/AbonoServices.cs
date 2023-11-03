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

        public async Task<(List<TransaccionesResponse>, int)> GetTransaccionesFilterAsync(int pageNumber, int recordsTotal, int columna, bool ascendente, List<RequestListFilters> filters)
        {
            List<TransaccionesResponse>? ListaTransacciones = new List<TransaccionesResponse>();
            FiltroDatosResponseTransacciones responseConsulta = new FiltroDatosResponseTransacciones();
            int paginacion = 0;

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.TransaccionesOperations.GetTransaccionesFilter(pageNumber, recordsTotal);

                var filtroCuentaOrdenante = filters.FirstOrDefault(x => x.Key == "cuentaOrdenante");
                var filtronombreOrdenante = filters.FirstOrDefault(x => x.Key == "nombreOrdenante");
                var filtronombreBeneficiario = filters.FirstOrDefault(x => x.Key == "nombreBeneficiario");
                var filtroclaveRastreo = filters.FirstOrDefault(x => x.Key == "claveRastreo");
                var filtroEstatus = filters.FirstOrDefault(x => x.Key == "estatus");
                var filtromonto = filters.FirstOrDefault(x => x.Key == "monto");
                var filtrofechaInstruccion = filters.FirstOrDefault(x => x.Key == "fecha_instruccion");
                var filtrofechaAutorizacion = filters.FirstOrDefault(x => x.Key == "fecha_autorizacion");
                var filtroconcepto = filters.FirstOrDefault(x => x.Key == "concepto");


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

                if (filtronombreBeneficiario!.Value != null)
                {
                    uri += string.Format("&nombreBeneficiario={0}", Convert.ToString(filtronombreBeneficiario.Value));
                }

                if (filtroconcepto!.Value != null)
                {
                    uri += string.Format("&concepto={0}", Convert.ToString(filtroconcepto.Value));
                }

                if (filtromonto!.Value != null)
                {
                    uri += string.Format("&monto={0}", Convert.ToString(filtromonto.Value));
                }

                if (filtrofechaInstruccion!.Value != null)
                {
                    uri += string.Format("&fechaInstruccion={0}", Convert.ToString(filtrofechaInstruccion.Value));
                }

                if (filtrofechaAutorizacion!.Value != null)
                {
                    uri += string.Format("&fechaAutorizacion={0}", Convert.ToString(filtrofechaAutorizacion.Value));
                }

                if (filtroEstatus!.Value != null)
                {
                    uri += string.Format("&estatus={0}", Convert.ToString(filtroEstatus.Value));
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

                    responseConsulta = JsonConvert.DeserializeObject<FiltroDatosResponseTransacciones>(jsonResponse, settings);

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

