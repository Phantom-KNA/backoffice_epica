using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Models.Response;
/// <summary>
/// Clase que representa la respuesta de un token de autenticación.
/// </summary>

public class AsignarRolUsuario
{
    [JsonPropertyName("idUsuario")]
    public int idUsuario { get; set; }
    [JsonPropertyName("idRol")]
    public int idRol { get; set; }
    [JsonPropertyName("idEmpresa")]
    public List<DatosCatalogoResponse>? ListaRoles { get; set; }
}
