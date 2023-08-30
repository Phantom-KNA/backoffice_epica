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
        public static string RestableceContraseñaCorreo(string Correo) => $"/api/v1/recuperapassword/email?email={Correo}";
        public static string RestableceContraseñaTelefono(string Telefono) => $"/api/v1/recuperapassword/sms?telefono={Telefono}";
    }

    public class TransaccionesOperations
    {
        public static string GetTransacciones(int numberPage, int TotalRecords) => $"/api/v1/transacciones/movimientos_cta?pageNumber={numberPage}&pageSize={TotalRecords}";
        public static string GetTransaccion(int idInterno) => $"/api/resumentransaccion?idInterno={idInterno}";
        public static string GetTransaccionesTotal() => $"/api/v1/transacciones/total";
        public static string GetTransaccionesPorCuenta(int idCuenta) => $"/api/v1/cuentas/cuenta_trasacciones?idCuenta={idCuenta}";
        public static string GetTransaccionesPorCliente(int idCliente) => $"/api/v1/transacciones/movimientos_cliente?idCliente={idCliente}";
        public static string GetRegistraTransaccion() => $"/api/v1/transacciones/insertar";
        public static string ModificarTransaccion() => $"/api/v1/transacciones/modificar";
        public static string DetalleTransaccion(int idTransaccion) => $"/api/v1/transacciones/detalle_movimiento?idtrasaccion={idTransaccion}";
        public static string DetalleTransaccionClaveCobranza(string claveCobranza) => $"/api/v1/transacciones/buscar_claverastro_cobranza?calveRastreoCobranza={claveCobranza}";
    }

    public class CuentasOperations
    {
        public static string GetCuentas(int numberPage, int TotalRecords) => $"/api/v1/cuentas/cliente_vista?pageNumber={numberPage}&pageSize={TotalRecords}";
        public static string GetCuentasTotal() => $"/api/v1/cuentas/total";
        public static string GetCobranzaReferenciada(int id) => $"/api/v1/transacciones/cobranza_referenciada?idCuenta={id}";
        public static string GetCuentasClientes(int id) => $"/api/v1/cuentas/cliente_cuentas?idCliente={id}";
        public static string GetCuentaDetalle(string NoCuenta) => $"/api/v1/cuentas/buscar_cuenta?noCuenta={NoCuenta}";
        public static string GetCuentaDetalleSinAsignar(string NoCuenta) => $"/api/v1/catalogos/buscar_cuenta_asignar?noCuenta={NoCuenta}";
    }

    public class ClientesOperations
    {
        public static string GetDetallesClientesByNombres(string NombresApellidos) => $"/api/v1/clientes/buscar_concidencia_nombres?NombresApellidos={NombresApellidos}";
        public static string GetClientesByNombre(string nombre) => $"/api/v1/clientes/buscar_clientes?nombre={nombre}";
        public static string GetClientesTotal() => $"/api/v1/clientes/total";
        public static string GetCliente(int id) => $"/api/v1/clientes/cliente?id={id}";
        public static string GetClienteDocumentos(int id) => $"/api/v1/clientes/cliente_documentos?idUsuario={id}";
        public static string GetClienteInfo(int pageNumber, int totalRecords) => $"/api/v1/clientes/clientes_info?pageNumber={pageNumber}&pageSize={totalRecords}";
        public static string GetDocumentosCliente(int idUsuario) => $"/api/v1/clientes/cliente_documento?idUsuario={idUsuario}";
        public static string GetBloqueaWebCliente() => $"/api/v1/clientes/bloque_web";
        public static string GetBloqueaTotalCliente() => $"/api/v1/clientes/bloque_total";
        public static string InsertarClienteNuevo() => $"/api/v1/clientes/inserta_cliente";
        public static string ModificarCliente() => $"/api/v1/clientes/modificar_cliente";
        public static string GetAllClientes() => $"api/v1/clientes/AllClientes";
        public static string ModificarClienteNuevo() => $"/api/v1/clientes/modificar_cliente";
        public static string AsignarCuentaCliente() => $"/api/v1/clientes/asignar_cuenta";
        public static string DesvincularCuentaCliente() => $"/api/v1/clientes/desasociar_cuenta";
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

    public class UsuariosOperations
    {
        public static string GetUsuariosRoles() => $"/api/v1/usuario/resumen_permisos";
        public static string GetUsuariosVista() => $"/api/v1/usuario/resumen_permisos";
    }
    public string users { get; set; }
    public string Authenticate { get; set; }
    public string Transaccion { get; set; }
}