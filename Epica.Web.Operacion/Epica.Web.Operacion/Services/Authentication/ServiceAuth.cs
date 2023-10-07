

using Epica.Web.Operacion.Config;
using Epica.Web.Operacion.Helpers;
using Epica.Web.Operacion.Models.Entities;
using Epica.Web.Operacion.Models.Request;
using Epica.Web.Operacion.Services.Authentication;
using Epica.Web.Operacion.Services.UserResolver;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Epica.Web.Operacion.Services
{
    public class ServiceAuth : ApiClientBase, IServiceAuth
    {
        private readonly UserContextService _userContextService;
        protected readonly HttpClient _apiClient;
        protected readonly ILogger _logger;
        protected readonly UrlsConfig _urls;
        protected readonly IHttpContextAccessor _httpContext;
        protected readonly JsonSerializerOptions _serializerOptions;
        protected readonly IConfiguration _configuration;

        public ServiceAuth(
            HttpClient httpClient,
        ILogger<ServiceAuth> logger,
        IOptions<UrlsConfig> config,
        IHttpContextAccessor httpContext,
        IConfiguration configuration,
        IUserResolver userResolver,
            UserContextService userContextService): base(httpClient, logger, config, httpContext, configuration, userResolver)
        {
            _userContextService = userContextService;
            _apiClient = httpClient;
            _logger = logger;
            _urls = config.Value;
            _httpContext = httpContext;
            _configuration = configuration;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }
        public async Task<TokenResponse> Auth(string username, string password)
        {
            var uri = _urls.Authenticate + UrlsConfig.AuthenticateOperations.PostToken();

            var credentials = new TokenRequest() { Username = username, Password = password };

            var content = new StringContent(JsonConvert.SerializeObject(credentials), Encoding.UTF8, "application/json");
            var response = await _apiClient.PostAsync(uri, content);

            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<TokenResponse>(stringResponse, _serializerOptions);

            return result;
        }

        /// <summary>
        /// Creacion del token del sitio web.
        /// </summary>
        /// <param name="autenticacion">Token que se creo ala api.</param>
        /// <param name="configuration">Configuraciones del appsettings json.</param>
        /// <returns></returns>
        public string CreacionTokenWebSite(IConfiguration configuration, string token)
        {
            return GenerarToken(new GenericToken
            {
                Issuer = configuration["JWT:Issuer"],
                Audience = configuration["JWT:Audience"],
                KeySecret = configuration["JWT:ClaveSecreta"],
                TimeExpire = int.Parse(configuration["TiempoExpiracionSesion"]),
                Claims = new List<Claim> {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("TokenWebApp", token),
                }
            });
        }

        /// <summary>
        /// Método generico de la creacion del token.
        /// </summary>
        /// <param name="genericToken">Entidad de la sesion.</param>
        /// <returns></returns>
        private string GenerarToken(GenericToken genericToken)
        {
            var _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(genericToken.KeySecret));

            var _signingCredentials = new SigningCredentials(
                    _symmetricSecurityKey, SecurityAlgorithms.HmacSha256
                );
            var _Header = new JwtHeader(_signingCredentials);

            var _Payload = new JwtPayload(
                    issuer: genericToken.Issuer,
                    audience: genericToken.Audience,
                    claims: genericToken.Claims,
                    notBefore: DateTime.UtcNow,
                    expires: DateTime.UtcNow.AddMinutes(genericToken.TimeExpire)
            );

            var _Token = new JwtSecurityToken(
                    _Header,
                    _Payload
                    );

            return new JwtSecurityTokenHandler().WriteToken(_Token);
        }

        public async Task<MensajeResponse> GetVerificarAccesoAsync(VerificarAccesoRequest request)
        {
            MensajeResponse respuesta = new MensajeResponse();

            try
            {
                var userResponse = _userContextService.GetLoginResponse();
                request.IdCliente = userResponse.IdUsuario.ToString();
                var uri = UrlApi + UrlsConfig.AuthenticateOperations.VerificarAcceso();
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var accessToken = userResponse.Token;
                _apiClient.DefaultRequestHeaders.Clear();
                _apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

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