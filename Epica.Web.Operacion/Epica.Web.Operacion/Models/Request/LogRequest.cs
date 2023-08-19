using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Models.Request
{
    public class LogRequest
    {
        [JsonPropertyName("idUser")]
        public string? IdUser { get; set; }
        [JsonPropertyName("modulo")]
        public string? Modulo { get; set; }
        [JsonPropertyName("fecha")]
        public DateTime Fecha { get; set; }
        [JsonPropertyName("nombreEquipo")]
        public string? NombreEquipo { get; set; }
        [JsonPropertyName("ip")]
        public string? Ip { get; set; }
        [JsonPropertyName("accion")]
        public string? Accion { get; set; }
        [JsonPropertyName("idRegistro")]
        public int IdRegistro { get; set; }
        [JsonPropertyName("envio")]
        public string? Envio { get; set; }
        [JsonPropertyName("respuesta")]
        public string? Respuesta { get; set; }
        [JsonPropertyName("error")]
        public string? Error { get; set; }
    }
}
