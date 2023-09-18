using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Models.Response;

/// <summary>
/// Clase del modelo para obtener el listado de documentos
/// </summary>
/// 
public class DocumentosClienteDetailsResponse
{
    public string result { get; set; }
    public List<DocumentosClienteResponse> value { get; set; }

}
public class DocumentosClienteResponse
{
    [JsonPropertyName("idDocumento")]
    public int IdDocumento { get; set; }
    [JsonPropertyName("idCliente")]
    public int IdCliente { get; set; }
    [JsonPropertyName("tipoDocumento")]
    public int TipoDocumento { get; set; }
    [JsonPropertyName("descripcionDocumento")]
    public string? DescripcionDocumento { get; set; }
    [JsonPropertyName("ruta")]
    public string? Ruta { get; set; }
    [JsonPropertyName("observaciones")]
    public string? Observaciones { get; set; }
    [JsonPropertyName("urlImagen")]
    public string? UrlImagen { get; set; }
    [JsonPropertyName("idUsuarioAlta")]
    public int IdUsuarioAlta { get; set; }
    [JsonPropertyName("fechaUsuarioAlta")]
    public DateTime? FechaUsuarioAlta { get; set; }

    [JsonPropertyName("idUsuarioActualizacion")]
    public int IdUsuarioActualizacion { get; set; }
    [JsonPropertyName("fechaUsuarioActualizacion")]
    public DateTime? FechaUsuarioActualizacion { get; set; }
}

public class DocumentosClienteResponseGrid : DocumentosClienteResponse
{
    public string Acciones { get; set; }
}