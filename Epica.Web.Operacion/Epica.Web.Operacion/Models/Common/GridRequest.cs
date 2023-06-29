using Newtonsoft.Json;

namespace Epica.Web.Operacion.Models.Common;
/// <summary>
/// Modelo para representar una solicitud de datos de una cuadrícula.
/// </summary>
public class GridRequest
{
    /// <summary>
    /// Pagina solicitada
    /// </summary>
    [JsonProperty("pagina")]
    public int? Pagina { get; set; }
    
    /// <summary>
    /// Cantidad de registros por pagina.
    /// </summary>
    
    [JsonProperty("registros")]
    public int? Registros { get; set; }

    /// <summary>
    /// Indica si la cuadrícula está activa.
    /// </summary>

    [JsonProperty("activo")]
    public bool? Activo { get; set; }
    /// <summary>
    /// Término de búsqueda aplicado en la cuadrícula.
    /// </summary>
    [JsonProperty("busqueda")]
    public string? Busqueda { get; set; }
    /// <summary>
    /// Columna utilizada para el ordenamiento.
    /// </summary>
    [JsonProperty("columnaOrdenamiento")]
    public string? ColumnaOrdenamiento { get; set; }

    /// <summary>
    /// Dirección de ordenamiento (ascendente o descendente).
    /// </summary>
    [JsonProperty("ordenamiento")]
    public string? Ordenamiento { get; set; }
}
