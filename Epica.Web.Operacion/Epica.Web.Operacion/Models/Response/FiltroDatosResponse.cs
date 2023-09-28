using Epica.Web.Operacion.Models.Request;
using System.Text.Json.Serialization;
using static Epica.Web.Operacion.Controllers.CuentaController;

namespace Epica.Web.Operacion.Models.Response;
/// <summary>
/// Clase que representa la respuesta de un token de autenticación.
/// </summary>

public class FiltroDatosResponse
{
    public List<ClienteResponse> listaResultado { get; set; }
    public int? cantidadEncontrada { get; set; }
}

public class FiltroDatosResponseCuenta
{
    public List<CuentasResponse> listaResultado { get; set; }
    public int? cantidadEncontrada { get; set; }
}

public class FiltroDatosResponseTransacciones
{
    public List<TransaccionesResponse> listaResultado { get; set; }
    public int? cantidadEncontrada { get; set; }
}

public class FiltroDatosResponseTransaccionesBatch
{
    public List<CargaBachRequest> listaResultado { get; set; }
    public int? cantidadEncontrada { get; set; }
}
