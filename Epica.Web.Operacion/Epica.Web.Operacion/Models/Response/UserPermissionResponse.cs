using System.Text.Json.Serialization;
using static Epica.Web.Operacion.Controllers.CuentaController;

namespace Epica.Web.Operacion.Models.Response;
/// <summary>
/// Clase que representa la respuesta de un token de autenticación.
/// </summary>

public class UserPermissionResponse
{
    public long id { get; set; }
    public string nombreRol { get; set; }
    public List<ListUserPermissionResponse> listaGen { get; set; }
}

public class ListUserPermissionResponse
{
    public int IdPermiso { get; set; }
    public int IdRol { get; set; }
    public string vista { get; set; }
    public bool Escritura { get; set; }
    public bool Lectura { get; set; }
    public bool Eliminar { get; set; }
    public bool Actualizar { get; set; }
}

public class UserPermissionResponseGrid : UserPermissionResponse
{
    public string Acciones { get; set; }
}