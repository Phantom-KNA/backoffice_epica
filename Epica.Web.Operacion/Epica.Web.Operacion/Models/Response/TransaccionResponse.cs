using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json.Serialization;
using static Epica.Web.Operacion.Controllers.CuentaController;

namespace Epica.Web.Operacion.Models.Response;
/// <summary>
/// Clase que representa la respuesta del Api Consultar Tarjetas.
/// </summary>

public class TransaccionDetailsResponse
{
    public string result { get; set; }
    public TransaccionResponse value { get; set; }

}
public class TransaccionResponse
{
    [JsonPropertyName("idCuentaAhorro")]
    public int IdCuentaAhorro { get; set; }
    [JsonPropertyName("idCliente")]
    public int IdCliente { get; set; }
    [JsonPropertyName("claveRastreo")]
    public string? ClaveRastreo { get; set; }
    [JsonPropertyName("idMedioPago")]
    public int? IdMedioPago { get; set; }
    [JsonPropertyName("idTransaccion")]
    public int? IdTrasaccion { get; set; }
    [JsonPropertyName("nombreOrdenante")]
    public string? nombreOrdenante { get; set; }
    [JsonPropertyName("cuetaOrigenOrdenante")]
    public string? cuetaOrigenOrdenante { get; set; }
    [JsonPropertyName("nombreBeneficiario")]
    public string? NombreBeneficiario { get; set; }
    [JsonPropertyName("cuentaDestinoBeneficiario")]
    public string? CuentaDestinoBeneficiario { get; set; }
    [JsonPropertyName("monto")]
    public double? Monto { get; set; }
    [JsonPropertyName("concepto")]
    public string? Concepto { get; set; }
    [JsonPropertyName("conceptopOtro")]
    public string? ConceptoOtro { get; set; }
    [JsonPropertyName("clabeCobranza")]
    public string? ClabeCobranza { get; set; }
    [JsonPropertyName("estatus")]
    public int? Estatus { get; set; }
    [JsonPropertyName("categoria")]
    public string? Categoria { get; set; }
    [JsonPropertyName("establecimiento")]
    public string? Establecimiento { get; set; }
    [JsonPropertyName("instructor")]
    public string? Instructor { get; set; }
    [JsonPropertyName("fechaAlta")]
    public string? FechaAlta { get; set; }
    [JsonPropertyName("fechaActualizacion")]
    public string? FechaActualizacion { get; set; }
}
