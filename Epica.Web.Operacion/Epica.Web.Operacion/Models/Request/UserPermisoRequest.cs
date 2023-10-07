using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Models.Request
{
    public class UserPermisoPostRequest
    {
        public string? vista { get; set; }
        public int permiso { get; set; }
        public int rol { get; set; }
        public string? accion { get; set; }
        public Boolean? activo { get; set; }
    }

    public class UserPermisoRequest
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
