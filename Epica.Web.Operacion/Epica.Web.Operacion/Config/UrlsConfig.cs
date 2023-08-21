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
        public static string GetTransaccionesPorCuenta(int idCuenta) => $"/api/v1/cuentas/cuenta_trasacciones?idCuenta={idCuenta}";
        public static string GetRegistraTransaccion() => $"/api/v1/transacciones/insertar";
    }

    public class CuentasOperations
    {
        public static string GetCuentas(int numberPage, int TotalRecords) => $"/api/v1/cuentas/cliente_vista?pageNumber={numberPage}&pageSize={TotalRecords}";
        public static string GetCuentasTotal() => $"/api/v1/cuentas/total";
        public static string GetCobranzaReferenciada(int id) => $"/api/v1/transacciones/cobranza_referenciada?id={id}";
        public static string GetCuentasClientes(int id) => $"/api/v1/cuentas/cliente_cuentas?idCliente={id}";
    }

    public class ClientesOperations
    {
        public static string GetClientesTotal() => $"/api/v1/clientes/total";
        public static string GetCliente(int id) => $"/api/v1/clientes/cliente?id={id}";
        public static string GetClienteDocumentos(int id) => $"/api/v1/clientes/cliente_documento?idUsuario={id}";
        public static string GetClienteInfo(int pageNumber, int totalRecords) => $"/api/v1/clientes/clientes_info?pageNumber={pageNumber}&pageSize={totalRecords}";
        public static string GetDocumentosCliente(int idUsuario) => $"/api/v1/clientes/cliente_documento?idUsuario={idUsuario}";
        public static string GetBloqueaWebCliente() => $"/api/v1/clientes/bloque_web";
        public static string GetBloqueaTotalCliente() => $"/api/v1/clientes/bloque_total";
        public static string InsertarClienteNuevo() => $"/api/v1/clientes/inserta_cliente";
        public static string ModificarClienteNuevo() => $"/api/v1/clientes/modificar_cliente";
    }

    public class CatalogosOperations
    {
        public static string GetMediosPago() => $"/api/v1/catalogos/medios_pago";
        public static string GetEmpresas() => $"/api/v1/catalogos/empresas";
        public static string GetOcupaciones() => $"/api/v1/catalogos/ocupaciones";
        public static string GetNacionalidades() => $"/api/v1/catalogos/nacionalidades";
        public static string GetPaises() => $"/api/v1/catalogos/paises";
        public static string GetRoles() => $"/api/v1/catalogos/rol_clientes";
    }

    public class TarjetasOperations
    {
        public static string GetTarjetasCliente(int idClientes) => $"/api/v1/tarjetas/clientes_tarjetas?idCliente={idClientes}";
        public static string GetTarjetas(int numberPage, int TotalRecords) => $"/api/v1/tarjetas/view_tarjetas?pageNumber={numberPage}&pageSize={TotalRecords}";
        public static string GetTarjetasRegistra() => $"/api/v1/tarjetas/add_toka";
    }

    public class LoginOperations
    {
        public static string GetCredentials() => $"/api/v1/authentication/authentication";
    }

    public class LogOperations
    {
        public static string InsertarLog() => $"/api/v1/log/insertar_log";
    }
    public string Authenticate { get; set; }
    public string Transaccion { get; set; }
}