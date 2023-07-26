using System.Text.Json.Serialization;
using static Epica.Web.Operacion.Controllers.CuentaController;

namespace Epica.Web.Operacion.Models.Response;
/// <summary>
/// Clase que representa la respuesta de un token de autenticación.
/// </summary>

public class UserResponse
{
    public long IdCliente { get; set; }
    public string Nombre { get; set; }
    public string Telefono { get; set; }
    public string Email { get; set; }
    public string Curp { get; set; }
    public string Organizacion { get; set; }
    public string TipoMembresia { get; set; }
    public string Sexo { get; set; }
    public bool Estatus { get; set; }
}

public class UserResponseGrid : UserResponse
{
    public string Acciones { get; set; }
}