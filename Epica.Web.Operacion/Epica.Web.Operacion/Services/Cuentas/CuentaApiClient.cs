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
        public async Task<(List<CuentasResponse>, int)> GetCuentasAsync(int pageNumber, int recordsTotal)
        {

            List<CuentasResponse>? ListaCuentas = new List<CuentasResponse>();
            int paginacion = 0;

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.CuentasOperations.GetCuentas(pageNumber, recordsTotal);
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


    }
}
