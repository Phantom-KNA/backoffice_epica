
using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Models.Request;

/// <summary>
/// Clase que representa una solicitud de registro de transacciones.
/// </summary>

public class RegistrarTransaccionRequest
{
    [JsonPropertyName("fechaOperacion")]
    public string? FechaOperacion { get; set; }
    [JsonPropertyName("noCuentaBeneficiario")]
    public string? NoCuentaBeneficiario { get; set; }
    [JsonPropertyName("monto")]
    public double Monto { get; set; }
    [JsonPropertyName("claveRastreo")]
    public string? ClaveRastreo { get; set; }
    [JsonPropertyName("concepto")]
    public string? Concepto { get; set; }
    [JsonPropertyName("tipoOperacion")]
    public int TipoOperacion { get; set; }
    [JsonPropertyName("medioPago")]
    public int MedioPago { get; set; }
    [JsonPropertyName("comision")]
    public int Comision { get; set; }
    [JsonPropertyName("nombreOrdenante")]
    public string? NombreOrdenante { get; set; }
    public string? cuentaOrdenante { get; set; }
}

