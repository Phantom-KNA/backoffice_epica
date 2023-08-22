using Epica.Web.Operacion.Config;
using Epica.Web.Operacion.Services.Transaccion;
using Epica.Web.Operacion.Services.UserResolver;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Epica.Web.Operacion.Services.Usuarios
{
    public class UsuariosApiClient : ApiClientBase, IUsuariosApiClient
    {
        public UsuariosApiClient(
            HttpClient httpClient,
            ILogger<UsuariosApiClient> logger,
            IOptions<UrlsConfig> config,
            IHttpContextAccessor httpContext,
            IConfiguration configuration,
            IUserResolver userResolver) : base(httpClient, logger, config, httpContext, configuration, userResolver)
        {
        }

        public async Task<List<LoginResponse>> GetUsuariosRolesAsync()
        {

            List<LoginResponse>? listaUsuarios = new List<LoginResponse>();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.UsuariosOperations.GetUsuariosRoles();
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    listaUsuarios = JsonConvert.DeserializeObject<List<LoginResponse>>(jsonResponse);
                }

            }
            catch (Exception ex)
            {
                return listaUsuarios;
            }

            return listaUsuarios;
        }

        public async Task<List<UserRolPermisosResponse>> GetUsuariosRolesVistaAsync()
        {

            List<UserRolPermisosResponse>? listaUsuarios = new List<UserRolPermisosResponse>();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.UsuariosOperations.GetUsuariosRoles();
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    listaUsuarios = JsonConvert.DeserializeObject<List<UserRolPermisosResponse>>(jsonResponse);
                }

            }
            catch (Exception ex)
            {
                return listaUsuarios;
            }

            return listaUsuarios;
        }
    }
}
