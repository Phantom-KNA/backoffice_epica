using Epica.Web.Operacion.Config;
using Epica.Web.Operacion.Services.UserResolver;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Services;

public class ApiClientBase
{
    protected readonly HttpClient ApiClient;
    protected readonly ILogger Logger;
    protected readonly UrlsConfig Urls;
    protected readonly IHttpContextAccessor HttpContext;
    protected readonly JsonSerializerOptions SerializerOptions;
    protected readonly IUserResolver UserResolver;
    protected readonly string _apiKey;

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
        _apiKey = configuration.GetValue<string>("credential:privatekey");
        ApiClient.DefaultRequestHeaders.Add("Private-Merchant-Id", _apiKey);
        //ApiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserResolver.GetToken());
        //ApiClient.DefaultRequestHeaders.Add("x-api-key", userResolver.GetToken());
    }
}