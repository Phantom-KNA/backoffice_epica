using System.Text.Json.Serialization;

namespace Epica.Api.Operacion.Models.Response
{
    public class ErrorResponse
    {
        [JsonPropertyName("error")]
        public bool Error { get; set; }

        [JsonPropertyName("detalle")]
        public string Detalle { get; set; }

        [JsonPropertyName("codigo")]
        public string Codigo { get; set; }

    }
}
