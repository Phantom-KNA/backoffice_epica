using System.Text.Json.Serialization;
using static Epica.Web.Operacion.Controllers.CuentaController;

namespace Epica.Web.Operacion.Models.Response;
/// <summary>
/// Clase que representa la respuesta de un token de autenticación.
/// </summary>

public class VoucherResponse
{
    public string? tipo_archivo { get; set; }
    public string? archivo { get; set; }
}
