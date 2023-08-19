using System.Text.Json.Serialization;
using static Epica.Web.Operacion.Controllers.CuentaController;

namespace Epica.Web.Operacion.Models.Response;
/// <summary>
/// Clase que representa la respuesta del servicio de cobranza referenciada.
/// </summary>

public class CobranzaReferenciadaResponse
{
    public int id { get; set; }
    public int idCuentaAhorroPadre { get; set; }
    public string noCuentaPadre { get; set; }
    public string nombre { get; set; }
    public string fechaAlta { get; set; }
    public int idMedioPago { get; set; }
    public string numeroReferencia { get; set; }
    public string estatus { get; set; }
}

public class CobranzaReferenciadaResponseResponseGrid : CobranzaReferenciadaResponse
{
    public string Acciones { get; set; }
}