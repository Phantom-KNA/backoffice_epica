using Epica.Web.Operacion.Models.Request;
using System.Text.Json.Serialization;
using static Epica.Web.Operacion.Controllers.CuentaController;

namespace Epica.Web.Operacion.Models.Response;
/// <summary>
/// Clase que representa la respuesta de un token de autenticación.
/// </summary>

public class ClienteDetailsResponse
{
    public string result { get; set; }
    public DatosClienteResponse value { get; set; }

}

public class DatosClienteResponse
{
    public int idCliente { get; set; }
    public string? NombreCompleto { get; set; }
    public string? Nombre { get; set; }
    public string? ApellidoPaterno { get; set; }
    public string? ApellidoMaterno { get; set; }
    public string? Telefono { get; set; }
    public string? TelefonoRecado { get; set; }
    public string? Email { get; set; }
    public string? CURP { get; set; }
    public string? Organizacion { get; set; }
    public string? FechaNacimiento { get; set; }
    public string? EntidadNacimiento { get; set; }
    public string? Membresia { get; set; }
    public string? Sexo { get; set; }
    public string? INE { get; set; }
    public string? RFC { get; set; }
    public string? PaisNacimiento { get; set; }
    public string? Observaciones { get; set; }
    public int IdOcupacion { get; set; }
    public string? Nacionalidad { get; set; }
    public string? Fiel { get; set; }
    public string? NSS { get; set; }
    public string? Colonia { get; set; }
    public string? NoIntExt { get; set; }
    public string? Municipio { get; set; }
    public string? Estado { get; set; }
    public string? CodigoPostal { get; set; }
    public string? Calle { get; set; }
    public string? CalleSecundaria { get; set; }
    public string? CalleSecundaria2 { get; set; }
    public int? TipoVivienda { get; set; }
    public string? TiempoVivienda { get; set; }
    public string? TipoTrabajador { get; set; }
    public string? puesto { get; set; }
    public double? SalarioNetoMensual { get; set; }
    public double? SalarioMensual { get; set; }
    public double? montoMaximo { get; set; }
    public string? AntiguedadLaboral { get; set; }
    public string? calleNumero { get; set; }
    public string? rol { get; set; }
    public int idEmpresa { get; set; }
    public int idPais { get; set; }
    public int idNacionalidad { get; set; }
    public int? Active { get; set; }//= default(bool?);
}