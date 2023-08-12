using Microsoft.Extensions.Diagnostics.HealthChecks;

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
        public static string GetTransaccionesTotal() => $"/api/v1/transacciones/total";

    }

    public class CuentasOperations
    {
        public static string GetCuentas(int numberPage, int TotalRecords) => $"/api/v1/cuentas/cliente_vista?pageNumber={numberPage}&pageSize={TotalRecords}";
        public static string GetCuentasTotal() => $"/api/v1/cuentas/total";

        public static string GetCobranzaReferenciada(int id) => $"/api/v1/transacciones/cobranza_referenciada?id={id}";
    }

    public class UsuariosOperations
    {
        public static string GetUsuariosTotal() => $"/api/v1/clientes/total";
        public static string GetUsuario(int id) => $"/api/v1/clientes/usuario?id={id}";
        public static string GetUsuarioDocumentos(int id) => $"/api/v1/clientes/usuario_documento?idUsuario={id}";
        public static string GetUsuarioInfo(int pageNumber, int totalRecords) => $"/api/v1/clientes/usuario_info?pageNumber={pageNumber}&pageSize={totalRecords}";
        public static string GetDocumentosUsuario(int idUsuario) => $"/api/v1/clientes/Usuario_documento?idUsuario={idUsuario}";
        public static string GetBloqueaWebUsuario() => $"/api/v1/clientes/bloque_web";
        public static string GetBloqueaTotalUsuario() => $"/api/v1/clientes/bloque_total";
        public static string InsertarUsuarioNuevo() => $"/api/v1/clientes/inserta_cliente";
    }

    public class LoginOperations
    {
        public static string GetCredentials() => $"/api/v1/authentication/authentication";
    }

    public string Authenticate { get; set; }
    public string Transaccion { get; set; }
}