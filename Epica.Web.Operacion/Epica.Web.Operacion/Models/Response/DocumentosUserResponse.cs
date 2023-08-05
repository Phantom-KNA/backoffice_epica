namespace Epica.Web.Operacion.Models.Response;

/// <summary>
/// Clase del modelo para obtener el listado de documentos
/// </summary>
/// 
public class DocumentosUserResponse
{
    public int idCliente { get; set; }
    public int idDocApodLeg { get; set; }
    public int tipoDocumento { get; set; }
    public string documento { get; set; }
    public string numeroIdentificacion { get; set; }
    public string nombreDocumento { get; set; }
}

public class DocumentosUserResponseGrid : DocumentosUserResponse
{
    public string Acciones { get; set; }
}