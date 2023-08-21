using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Models.Response
{
    public class UserPermisosResponse
    {
        [JsonPropertyName("idPermiso")]
        public int IdPermiso { get; set; }
        [JsonPropertyName("idRol")]
        public int IdRol { get; set; }
        [JsonPropertyName("moduloAcceso")]
        public string? ModuloAcceso { get; set; }
        [JsonPropertyName("ver")]
        public int Ver { get; set; }
        [JsonPropertyName("insertar")]
        public int Insertar { get; set; }
        [JsonPropertyName("editar")]
        public int Editar { get; set; }
        [JsonPropertyName("eliminar")]
        public int Eliminar { get; set; }
    }
}
