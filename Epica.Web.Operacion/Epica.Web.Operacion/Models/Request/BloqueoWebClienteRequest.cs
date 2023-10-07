
namespace Epica.Web.Operacion.Models.Request;

/// <summary>
/// Clase que representa una solicitud de bloqueo de usuario.
/// </summary>

public class BloqueoWebClienteRequest
{
	public int idCliente { get; set; }
	public int estatus { get; set; }
}

