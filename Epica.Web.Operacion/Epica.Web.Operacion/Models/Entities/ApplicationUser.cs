namespace Epica.Web.Operacion.Models.Entities;
/// <summary>
/// Esta clase representa un usuario de la aplicación.
/// </summary>
public class ApplicationUser
{
    public long IdUser { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Nombre { get; set; }
    public List<long> Roles { get; set; }
    public List<string> RoleNames { get; set; }
    public List<long> Permisos { get; set; }
}