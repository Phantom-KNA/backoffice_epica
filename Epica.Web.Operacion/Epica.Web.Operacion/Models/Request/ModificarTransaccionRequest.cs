using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Models.Request
{
    public class ModificarTransaccionRequest
    {
        [JsonPropertyName("idTrasaccion")]
        public int IdTrasaccion { get; set; }
        [JsonPropertyName("claveRastreo")]
        public string? ClaveRastreo { get; set; }
        [JsonPropertyName("fechaOperacion")]
        public string? FechaOperacion { get; set; }
        [JsonPropertyName("concepto")]
        public string? Concepto { get; set; }
        [JsonPropertyName("nombreOrdenante")]
        public string? NombreOrdenante { get; set; }
        [JsonPropertyName("noCuentaOrdenante")]
        public string? NoCuentaOrdenante { get; set; }
        [JsonPropertyName("cuentaOrigenOrdenante")]
        public string? CuentaOrigenOrdenante { get; set; }
        [JsonPropertyName("nombreBeneficiario")]
        public string? NombreBeneficiario { get; set; }
        [JsonPropertyName("noCuentaBeneficiario")]
        public string? NoCuentaBeneficiario { get; set; }
        [JsonPropertyName("cuentaDestinoBeneficiario")]
        public string? CuentaDestinoBeneficiario { get; set; }
    }
}
