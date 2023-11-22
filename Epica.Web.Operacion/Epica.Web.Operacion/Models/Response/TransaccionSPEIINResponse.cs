using Epica.Web.Operacion.Models.Request;
using System.Text.Json.Serialization;
using static Epica.Web.Operacion.Controllers.CuentaController;

namespace Epica.Web.Operacion.Models.Response;

public class TransaccionSPEIINResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("claveRastreo")]
    public string? ClaveRastreo { get; set; }
    [JsonPropertyName("monto")]
    public double Monto { get; set; }
    [JsonPropertyName("cuentaOrdenante")]
    public string? CuentaOrdenante { get; set; }
    [JsonPropertyName("nombreOrdenante")]
    public string? NombreOrdenante { get; set; }
    [JsonPropertyName("concepto")]
    public string? Concepto { get; set; }
    [JsonPropertyName("descripcionEstatusAutorizacion")]
    public string? DescripcionEstatusAutorizacion { get; set; }
    [JsonPropertyName("descripcioEstatusTransaccion")]
    public string? descripcioEstatusTransaccion { get; set; }
    [JsonPropertyName("fecha")]
    public string? Fecha { get; set; }
}

public class ResumenTransaccionSPEIINResponseGrid : TransaccionSPEIINResponse
{
    public string Acciones { get; set; }
}

public class ResponseSpeiINConsulta {
    public List<TransaccionSPEIINResponse> listaResultado { get; set; }
    public int cantidadEncontrada { get; set; }
}