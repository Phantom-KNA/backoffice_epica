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
using System.Text.Json;
using static Epica.Web.Operacion.Controllers.CuentaController;

namespace Epica.Web.Operacion.Services.Transaccion
{
    public class CuentaApiClient : ApiClientBase, ICuentaApiClient
    {
        private readonly UserContextService _userContextService;
        public CuentaApiClient(
            HttpClient httpClient, 
            ILogger<CuentaApiClient> logger, 
            IOptions<UrlsConfig> config, 
            IHttpContextAccessor httpContext,
            UserContextService userContextService,
            IConfiguration configuration, 
            IUserResolver userResolver) : base(httpClient, logger, config, httpContext, configuration, userResolver)
        {
            _userContextService = userContextService;
        }

        public async Task<List<ResumenCuentasResponse>> GetListaByNumeroCuentaAsync(string noCuenta)
        {
            List<ResumenCuentasResponse> listaCuentas = new List<ResumenCuentasResponse>();
            try
            {
                var uri = Urls.Transaccion + UrlsConfig.CuentasOperations.GetListaByNumeroCuenta(noCuenta);
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    listaCuentas = JsonConvert.DeserializeObject<List<ResumenCuentasResponse>>(jsonResponse);
                }

            }
            catch (Exception ex)
            {
                return listaCuentas;
            }

