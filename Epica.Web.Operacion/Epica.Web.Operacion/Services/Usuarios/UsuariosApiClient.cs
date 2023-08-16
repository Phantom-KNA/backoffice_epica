using Epica.Web.Operacion.Config;
using Epica.Web.Operacion.Models.Entities;
using Epica.Web.Operacion.Models.Request;
using Epica.Web.Operacion.Services.UserResolver;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using static Epica.Web.Operacion.Controllers.CuentaController;

namespace Epica.Web.Operacion.Services.Transaccion
{
    public class UsuariosApiClient : ApiClientBase, IUsuariosApiClient
    {
        public UsuariosApiClient(
            HttpClient httpClient, 
            ILogger<CuentaApiClient> logger, 
            IOptions<UrlsConfig> config, 
            IHttpContextAccessor httpContext, 
            IConfiguration configuration, 
            IUserResolver userResolver) : base(httpClient, logger, config, httpContext, configuration, userResolver)
        {
        }

        public async Task<List<UserResponse>> GetUsuariosAsync(int pageNumber, int recordsTotal)
        {

            List<UserResponse>? ListaUsuarios = new List<UserResponse>();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.UsuariosOperations.GetUsuarioInfo(pageNumber, recordsTotal);
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    ListaUsuarios = JsonConvert.DeserializeObject<List<UserResponse>>(jsonResponse);
                }

            } catch (Exception ex) {
                return ListaUsuarios;
            }

            return ListaUsuarios;
        }

        public async Task<int> GetTotalUsuariosAsync()
        {
            int result = 0;
            try
            {
                var uri = Urls.Transaccion + UrlsConfig.UsuariosOperations.GetUsuariosTotal();
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

        public async Task<UserResponse> GetUsuarioAsync(int id)
        {
            UserResponse? Usuario = new UserResponse();
            try
            {
                var uri = Urls.Transaccion + UrlsConfig.UsuariosOperations.GetUsuario(id);
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    Usuario = JsonConvert.DeserializeObject<UserResponse>(jsonResponse);
                }

            }
            catch (Exception)
            {
                return Usuario;
            }

            return Usuario;
        }

        public async Task<List<DocumentosUserResponse>> GetDocumentosUsuarioAsync(int id)
        {
            List<DocumentosUserResponse>? ListaDocumentosUsuario = new List<DocumentosUserResponse>();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.UsuariosOperations.GetUsuarioDocumentos(id);
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    ListaDocumentosUsuario = JsonConvert.DeserializeObject<List<DocumentosUserResponse>>(jsonResponse);
                }

            }
            catch (Exception)
            {
                return ListaDocumentosUsuario;
            }

            return ListaDocumentosUsuario;
        }

        public async Task<BloqueoWebResponse> GetBloqueoWeb(BloqueoWebUsuarioRequest request)
        {
            BloqueoWebResponse respuesta = new BloqueoWebResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.UsuariosOperations.GetBloqueaWebUsuario();
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await ApiClient.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    respuesta = JsonConvert.DeserializeObject<BloqueoWebResponse>(jsonResponse);
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

        public async Task<BloqueoTotalResponse> GetBloqueoTotal(BloqueoTotalUsuarioRequest request)
        {
            BloqueoTotalResponse respuesta = new BloqueoTotalResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.UsuariosOperations.GetBloqueaTotalUsuario();
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await ApiClient.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    respuesta = JsonConvert.DeserializeObject<BloqueoTotalResponse>(jsonResponse);
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

        public async Task<RegistrarModificarClienteResponse> GetRegistroCliente(RegistroModificacionClienteRequest request)
        {
            RegistrarModificarClienteResponse respuesta = new RegistrarModificarClienteResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.UsuariosOperations.InsertarUsuarioNuevo();
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await ApiClient.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    respuesta = JsonConvert.DeserializeObject<RegistrarModificarClienteResponse>(jsonResponse);
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

        public async Task<UserDetailsResponse> GetDetallesCliente(int id)
        {
            UserDetailsResponse? Usuario = new UserDetailsResponse();
            try
            {
                var uri = Urls.Transaccion + UrlsConfig.UsuariosOperations.GetUsuario(id);
                var response = await ApiClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    Usuario = JsonConvert.DeserializeObject<UserDetailsResponse>(jsonResponse);
                }

            }
            catch (Exception)
            {
                return Usuario;
            }

            return Usuario;
        }
    }
}
