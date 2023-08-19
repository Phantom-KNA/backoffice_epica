using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json.Serialization;
using static Epica.Web.Operacion.Controllers.CuentaController;

namespace Epica.Web.Operacion.Models.Response;
/// <summary>
/// Clase que representa la respuesta del Api Consultar Tarjetas.
/// </summary>

public class TarjetasResponse
{
    public int idCliente { get; set; }
    public int idCuentaAhorro { get; set; }
    public string nombreCompleto { get; set; }
    public string tarjeta { get; set; }
    public string clabe { get; set; }
    public string proxyNumber { get; set; }
    //public int Estatus { get; set; }
}

public class TarjetasResponseGrid : TarjetasResponse
{
    public string Acciones { get; set; }
}