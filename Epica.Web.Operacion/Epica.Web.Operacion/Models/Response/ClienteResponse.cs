using System.Text.Json.Serialization;
using static Epica.Web.Operacion.Controllers.CuentaController;

namespace Epica.Web.Operacion.Models.Response;
/// <summary>
/// Clase que representa la respuesta de un token de autenticación.
/// </summary>

public class ClienteResponse
{
    public long id { get; set; }
    public string nombreCompleto { get; set; }
    public string telefono { get; set; }
    public string email { get; set; }
    public string CURP { get; set; }
    public string organizacion { get; set; }
    public string membresia { get; set; }
    public string sexo { get; set; }
    public int estatus { get; set; }
    public string estatusweb { get; set; }
    public string vinculo { get; set; }
}

public class ClienteResponseGrid : ClienteResponse
{
    public string Acciones { get; set; }
}