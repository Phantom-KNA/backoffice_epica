using Epica.Web.Operacion.Models.Request;
using System.Text.Json.Serialization;
using static Epica.Web.Operacion.Controllers.CuentaController;

namespace Epica.Web.Operacion.Models.Response;
/// <summary>
/// Clase que representa la respuesta de un token de autenticación.
/// </summary>

public class UserDetailsResponse
{
    public string result { get; set; }
    public DatosClienteResponse value { get; set; }

}

public class DatosClienteResponse
{
    public int IdCliente { get; set; }
    public string? NombreCompleto { get; set; }
    public string? Telefono { get; set; }
    public string? TelefonoRecado { get; set; }
    public string? Email { get; set; }
    public string? CURP { get; set; }
    public string? Organizacion { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public string? EntidadNacimiento { get; set; }
    public int IdNacionalida { get; set; }
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
    public string? Puesto { get; set; }
    public double? SalarioNetoMensual { get; set; }
    public string? AntiguedadLaboral { get; set; }
    public int? Active { get; set; }//= default(bool?);
}