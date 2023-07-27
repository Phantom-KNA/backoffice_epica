namespace Epica.Web.Operacion.Config;

/// <summary>
/// Esta clase  contiene las configuraciones de las URL utilizadas en la operación de autenticación.
/// </summary>
/// 
public class UrlsConfig
{

    public class AuthenticateOperations
    {
        public static string PostToken() => $"/api/v1/authenticate/token";
    }

    public class TransaccionesOperations
    {
        public static string GetTransacciones(int numberPage, int TotalRecords) => $"/api/v1/transacciones/movimientos_cta?pageNumber={numberPage}&pageSize={TotalRecords}";
        public static string GetTransaccion(int idInterno) => $"/api/resumentransaccion?idInterno={idInterno}";
    }

    public string Authenticate { get; set; }
    public string Transaccion { get; set; }
}