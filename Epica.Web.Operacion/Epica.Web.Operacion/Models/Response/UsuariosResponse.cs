using System.Text.Json.Serialization;
using static Epica.Web.Operacion.Controllers.CuentaController;

namespace Epica.Web.Operacion.Models.Response;
/// <summary>
/// Clase que representa la respuesta de un token de autenticación.
/// </summary>

public class UsuariosResponse
{
    public int IdUser { get; set; }
    public string Username { get; set; }
    public string DescripcionRol { get; set; }  
    public string FechaAlta { get; set; }
    public string FechaUltimoAcceso { get; set; }
    public int estatus { get; set; }
}

public class UsuariosResponseGrid : UsuariosResponse
{
    public string Acciones { get; set; }
}