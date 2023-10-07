using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Models.Response
{
    public class UserRolPermisosResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("nombreRol")]
        public string? NombreRol { get; set; }
        [JsonPropertyName("accionesPorModulo")]
        public List<UserPermisosResponse>? AccionesPorModulo { get; set; }
    }
}
