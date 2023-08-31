using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Models.Request
{
    public class DocumentosClienteRequest
    {
        [JsonPropertyName("idCliente")]
        public int idCliente { get; set; }
        [JsonPropertyName("tipoDocumento")]
        public string? tipoDocumento { get; set; }
        [JsonPropertyName("documento")]
        public string? documento { get; set; }
    }

    public class DocumentosClienteRegistro
    {
        [JsonPropertyName("idCliente")]
        public int idCliente { get; set; }
        [JsonPropertyName("tipoDocumento")]
        public string? tipoDocumento { get; set; }
        [JsonPropertyName("documento")]
        public IFormFile? documento { get; set; }
        public List<DatosCatalogoResponse> listaDocs { get; set; }
    }
}
