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
        //public string? FechaFormat {  get; set; }
        public int IdUsuario { get; set; }
        public int estatus { get; set; }
        public string descripcionOperacion { get; set; }
        public string descripcionMedioPago { get; set; }
        public List<DatosCatalogoResponse>? ListaMediosPago { get; set; }
        public string observaciones { get; set; }
        //public List<DatosCatalogoResponse>? ListaTipoTransaccion { get; set; }
    }
    public class CargaBachRequestGrid : CargaBachRequest
    {
        public string Acciones { get; set; }
    }
}
