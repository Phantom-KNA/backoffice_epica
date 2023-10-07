
namespace Epica.Web.Operacion.Models;

/// <summary>
/// Clase que representa el modelo de error utilizado para mostrar información de errores en la vista.
/// </summary>
public class ErrorViewModel
{
    public string RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
