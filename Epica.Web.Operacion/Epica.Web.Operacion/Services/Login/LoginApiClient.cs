﻿using Epica.Web.Operacion.Config;
using Epica.Web.Operacion.Helpers;
using Epica.Web.Operacion.Models.Request;
using Epica.Web.Operacion.Services.Transaccion;
using Epica.Web.Operacion.Services.UserResolver;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Services.Login
{
    public class LoginApiClient: ApiClientBase, ILoginApiClient
    {
        public LoginApiClient(
            HttpClient httpClient,
            ILogger<LoginApiClient> logger,
            IOptions<UrlsConfig> config,
            IHttpContextAccessor httpContext,
            IConfiguration configuration,
            IUserResolver userResolver) : base(httpClient, logger, config, httpContext, configuration, userResolver)
        {
        }

        public async Task<LoginResponse> GetCredentialsAsync(LoginRequest loginRequest, UserContextService userContextService)
        {
            LoginResponse loginResponse = new LoginResponse();

            try
            {
                var uri = Urls.Transaccion + UrlsConfig.LoginOperations.GetCredentials();
                var jsonRequest = JsonConvert.SerializeObject(loginRequest);
                var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                var response = await ApiClient.PostAsync(uri, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    loginResponse = JsonConvert.DeserializeObject<LoginResponse>(jsonResponse);

                    loginResponse.IsAuthenticated = true;
                    loginResponse.DireccionIp = loginRequest.Ip;
                    loginResponse.NombreDispositivo = loginRequest.DispositivoAcceso;

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, loginResponse.Usuario)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, "EpicaWebEsquema"); 

                    await HttpContext.HttpContext.SignInAsync("EpicaWebEsquema", new ClaimsPrincipal(claimsIdentity));
                    userContextService.SetLoginResponse(loginResponse);
                    try
                    {
                        var token = await GenTokenAsync();
                        userContextService.SetTokenResponse(token);
                    }
                    catch(Exception)
                    {

                    }

                }
                else
                {
                    loginResponse.IsAuthenticated = false;

                }

            }
            catch (Exception)
            {
                loginResponse.IsAuthenticated = false;

            }

            return loginResponse;
        }

        public async Task<TokenResponse> GenTokenAsync()
        {
            var uri = UrlApi + UrlsConfig.AuthenticateOperations.PostToken();

            var credentials = new TokenRequest() { Username = UsernameApi, Password = PasswordApi, IdDispositivo = "", ModeloDispositivo = "" };

            var content = new StringContent(JsonConvert.SerializeObject(credentials), Encoding.UTF8, "application/json");
            JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            var response = await ApiClient.PostAsync(uri, content);

            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<TokenResponse>(stringResponse, _serializerOptions);

            return result;
        }
        public async Task LogoutAsync(HttpContext httpContext)
        {
            await httpContext.SignOutAsync("EpicaWebEsquema");
            httpContext.Session.Clear();
            httpContext.Response.Clear();
        }
    }
}
