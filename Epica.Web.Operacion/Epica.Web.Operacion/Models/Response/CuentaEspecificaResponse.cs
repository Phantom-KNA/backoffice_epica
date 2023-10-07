using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Models.Response;
/// <summary>
/// Clase que representa la respuesta de un token de autenticación.
/// </summary>

public class CuentaEspecificaResponse
{

    [JsonPropertyName("id")]
    public int id { get; set; }

    [JsonPropertyName("descripcion")]
    public string descripcion { get; set; }

}
