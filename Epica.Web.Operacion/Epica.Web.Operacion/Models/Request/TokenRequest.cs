
namespace Epica.Web.Operacion.Models.Request;

/// <summary>
/// Clase que representa una solicitud de token de autenticación.
/// </summary>

public class TokenRequest
{
	public string Username { get; set; }
	public string Password { get; set; }
	public string? IdDispositivo { get; set; }
	public string? ModeloDispositivo { get; set; }
}

