
namespace Epica.Web.Operacion.Models.Request;

/// <summary>
/// Clase que representa una solicitud de bloqueo total de usuario.
/// </summary>

public class BloqueoTotalClienteRequest
{
	public int idCliente { get; set; }
	public int estatus { get; set; }
}

