using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Models.Request
{
    public class CargaBachRequest
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("fechaOperacion")]
        public DateTime FechaOperacion { get; set; }
        [JsonPropertyName("cuentaBeneficiario")]
        public string? CuentaBeneficiario { get; set; }
        [JsonPropertyName("monto")]
        public double Monto { get; set; }
        [JsonPropertyName("claveRastreo")]
        public string? ClaveRastreo { get; set; }
        [JsonPropertyName("conceptoPago")]
        public string? ConceptoPago { get; set; }
        [JsonPropertyName("tipoOperacion")]
        public int TipoOperacion { get; set; }
        [JsonPropertyName("medioPago")]
        public int MedioPago { get; set; }
        [JsonPropertyName("comision")]
        public int Comision { get; set; }
        [JsonPropertyName("ordenante")]
        public string? Ordenante { get; set; }
        [JsonPropertyName("idUsuario")]
        public int IdUsuario { get; set; }
    }
}
