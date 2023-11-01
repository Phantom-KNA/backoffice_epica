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
    public class PersonaMoralServices : ApiClientBase, IPersonaMoralServices
    {
        private readonly UserContextService _userContextService;

        public PersonaMoralServices(
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

        public async Task<(List<DatosClienteMoralResponse>, int)> GetPersonasMoralesAsync(int pageNumber, int recordsTotal, int columna, bool ascendente)
        {

            List<DatosClienteMoralResponse>? listaClientes = new List<DatosClienteMoralResponse>();
            int paginacion = 0;

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.PersonaMoralOperations.GetPersonasMoralesInfo(pageNumber, recordsTotal);

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
                    listaClientes = JsonConvert.DeserializeObject<List<DatosClienteMoralResponse>>(jsonResponse, settings);
                    paginacion = int.Parse(response.Headers.GetValues("x-totalRecord").FirstOrDefault()!.ToString());
                    return (listaClientes, paginacion)!;
                }

            } catch (Exception) {
                return (listaClientes, paginacion)!; ;
            }

            return (listaClientes, paginacion);
        }

        public async Task<DatosClienteMoralResponse> GetDetallesPersonaMoral(int id)
        {
            DatosClienteMoralResponse? cliente = new DatosClienteMoralResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.PersonaMoralOperations.GetPersonaMoral(id);
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
                    cliente = JsonConvert.DeserializeObject<DatosClienteMoralResponse>(jsonResponse, settings);
                }

            }
            catch (Exception ex)
            {
                return cliente!;
            }

            return cliente!;
        }

    }
}
