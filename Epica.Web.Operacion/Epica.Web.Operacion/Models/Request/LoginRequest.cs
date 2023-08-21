using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Models.Request
{
    public class LoginRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Ip { get; set; }
        public string? DispositivoAcceso { get; set; }
    }
}
