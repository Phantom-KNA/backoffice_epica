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
            //URL de pruebas
            HttpClient ApiClient = new HttpClient();
            List<UserResponse>? ListaUsuarios = new List<UserResponse>();

            try
            {
                
                ApiClient.BaseAddress = new Uri("http://20.225.114.135/");
                ApiClient.DefaultRequestHeaders.Add("Api-Key", "tmfiiA3sCEe9Ybf4GL5D8gqlN0BOtWakmgvD1yHF6BhA");
                var parametros = string.Format("api/v1/usuarios/usuario_info?pageNumber={0}&pageSize={1}", pageNumber,recordsTotal);
                var url = "http://20.225.114.135/" + parametros;
                var response = await ApiClient.GetAsync(url);

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
    }
}
