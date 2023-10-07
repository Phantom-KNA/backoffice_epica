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
    public int tipo_documento { get; set; }
    [JsonPropertyName("descripcionDocumento")]
    public string? DescripcionDocumento { get; set; }
    [JsonPropertyName("ruta")]
    public string? Ruta { get; set; }
    [JsonPropertyName("observaciones")]
    public string? Observaciones { get; set; }
    [JsonPropertyName("ruta_documento")]
    public string? ruta_documento { get; set; }
    [JsonPropertyName("idUsuarioAlta")]
    public int IdUsuarioAlta { get; set; }
    [JsonPropertyName("fechaUsuarioAlta")]
    public DateTime? fecha_alta { get; set; }
    public string? fechaalta { get; set; }

    [JsonPropertyName("idUsuarioActualizacion")]
    public int IdUsuarioActualizacion { get; set; }
    [JsonPropertyName("fechaUsuarioActualizacion")]
    public DateTime? fecha_actualizacion { get; set; }
    public string? fechaactualizacion { get; set; }

    public string? urlAlly { get; set; }
}

public class DocumentosClienteResponseGrid : DocumentosClienteResponse
{
    public string Acciones { get; set; }
}