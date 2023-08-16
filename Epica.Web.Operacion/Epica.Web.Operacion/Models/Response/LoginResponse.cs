using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Models.Response
{
    public class LoginResponse
    {
        public int IdUsuario { get; set; }
        public string Usuario { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; }
        public List<AccionModulo> AccionesPorModulo { get; set; }
        public int Activo { get; set; }
        public bool IsAuthenticated { get; set; }

    }
    public class AccionModulo
    {
        public string Modulo { get; set; }
        public List<string> Acciones { get; set; }
    }
}
