namespace Epica.Web.Operacion.Models;

/// <summary>
/// Clase que representa el modelo de respuesta JSON utilizado para transmitir datos y estado de error en una respuesta HTTP.
/// </summary>
/// 
public class JsonResultDto
{
    public string ErrorDescription { get; set; }
    public bool Error { get; set; }
    public object Result { get; set; }
}
