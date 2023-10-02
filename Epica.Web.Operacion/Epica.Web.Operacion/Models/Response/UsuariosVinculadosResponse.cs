using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Models.Response
{
    public class UsuariosVinculadosResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("idUser")]
        public int IdUser { get; set; }
        [JsonPropertyName("username")]
        public string? Username { get; set; }
        [JsonPropertyName("idRol")]
        public int IdRol { get; set; }
        [JsonPropertyName("descripcionRol")]
        public string? DescripcionRol { get; set; }
        [JsonPropertyName("activo")]
        public int Estatus { get; set; }
        [JsonPropertyName("fechaAlta")]
        public string? FechaAlta { get; set; }
        [JsonPropertyName("ipUltimoAcceso")]
        public string? Ip { get; set; }
        [JsonPropertyName("dispositivoUltimoAcceso")]
        public string? DispositivoAcceso { get; set; }
        [JsonPropertyName("fechaUltimoAcceso")]
        public string? FechaUltimoAcceso { get; set; }
        public string? email { get; set; }
        public bool validarpermiso { get; set; }
    }
}
