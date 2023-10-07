
using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Models.Request;

/// <summary>
/// Clase que representa una solicitud de registro de tarjetas asociado a usuario.
/// </summary>

public class RegistrarTarjetaRequest
{
    public int idCliente { get; set; }
    public string? numeroTarjeta { get; set; }
    public string? proxyNumber { get; set; }
    public string? mesVigencia { get; set; }
    public string? yearVigencia { get; set; }
}

