using System.Text.Json.Serialization;
using static Epica.Web.Operacion.Controllers.CuentaController;

namespace Epica.Web.Operacion.Models.Response;

public class EmpresaClienteResponse
{
    [JsonPropertyName("idCliente")]
    public int IdCliente { get; set; }
    [JsonPropertyName("idEmpresa")]
    public int IdEmpresa { get; set; }
    [JsonPropertyName("nombreCompleto")]
    public string? NombreCompleto { get; set; }
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    [JsonPropertyName("curp")]
    public string? Curp { get; set; }
    [JsonPropertyName("puesto")]
    public string? Puesto { get; set; }
    [JsonPropertyName("area")]
    public string? Area { get; set; }
    [JsonPropertyName("apoderadoLegal")]
    public bool ApoderadoLegal { get; set; }
    [JsonPropertyName("roles")]
    public string? Roles { get; set; }
    [JsonPropertyName("active")]
    public int? active { get; set; }
    [JsonPropertyName("estatusWeb")]
    public int? EstatusWeb { get; set; }
}

public class EmpresaClienteResponseGrid : EmpresaClienteResponse
{
    public string Acciones { get; set; }
}

public class ResponseEmpleadosConsulta
{
    public List<EmpresaClienteResponse> listaResultado { get; set; }
    public int cantidadEncontrada { get; set; }
}