            return listaCuentas;
        }
        public async Task<(List<CuentasResponse>, int)> GetCuentasAsync(int pageNumber, int recordsTotal, int columna, bool ascendente)
        {

            List<CuentasResponse>? ListaCuentas = new List<CuentasResponse>();
            int paginacion = 0;

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.CuentasOperations.GetCuentas(pageNumber, recordsTotal);
                if (columna != 0)
                {
                    uri += string.Format("&columna={0}&ascendente={1}", Convert.ToString(columna), ascendente);
                }
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    ListaCuentas = JsonConvert.DeserializeObject<List<CuentasResponse>>(jsonResponse,settings);
                    paginacion = int.Parse(response.Headers.GetValues("x-totalRecord").FirstOrDefault()!.ToString());
                    return (ListaCuentas, paginacion)!;
                }

            }
            catch (Exception ex)
            {
                return (ListaCuentas, paginacion)!; ;
            }

            return (ListaCuentas, paginacion);
        }
        public async Task<(List<CuentasResponse>, int)> GetCuentasFilterAsync(int pageNumber, int recordsTotal, int columna, bool ascendente, List<RequestListFilters> filters)
        {

            List<CuentasResponse>? ListaCuentas = new List<CuentasResponse>();
            FiltroDatosResponseCuenta filtroRespuesta = new FiltroDatosResponseCuenta();
            int paginacion = 0;

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.CuentasOperations.GetCuentasFiltroInfo(pageNumber, recordsTotal);

                var filtroClabe = filters.FirstOrDefault(x => x.Key == "cuentaClabe");
                var filtroNoCuenta = filters.FirstOrDefault(x => x.Key == "noCuenta");
                var filtroNombreCliente = filters.FirstOrDefault(x => x.Key == "nombreCliente");
                var filtroSaldo = filters.FirstOrDefault(x => x.Key == "saldo");
                var filtroTipoPersona = filters.FirstOrDefault(x => x.Key == "tipoPersona");
                var filtroEstatusCuenta = filters.FirstOrDefault(x => x.Key == "estatusCuenta");
                var filtroEstatusSpeiOut = filters.FirstOrDefault(x => x.Key == "estatusSpeiOut");

                if (filtroNombreCliente!.Value != null)
                {
                    uri += string.Format("&nombre={0}", Convert.ToString(filtroNombreCliente.Value));
                }

                if (filtroClabe!.Value != null)
                {
                    uri += string.Format("&cuentaClabe={0}", Convert.ToString(filtroClabe.Value));
                }

                if (filtroNoCuenta!.Value != null)
                {
                    uri += string.Format("&numeroCuenta={0}", Convert.ToString(filtroNoCuenta.Value));
                }

                if (filtroNombreCliente!.Value != null)
                {
                    uri += string.Format("&nombrePersona={0}", Convert.ToString(filtroNombreCliente.Value));
                }

                if (filtroSaldo!.Value != null)
                {
                    uri += string.Format("&saldo={0}", Convert.ToString(filtroSaldo.Value));
                }

                if (filtroTipoPersona!.Value != null)
                {
                    uri += string.Format("&tipoPersona={0}", Convert.ToString(filtroTipoPersona.Value));
                }

                if (filtroEstatusCuenta!.Value != null)
                {
                    uri += string.Format("&estatusCuenta={0}", Convert.ToString(filtroEstatusCuenta.Value));
                }

                if (filtroEstatusSpeiOut!.Value != null)
                {
                    uri += string.Format("&estatusSpei={0}", Convert.ToString(filtroEstatusSpeiOut.Value));
                }

                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    filtroRespuesta = JsonConvert.DeserializeObject<FiltroDatosResponseCuenta>(jsonResponse, settings);

                    ListaCuentas = filtroRespuesta.listaResultado;
                    paginacion = Convert.ToInt32(filtroRespuesta.cantidadEncontrada);
                    return (ListaCuentas, paginacion)!;
                }

            }
            catch (Exception ex)
            {
                return (ListaCuentas, paginacion)!; ;
            }

            return (ListaCuentas, paginacion);
        }

        public async Task<int> GetTotalCuentasAsync()
        {
            int result = 0;
            try
            {
                var uri = Urls.Transaccion + UrlsConfig.CuentasOperations.GetCuentasTotal();
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

        public async Task<List<CuentasResponse>> GetCuentasByClienteAsync(int idCliente)
        {

            List<CuentasResponse>? ListaCuentas = new List<CuentasResponse>();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.CuentasOperations.GetCuentasClientes(idCliente);
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

        public async Task<List<DatosCuentaResponse>> GetDetalleCuentasAsync(string NumCuenta)
        {

            List<DatosCuentaResponse>? ListaCuentas = new List<DatosCuentaResponse>();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.CuentasOperations.GetCuentaDetalle(NumCuenta);
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    ListaCuentas = JsonConvert.DeserializeObject<List<DatosCuentaResponse>>(jsonResponse);
                }

            }
            catch (Exception ex)
            {
                return ListaCuentas;
            }

            return ListaCuentas;
        }

        public async Task<List<CuentaEspecificaResponse>> GetDetalleCuentasSinAsignarAsync(string NumCuenta)
        {

            List<CuentaEspecificaResponse>? getcuenta = new List<CuentaEspecificaResponse>();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.CuentasOperations.GetCuentaDetalleSinAsignar(NumCuenta);
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    getcuenta = JsonConvert.DeserializeObject<List<CuentaEspecificaResponse>>(jsonResponse);
                }

            }
            catch (Exception ex)
            {
                return getcuenta;
            }

            return getcuenta;
        }
        public async Task<List<CuentasResponse>> GetCuentasFiltroAsync(string Valor, int TipoFiltro)
        {

            List<CuentasResponse>? ListaCuentas = new List<CuentasResponse>();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.CuentasOperations.GetCuentasFiltrado(Valor, TipoFiltro);
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
        public async Task<MensajeResponse> BloqueoCuentaAsync(int idCuenta, int estatus, int nip, string softoken)
        {
            MensajeResponse respuesta = new MensajeResponse();

            try
            {
                var responseToken = _userContextService.GetLoginResponse();
                var uri = Urls.Transaccion + UrlsConfig.CuentasOperations.GetBloqueaCuenta(idCuenta, estatus,nip,softoken, responseToken.Token);
                var json = JsonConvert.SerializeObject("");
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
        public async Task<MensajeResponse> BloqueoCuentaSpeiOutAsync(int idCuenta, int estatus, int nip, string softoken)
        {
            MensajeResponse respuesta = new MensajeResponse();

            try
            {
                var responseToken = _userContextService.GetLoginResponse();
                var uri = Urls.Transaccion + UrlsConfig.CuentasOperations.GetBloqueaCuentaSpeiOut(idCuenta, estatus, nip, softoken, responseToken.Token);
                var json = JsonConvert.SerializeObject("");
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
    }
}
