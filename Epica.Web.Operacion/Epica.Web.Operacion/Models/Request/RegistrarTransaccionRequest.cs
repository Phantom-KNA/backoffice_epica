
using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Models.Request;

/// <summary>
/// Clase que representa una solicitud de registro de transacciones.
/// </summary>

public class RegistrarTransaccionRequest
{
    public int IdTrasaccion { get; set; }
    public string? ClaveRastreo { get; set; }
    public string? FechaOperacion { get; set; }
    public string? Concepto { get; set; }
    public string? NombreOrdenante { get; set; }
    public string? NoCuentaOrdenante { get; set; }
    public string? CuentaOrigenOrdenante { get; set; }
    public string? NombreBeneficiario { get; set; }
    public string? NoCuentaBeneficiario { get; set; }
    public string? CuentaDestinoBeneficiario { get; set; }
    public decimal Monto { get; set; }
    public int tipoOperacion { get; set; }
    public int medioPago { get; set; }
    public int comision { get; set; }
}

