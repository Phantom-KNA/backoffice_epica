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
    [JsonPropertyName("idCliente")]
    public int IdCliente { get; set; }
    [JsonPropertyName("nombre")]
    public string? Nombre { get; set; }
    [JsonPropertyName("apellidoPaterno")]
    public string? ApellidoPaterno { get; set; }
    [JsonPropertyName("apellidoMaterno")]
    public string? ApellidoMaterno { get; set; }
    [JsonPropertyName("telefono")]
    public string? Telefono { get; set; }
    [JsonPropertyName("telefonoRecado")]
    public string? TelefonoRecado { get; set; }
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    [JsonPropertyName("curp")]
    public string? CURP { get; set; }
    [JsonPropertyName("organizacion")]
    public string? Organizacion { get; set; }
    [JsonPropertyName("fechaNacimiento")]
    public string? FechaNacimiento { get; set; }
    [JsonPropertyName("entidadNacimiento")]
    public string? EntidadNacimiento { get; set; }
    [JsonPropertyName("idNacionalidad")]
    public int IdNacionalidad { get; set; }
    [JsonPropertyName("membresia")]
    public string? Membresia { get; set; }
    [JsonPropertyName("sexo")]
    public string? Sexo { get; set; }
    [JsonPropertyName("ine")]
    public string? INE { get; set; }
    [JsonPropertyName("rfc")]
    public string? RFC { get; set; }
    [JsonPropertyName("paisNacimiento")]
    public string? PaisNacimiento { get; set; }
    [JsonPropertyName("observaciones")]
    public string? Observaciones { get; set; }
    [JsonPropertyName("idOcupacion")]
    public int IdOcupacion { get; set; }
    [JsonPropertyName("nacionalidad")]
    public string? Nacionalidad { get; set; }
    [JsonPropertyName("fiel")]
    public string? Fiel { get; set; }
    [JsonPropertyName("nss")]
    public string? NSS { get; set; }
    [JsonPropertyName("colonia")]
    public string? Colonia { get; set; }
    [JsonPropertyName("calleNumero")]
    public string? CalleNumero { get; set; }
    [JsonPropertyName("noInt")]
    public string? NoInt { get; set; }
    [JsonPropertyName("municipio")]
    public string? Municipio { get; set; }
    [JsonPropertyName("estado")]
    public string? Estado { get; set; }
    [JsonPropertyName("codigoPostal")]
    public string? CodigoPostal { get; set; }
    [JsonPropertyName("calle")]
    public string? Calle { get; set; }
    [JsonPropertyName("calleSecundaria")]
    public string? CalleSecundaria { get; set; }
    [JsonPropertyName("calleSecundaria2")]
    public string? CalleSecundaria2 { get; set; }
    [JsonPropertyName("tipoVivienda")]
    public int TipoVivienda { get; set; }
    [JsonPropertyName("tiempoVivienda")]
    public string? TiempoVivienda { get; set; }
    [JsonPropertyName("tipoTrabajador")]
    public string? TipoTrabajador { get; set; }
    [JsonPropertyName("puesto")]
    public string? Puesto { get; set; }
    [JsonPropertyName("salarioMensual")]
    public double? SalarioMensual { get; set; }
    [JsonPropertyName("antiguedadLaboral")]
    public string? AntiguedadLaboral { get; set; }
    [JsonPropertyName("active")]
    public int Active { get; set; }//= default(bool?);
    [JsonPropertyName("idEmpresa")]
    public int? IdEmpresa { get; set; }
    [JsonPropertyName("idPais")]
    public int? IdPais { get; set; }
    [JsonPropertyName("montoMaximo")]
    public string? MontoMaximo { get; set; }
    [JsonPropertyName("rol")]
    public string? Rol { get; set; }
    [JsonPropertyName("apoderadoLegal")] 
    public int ApoderadoLegal { get; set; }
}