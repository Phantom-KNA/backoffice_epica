using Epica.Web.Operacion.Config;
using Epica.Web.Operacion.Services.UserResolver;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Services;

public class ApiClientBase
{
    protected readonly HttpClient ApiClient;
    //protected readonly HttpClient ApiClientUser;
    protected readonly ILogger Logger;
    protected readonly UrlsConfig Urls;
    protected readonly IHttpContextAccessor HttpContext;
    protected readonly JsonSerializerOptions SerializerOptions;
    protected readonly IUserResolver UserResolver;
    protected readonly string? _apiKey;
    protected readonly string? UsernameApi;
    protected readonly string? PasswordApi;
    protected readonly string? UrlApi;
    public ApiClientBase(HttpClient httpClient,
    ILogger logger,
    IOptions<UrlsConfig> config,
    IHttpContextAccessor httpContext,
    IConfiguration configuration,
    IUserResolver userResolver)
    {
        ApiClient = httpClient;
        Logger = logger;
        Urls = config.Value;
        HttpContext = httpContext;
        SerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        UserResolver = userResolver;
        _apiKey = configuration.GetValue<string>("credential:ApiKey");
        ApiClient.DefaultRequestHeaders.Add("Api-Key", _apiKey);
        UsernameApi = configuration.GetValue<string>("CredentialsApi:Username");
        PasswordApi = configuration.GetValue<string>("CredentialsApi:Password");
        UrlApi = configuration.GetValue<string>("CredentialsApi:Url");
    }
}