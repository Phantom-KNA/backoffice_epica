namespace Epica.Web.Operacion.Config;

public class UrlsConfig
{

    public class AuthenticateOperations
    {
        public static string PostToken() => $"/api/v1/authenticate/token";
    }

    public string Authenticate { get; set; }
}