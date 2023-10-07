using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Models.Response;
/// <summary>
/// Clase que representa la respuesta de un token de autenticación.
/// </summary>

public class AsignarCuentaResponse
{
    [JsonPropertyName("idCliente")]
    public int idCliente { get; set; }
    [JsonPropertyName("idCuenta")]
    public int idCuenta { get; set; }
    [JsonPropertyName("idEmpresa")]
    public int idEmpresa { get; set; }
    [JsonPropertyName("descripcionRol")]
    public string? descripcionRol { get; set; }
}
