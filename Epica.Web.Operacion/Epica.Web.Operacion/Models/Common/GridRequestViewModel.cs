using Epica.Web.Operacion.Models.Entities;
using Newtonsoft.Json;

namespace Epica.Web.Operacion.Models.Common;

public class GridRequestViewModel
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public bool? Active { get; set; }
    public string? SearchValue { get; set; }
    public string? SortColumn { get; set; }
    public string? Sort { get; set; }
    public int Skip { get; set; }
}


public class RequestLista
{
    public string Draw { get; set; }
    public int PageSize { get; set; }
    public int Skip { get; set; }
    public string Busqueda { get; set; }
    public string ColumnaOrdenamiento { get; set; }
    public string Ordenamiento { get; set; }
}

public class RequestList
{
    [JsonProperty("Id")]
    public long? Id { get; set; }
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
    [JsonProperty("filtros")]
    public List<RequestListFilters>? Filtros { get; set; }

}

public class RequestListFilters
{
    [JsonProperty("key")]
    public string Key { get; set; }
    [JsonProperty("value")]
    public string Value { get; set; }
}

public class ResponseGrid<T>
{
    #region Propiedades
    [JsonProperty("recordsTotal")]
    public int? RecordsTotal { get; set; }
    [JsonProperty("recordsFiltered")]
    public int? RecordsFiltered { get; set; }
    [JsonProperty("draw")]
    public string? Draw { get; set; }

    [JsonProperty("data")]
    public List<T> Data { get; set; }
    #endregion

    #region Constructores
    /// <summary>
    /// Constructor vacio.
    /// </summary>
    public ResponseGrid() { }


    #endregion
}
public class ResponseGridMovil<T>
{
    #region Propiedades       

    [JsonProperty("data")]
    public List<T> Data { get; set; }
    #endregion

    #region Constructores
    /// <summary>
    /// Constructor vacio.
    /// </summary>
    public ResponseGridMovil() { }


    #endregion
}
