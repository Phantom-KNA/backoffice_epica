using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Models.Response
{
    public class LoginResponse
    {
        public int IdUsuario { get; set; }
        public string? Usuario { get; set; }
        public string? Email { get; set; }
        public string? Rol { get; set; }
        public List<UserPermisosResponse>? AccionesPorModulo { get; set; }
        public int Activo { get; set; }
        public bool IsAuthenticated { get; set; }

    }
}
