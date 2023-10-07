using Epica.Web.Operacion.Config;
using Epica.Web.Operacion.Models.Request;
using Epica.Web.Operacion.Services.Transaccion;
using Epica.Web.Operacion.Services.UserResolver;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

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

        public async Task<List<UserPermisosResponse>> GetRolPermisosEspecificoAsync(int idRol, string moduloAcceso)
        {

            List<UserPermisosResponse>? listaRolEspecifico = new List<UserPermisosResponse>();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.UsuariosOperations.GetUsuariosRolesPorVista(idRol,moduloAcceso);
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    listaRolEspecifico = JsonConvert.DeserializeObject<List<UserPermisosResponse>>(jsonResponse);
                }

            }
            catch (Exception ex)
            {
                return listaRolEspecifico;
            }

            return listaRolEspecifico;
        }

        public async Task<MensajeResponse> GetAsignarRolPermisos(UserPermisoRequest request)
        {
            MensajeResponse respuesta = new MensajeResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.UsuariosOperations.GetAsignarPermisoRolVista();
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
            catch (Exception ex)
            {
                respuesta.Error = true;
                respuesta.Detalle = ex.Message;
                return respuesta;
            }

            return respuesta;
        }

        public async Task<List<UsuariosVinculadosResponse>> GetAsignarRolPermisosAsync()
        {
            List<UsuariosVinculadosResponse> listaUsuarios = new List<UsuariosVinculadosResponse>();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.UsuariosOperations.GetUsuariosVista();
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    listaUsuarios = JsonConvert.DeserializeObject<List<UsuariosVinculadosResponse>>(jsonResponse);
                }

            }
            catch (Exception ex)
            {
                return listaUsuarios;
            }

            return listaUsuarios;
        }

        public async Task<List<DatosCatalogoResponse>> GetUsuarioPorNombreAsync(string nombre)
        {

            List<DatosCatalogoResponse>? getcuenta = new List<DatosCatalogoResponse>();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.UsuariosOperations.GetUsuarioPorNombre(nombre);
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    getcuenta = JsonConvert.DeserializeObject<List<DatosCatalogoResponse>>(jsonResponse);
                }

            }
            catch (Exception ex)
            {
                return getcuenta;
            }

            return getcuenta;
        }

        public async Task<MensajeResponse> GetRegistroAsignacionUsuarioRol(int idRol, int idUsuario)
        {
            MensajeResponse respuesta = new MensajeResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.UsuariosOperations.GetUsuariosAsignarRoles(idRol,idUsuario);
                var json = JsonConvert.SerializeObject("");
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await ApiClient.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    respuesta = JsonConvert.DeserializeObject<MensajeResponse>(jsonResponse);
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

        public async Task<MensajeResponse> GetDesasignacionUsuarioRol(int idUsuario)
        {
            MensajeResponse respuesta = new MensajeResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.UsuariosOperations.GetUsuariosDesasignarRoles(idUsuario);
                var response = await ApiClient.DeleteAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    respuesta = JsonConvert.DeserializeObject<MensajeResponse>(jsonResponse);
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

        public async Task<MensajeResponse> GetCrearRol(string DescripcionRol)
        {
            MensajeResponse respuesta = new MensajeResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.UsuariosOperations.GetInsertarRol(DescripcionRol);
                var json = JsonConvert.SerializeObject("");
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await ApiClient.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    respuesta = JsonConvert.DeserializeObject<MensajeResponse>(jsonResponse);
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
