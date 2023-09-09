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
    public string? noCuentaPadre { get; set; }
    public string? nombre { get; set; }
    public string? fechaAlta { get; set; }
    public int idMedioPago { get; set; }
    public string? numeroReferencia { get; set; }
    public string? DescripcionPago { get; set; }

    private string? cuentaClabe;
    public string CuentaClabe
    {
        get => string.IsNullOrEmpty(cuentaClabe) ? "N/A" : cuentaClabe;
        set => cuentaClabe = value;
    }
    private string? noTarjeta;
    public string NoTarjeta
    {
        get => !string.IsNullOrEmpty(noTarjeta) ? new string('*', noTarjeta.Length - 4) + noTarjeta.Substring(noTarjeta.Length - 4) : string.Empty;
        set => noTarjeta = value;
    }
    private string? alias;
    public string Alias
    {
        get => string.IsNullOrEmpty(alias) ? "N/A" : alias;
        set => alias = value;
    }
    public string estatus { get; set; }
}

public class CobranzaReferenciadaResponseResponseGrid : CobranzaReferenciadaResponse
{
    public string Acciones { get; set; }
}