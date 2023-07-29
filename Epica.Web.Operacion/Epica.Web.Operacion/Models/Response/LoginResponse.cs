using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Models.Response
{
    public class LoginResponse
    {
        public string? User { get; set; }
        public string? Nombre { get; set; }
        public string? Rol { get; set; }
        public bool IsAuthenticated { get; set; }

    }
}
