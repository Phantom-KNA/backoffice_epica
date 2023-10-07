using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Models.Response
{
    public class MensajeResponse
    {
        [JsonPropertyName("error")]
        public bool Error { get; set; }

        [JsonPropertyName("detalle")]
        public string? Detalle { get; set; }

        [JsonPropertyName("codigo")]
        public string? Codigo { get; set; }

        [JsonPropertyName("message")]
        public string? message { get; set; }

    }

    public class MensajeArchivoResponse
    {
        [JsonPropertyName("error")]
        public bool Error { get; set; }

        [JsonPropertyName("detalle")]
        public string? Detalle { get; set; }

        [JsonPropertyName("codigo")]
        public string? Codigo { get; set; }

        [JsonPropertyName("Archivo64")]
        public string? Archivo64 { get; set; }

    }
}
