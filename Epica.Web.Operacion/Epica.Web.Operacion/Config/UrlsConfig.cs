namespace Epica.Web.Operacion.Config;

public class UrlsConfig
{

    public class AuthenticateOperations
    {
        public static string PostToken() => $"/api/v1/authenticate/token";
    }

    public class TransaccionesOperations
    {
        public static string GetTransacciones() => $"/api/resumentransaccion";
    }

    public string Authenticate { get; set; }
    public string Transaccion { get; set; }
}