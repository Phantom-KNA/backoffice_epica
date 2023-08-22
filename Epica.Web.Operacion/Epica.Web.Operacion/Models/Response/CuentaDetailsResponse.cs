using Epica.Web.Operacion.Models.Request;
using System.Text.Json.Serialization;
using static Epica.Web.Operacion.Controllers.CuentaController;

namespace Epica.Web.Operacion.Models.Response;
/// <summary>
/// Clase que representa la respuesta de un token de autenticación.
/// </summary>

public class CuentaDetailsResponse
{
    public string result { get; set; }
    public DatosClienteResponse value { get; set; }

}

public class DatosCuentaResponse
{
    [JsonPropertyName("idCuenta")]
    public int IdCuenta { get; set; }
    [JsonPropertyName("noCuenta")]
    public string? NoCuenta { get; set; }
    [JsonPropertyName("nombrePersona")]
    public string? NombrePersona { get; set; }
    [JsonPropertyName("estatus")]
    public int Estatus { get; set; }
    [JsonPropertyName("saldo")]
    public double? Saldo { get; set; }
    [JsonPropertyName("cuentaAlquimiaPay")]
    public string? CuentaAlquimiaPay { get; set; }
    [JsonPropertyName("clabe")]
    public string? Clabe { get; set; }
    [JsonPropertyName("institucion")]
    public int Institucion { get; set; }
    [JsonPropertyName("producto")]
    public string? Producto { get; set; }
    [JsonPropertyName("alias")]
    public string? Alias { get; set; }
    [JsonPropertyName("idCliente")]
    public int IdCliente { get; set; }
    [JsonPropertyName("tipoPersona")]
    public string? TipoPersona { get; set; }
    [JsonPropertyName("fechaAlta")]
    public string? FechaAlta { get; set; }
    [JsonPropertyName("fechaActualizacion")]
    public string? FechaActualizacion { get; set; }
}