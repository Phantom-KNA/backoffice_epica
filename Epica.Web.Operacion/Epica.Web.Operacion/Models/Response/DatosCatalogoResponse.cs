using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Models.Response
{
    public class DatosCatalogoResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("descripcion")]
        public string? Descripcion { get; set; }
    }
}
