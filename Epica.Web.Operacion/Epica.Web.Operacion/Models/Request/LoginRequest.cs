using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Models.Request
{
    public class LoginRequest
    {
        public string? User{ get; set; }
        public string? Password { get; set; }
    }
}
