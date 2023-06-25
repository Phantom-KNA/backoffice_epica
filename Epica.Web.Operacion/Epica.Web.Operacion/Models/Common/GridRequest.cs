using Newtonsoft.Json;

namespace Epica.Web.Operacion.Models.Common;

public class GridRequest
{
    [JsonProperty("pagina")]
    public int? Pagina { get; set; }

    [JsonProperty("registros")]
    public int? Registros { get; set; }

    [JsonProperty("activo")]
    public bool? Activo { get; set; }

    [JsonProperty("busqueda")]
    public string? Busqueda { get; set; }

    [JsonProperty("columnaOrdenamiento")]
    public string? ColumnaOrdenamiento { get; set; }

    [JsonProperty("ordenamiento")]
    public string? Ordenamiento { get; set; }
}
