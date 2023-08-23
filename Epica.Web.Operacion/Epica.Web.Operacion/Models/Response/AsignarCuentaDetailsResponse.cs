using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Models.Response;
/// <summary>
/// Clase que representa la respuesta de un token de autenticación.
/// </summary>

public class AsignarCuentaDetailsResponse
{
    [JsonPropertyName("idCliente")]
    public int IdCliente { get; set; }
    [JsonPropertyName("idCuenta")]
    public int IdCuenta { get; set; }
    [JsonPropertyName("idEmpresa")]
    public int IdEmpresa { get; set; }
    public string? Rol { get; set; }
    public List<DatosCatalogoResponse>? ListaRoles { get; set; }
    public List<DatosCatalogoResponse>? ListaEmpresa { get; set; }
}
