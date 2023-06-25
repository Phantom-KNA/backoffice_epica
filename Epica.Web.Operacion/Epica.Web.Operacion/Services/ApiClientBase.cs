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
        ApiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserResolver.GetToken());
    }
}