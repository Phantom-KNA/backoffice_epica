using Epica.Web.Operacion.Config;
using Epica.Web.Operacion.Services.Transaccion;
using Epica.Web.Operacion.Services.UserResolver;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Epica.Web.Operacion.Services.Catalogos
{
    public class CatalogosApiClient: ApiClientBase, ICatalogosApiClient
    {
        public CatalogosApiClient(
        HttpClient httpClient,
        ILogger<CuentaApiClient> logger,
        IOptions<UrlsConfig> config,
        IHttpContextAccessor httpContext,
        IConfiguration configuration,
        IUserResolver userResolver) : base(httpClient, logger, config, httpContext, configuration, userResolver)
        {
        }

        public async Task<List<DatosCatalogoResponse>> GetMediosPagoAsync()
        {

            List<DatosCatalogoResponse>? listaMediosPago = new List<DatosCatalogoResponse>();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.CatalagosOperations.GetMediosPago();
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    listaMediosPago = JsonConvert.DeserializeObject<List<DatosCatalogoResponse>>(jsonResponse);
                }

            }
            catch (Exception ex)
            {
                return listaMediosPago;
            }

            return listaMediosPago;
        }

        public async Task<List<DatosCatalogoResponse>> GetEmpresasAsync()
        {

            List<DatosCatalogoResponse>? listaEmpresas = new List<DatosCatalogoResponse>();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.CatalagosOperations.GetEmpresas();
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    listaEmpresas = JsonConvert.DeserializeObject<List<DatosCatalogoResponse>>(jsonResponse);
                }

            }
            catch (Exception ex)
            {
                return listaEmpresas;
            }

            return listaEmpresas;
        }

        public async Task<List<DatosCatalogoResponse>> GetPaisesAsync()
        {

            List<DatosCatalogoResponse>? listaPaises = new List<DatosCatalogoResponse>();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.CatalagosOperations.GetPaises();
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    listaPaises = JsonConvert.DeserializeObject<List<DatosCatalogoResponse>>(jsonResponse);
                }

            }
            catch (Exception ex)
            {
                return listaPaises;
            }

            return listaPaises;
        }

        public async Task<List<DatosCatalogoResponse>> GetOcupacionesAsync()
        {

            List<DatosCatalogoResponse>? listaOcupaciones = new List<DatosCatalogoResponse>();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.CatalagosOperations.GetOcupaciones();
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    listaOcupaciones = JsonConvert.DeserializeObject<List<DatosCatalogoResponse>>(jsonResponse);
                }

            }
            catch (Exception ex)
            {
                return listaOcupaciones;
            }

            return listaOcupaciones;
        }

        public async Task<List<DatosCatalogoResponse>> GetNacionalidadesAsync()
        {

            List<DatosCatalogoResponse>? listaNacionalidades = new List<DatosCatalogoResponse>();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.CatalagosOperations.GetOcupaciones();
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    listaNacionalidades = JsonConvert.DeserializeObject<List<DatosCatalogoResponse>>(jsonResponse);
                }

            }
            catch (Exception ex)
            {
                return listaNacionalidades;
            }

            return listaNacionalidades;
        }
    }
}
