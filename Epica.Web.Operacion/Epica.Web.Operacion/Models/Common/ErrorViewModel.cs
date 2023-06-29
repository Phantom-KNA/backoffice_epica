
namespace Epica.Web.Operacion.Models.Common;
/// <summary>
/// Modelo para representar los errores
/// </summary>
public class ErrorViewModel
{
    /// <summary>
    /// Identificador de solicitud.
    /// </summary>
    public string RequestId { get; set; }
    /// <summary>
    /// Indica si se debe mostrar el identificador de la solicitud.
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
