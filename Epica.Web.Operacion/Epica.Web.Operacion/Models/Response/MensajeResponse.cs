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

    }
}
