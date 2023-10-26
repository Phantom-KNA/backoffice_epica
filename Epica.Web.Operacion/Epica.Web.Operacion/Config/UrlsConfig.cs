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
        public static string VerificarAcceso() => $"/api/v1/authenticate/verificaacceso";
    }

    public class TransaccionesOperations
    {
        public static string GetTransaccionRastreoCobranza(string calveRastreoCobranza) => $"/api/v1/transacciones/buscar_claverastro_cobranza?calveRastreoCobranza={calveRastreoCobranza}";
        public static string GetTransacciones(int numberPage, int TotalRecords) => $"/api/v1/transacciones/movimientos_cta?pageNumber={numberPage}&pageSize={TotalRecords}";
        public static string GetTransaccionesFilter(int numberPage, int TotalRecords) => $"/api/v1/transacciones/buscar_filtro?pageNumber={numberPage}&pageSize={TotalRecords}";
        public static string GetTransaccionesMasiva(int numberPage, int TotalRecords, int idUsuario) => $"/api/v1/cargabach/cargar_temp?pagina={numberPage}&registrosPagina={TotalRecords}&idusuario={idUsuario}";
        public static string GetTransaccionesMasivaFilter(int numberPage, int TotalRecords, int idUsuario) => $"/api/v1/cargabach/buscar_filtro?pagina={numberPage}&registrosPagina={TotalRecords}&idusuario={idUsuario}";
        public static string GetTransaccion(int idInterno) => $"/api/resumentransaccion?idInterno={idInterno}";
        public static string GetTransaccionesTotal() => $"/api/v1/transacciones/total";
        public static string GetTransaccionesPorCuenta(int idCuenta) => $"/api/v1/cuentas/cuenta_trasacciones?idCuenta={idCuenta}";
        public static string GetTransaccionesPorCliente(int idCliente) => $"/api/v1/transacciones/movimientos_cliente?idCliente={idCliente}";
        public static string GetRegistraTransaccion() => $"/api/v1/transacciones/insertar";
        public static string ModificarTransaccion() => $"/api/v1/transacciones/modificar";
        public static string DetalleTransaccion(int idTransaccion) => $"/api/v1/transacciones/detalle_movimiento?idtrasaccion={idTransaccion}";
        public static string DetalleTransaccionClaveCobranza(string claveCobranza) => $"/api/v1/transacciones/buscar_claverastro_cobranza?calveRastreoCobranza={claveCobranza}";
        public static string InsertarBatchTransaccion() => $"/api/v1/cargabach/inserta_temp";
        public static string GetEliminarTransaccionBatch(int idRegistro) => $"/api/v1/cargabach/remover_registro{idRegistro}";
        public static string GetTransaccionBatch(int idRegistro) => $"/api/v1/cargabach/buscar_registro?idRegistro={idRegistro}";
        public static string GetTransaccionEditBatch() => $"/api/v1/cargabach/modificar_registro";
        public static string GetProcesarTransaccion(int idUsuario) => $"/api/v1/cargabach/inserta_multiple_transacciones?idUsuario={idUsuario}";
        public static string GetComprobanteTransaccion(int cuentaAhorro, int Transaccion, string valida) => $"/api/v1/transacciones/imprimir_voucher?idCuentaAhorro={cuentaAhorro}&idTransaccion={Transaccion}&valida={valida}";
        public static string GetTransaccionesBatchTotalPorUsuario(int idUsuario) => $"/api/v1/cargabach/total?usuario={idUsuario}";
        public static string GetEliminarTransaccionBatchPorUsuario(int idUsuario, int estatus) => $"/api/v1/cargabach/remover_multiples_resgistro/{idUsuario}/{estatus}";
        public static string GetValidaExistenciaTransaccionBatch(int idUsuario, string claveRastreo) => $"/api/v1/cargabach/validar_existencia?Usuario={idUsuario}&claveRastreo={claveRastreo}";


    }

    public class CuentasOperations
    {
        public static string GetListaByNumeroCuenta(string noCuenta) => $"/api/v1/cuentas/buscar_cuenta?noCuenta={noCuenta}";
        public static string GetCuentas(int numberPage, int TotalRecords) => $"/api/v1/cuentas/cliente_vista?pageNumber={numberPage}&pageSize={TotalRecords}";
        public static string GetCuentasFiltroInfo(int pageNumber, int totalRecords) => $"/api/v1/cuentas/buscar_filtro?pageNumber={pageNumber}&pageSize={totalRecords}";
        public static string GetCuentasFiltrado(string Valor, int tipoFiltro) => $"/api/v1/cuentas/buscar_filtro?datoBuscar={Valor}&tipoFiltro={tipoFiltro}";
        public static string GetCuentasTotal() => $"/api/v1/cuentas/total";
        public static string GetCobranzaReferenciada(int id) => $"/api/v1/transacciones/cobranza_referenciada?idCuenta={id}";
        public static string GetCuentasClientes(int id) => $"/api/v1/cuentas/cliente_cuentas?idCliente={id}";
        public static string GetCuentaDetalle(string NoCuenta) => $"/api/v1/cuentas/buscar_cuenta?noCuenta={NoCuenta}";
        public static string GetCuentaDetalleSinAsignar(string NoCuenta) => $"/api/v1/catalogos/buscar_cuenta_asignar?noCuenta={NoCuenta}";
        public static string GetBloqueaCuenta(int idCuenta, int estatus, int nip, string softoken, string valida) => $"/api/v1/cuentas/bloquear_desbloquear_cuenta?idCuenta={idCuenta}&estatus={estatus}&nip={nip}&softoken={softoken}&valida={valida}";
        public static string GetBloqueaCuentaSpeiOut(int idCuenta, int estatus, int nip, string softoken, string valida) => $"/api/v1/cuentas/bloquear_desbloquear_speiout?idCuenta={idCuenta}&estatus={estatus}&nip={nip}&softoken={softoken}&valida={valida}";


    }

    public class ClientesOperations
    {
        public static string GetDetallesGeneralesCliente(int id) => $"/api/v1/clientes/detalles_cliente?id={id}";
        public static string GetDetallesClientesByNombres(string NombresApellidos) => $"/api/v1/clientes/buscar_concidencia_nombres?NombresApellidos={NombresApellidos}";
        public static string GetClientesByNombre(string nombre) => $"/api/v1/clientes/buscar_clientes?nombre={nombre}";
        public static string GetClientesTotal() => $"/api/v1/clientes/total";
        public static string GetCliente(int id) => $"/api/v1/clientes/cliente?id={id}";
        public static string GetClienteDocumentos(int id) => $"/api/v1/clientes/cliente_documentos?idUsuario={id}";
        public static string GetClienteInfo(int pageNumber, int totalRecords) => $"/api/v1/clientes/clientes_info?pageNumber={pageNumber}&pageSize={totalRecords}";
        public static string GetClienteFiltroInfo(int pageNumber, int totalRecords) => $"/api/v1/clientes/buscar_filtro?pageNumber={pageNumber}&pageSize={totalRecords}";
        public static string GetClienteExiste(int pageNumber, int pageSize, string telefono, string correo) => $"/api/v1/clientes/buscar_filtro?pageNumber={pageNumber}&pageSize={pageSize}&telefono={telefono}&correo={correo}";
        public static string GetDocumentosCliente(int idUsuario) => $"/api/v1/clientes/cliente_documento?idUsuario={idUsuario}";
        public static string GetBloqueaWebCliente() => $"/api/v1/clientes/bloque_web";
        public static string GetBloqueaTotalCliente() => $"/api/v1/clientes/bloque_total";
        public static string InsertarClienteNuevo() => $"/api/v1/clientes/inserta_cliente";
        public static string ModificarCliente() => $"/api/v1/clientes/modificar_cliente";
        public static string GetAllClientes() => $"api/v1/clientes/AllClientes";
        public static string ModificarClienteNuevo() => $"/api/v1/clientes/modificar_cliente";
        public static string AsignarCuentaCliente() => $"/api/v1/clientes/asignar_cuenta";
        public static string DesvincularCuentaCliente() => $"/api/v1/clientes/desasociar_cuenta";
        public static string GetInsertaDocumentoCliente() => $"/api/v1/registro/documento";

    }

    public class CatalogosOperations
    {
        public static string GetMediosPago() => $"/api/v1/catalogos/medios_pago";
        public static string GetEmpresas() => $"/api/v1/catalogos/empresas";
        public static string GetOcupaciones() => $"/api/v1/catalogos/ocupaciones";
        public static string GetNacionalidades() => $"/api/v1/catalogos/nacionalidades";
        public static string GetPaises() => $"/api/v1/catalogos/paises";
        public static string GetRoles() => $"/api/v1/catalogos/rol_clientes";
        public static string GetDocumentos() => $"/api/v1/catalogos/documentos";
        public static string GetRolesUsuario() => $"/api/v1/usuario/roles";
    }

    public class TarjetasOperations
    {
        public static string GetBuscarTarjeta(string noTarjeta) => $"/api/v1/tarjetas/buscar_tarjeta?noTarjeta={noTarjeta}";
        public static string GetTarjetasCliente(int idClientes) => $"/api/v1/tarjetas/clientes_tarjetas?idCliente={idClientes}";
        public static string GetTarjetas(int numberPage, int TotalRecords) => $"/api/v1/tarjetas/view_tarjetas?pageNumber={numberPage}&pageSize={TotalRecords}";
        public static string GetTarjetasFilter(int numberPage, int TotalRecords) => $"/api/v1/tarjetas/buscar_filtro?pageNumber={numberPage}&pageSize={TotalRecords}";
        public static string GetTarjetasRegistra() => $"/api/v1/tarjetas/add_toka";
        public static string GetBloquearDesbloquearTarjeta(string numeroTarjeta, int estatus, string valida) => $"/api/v1/tarjetas/bloquear_desbloquear_tarjeta?numeroTarjeta={numeroTarjeta}&status={estatus}&valida={valida}";
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
        public static string GetUsuariosVista() => $"/api/v1/usuario/usuarios_roles";
        public static string GetUsuariosRolesPorVista(int idRol, string vista) => $"/api/v1/usuario/set_permisos?idRol={idRol}&moduloAcceso={vista}";
        public static string GetAsignarPermisoRolVista() => $"/api/v1/usuario/asignar_permiso_rol";
        public static string GetUsuarioPorNombre(string nombre) => $"/api/v1/usuario/buscar_usuario?usuario={nombre}";
        public static string GetUsuariosAsignarRoles(int idRol, int idUsuario) => $"/api/v1/usuario/asignar_rol_usuario?idRol={idRol}&idUsuario={idUsuario}";
        public static string GetUsuariosDesasignarRoles(int idUsuario) => $"/api/v1/usuario/quitar_rol_usuario{idUsuario}";
        public static string GetInsertarRol(string nombreRol) => $"/api/v1/usuario/insertar_rol?nombreRol={nombreRol}";
    }

    public class ReintentadorOperations
    {
        public static string ReenviarTransacciones() => $"/api/v1/reintentador/reenviar_transacciones";
        public static string ReenviarTransaccion(string ClaveRastreo) => $"/api/v1/reintentador/reenviar_transaccion?claveRastreo={ClaveRastreo}";
        public static string DevolverTransacciones() => $"/api/v1/reintentador/aplicar_devoluciones";
        public static string DevolverTransaccion(string ClaveRastreo) => $"/api/v1/reintentador/aplicar_devolucion?claveRastreo={ClaveRastreo}";
        public static string AgregarTransaccion(string ClaveRastreo) => $"/api/v1/reintentador/add_transacciones?claveRastreo={ClaveRastreo}";
        public static string GetTransaccionesDevolver() => $"/api/v1/reintentador/resumen";
    }

    public string users { get; set; }
    public string Authenticate { get; set; }
    public string Transaccion { get; set; }
}