using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Models.Response
{
    public class CuentasPersonaMoralResponse
    {
        [JsonPropertyName("idCuenta")]
        public int IdCuenta { get; set; }
        [JsonPropertyName("noCuenta")]
        public string? NoCuenta { get; set; }
        [JsonPropertyName("nombrePersona")]
        public string? NombrePersona { get; set; }
        [JsonPropertyName("estatus")]
        public int Estatus { get; set; }
        [JsonPropertyName("bloqueoSPEIOut")]
        public int BloqueoSPEIOut { get; set; }
        [JsonPropertyName("saldo")]
        public double? Saldo { get; set; }
        [JsonPropertyName("cuentaAlquimiaPay")]
        public string? CuentaAlquimiaPay { get; set; }
        [JsonPropertyName("clabe")]
        public string? Clabe { get; set; }
        [JsonPropertyName("institucion")]
        public int Institucion { get; set; }
        [JsonPropertyName("descripcionInstitucion")]
        public string? DescripcionInstitucion { get; set; }
        [JsonPropertyName("producto")]
        public string? Producto { get; set; }
        [JsonPropertyName("alias")]
        public string? Alias { get; set; }
        [JsonPropertyName("idCliente")]
        public int IdCliente { get; set; }
        [JsonPropertyName("idTipoPersona")]
        public int? IdTipoPersona { get; set; }
        [JsonPropertyName("tipoPersona")]
        public string? TipoPersona { get; set; }
        [JsonPropertyName("fechaAlta")]
        public string? FechaAlta { get; set; }
        [JsonPropertyName("fechaActualizacion")]
        public string? FechaActualizacion { get; set; }
        public bool validarPermiso {  get; set; }
        public string? vinculo { get; set; }
    }

    public class CuentasPersonaMoralResponseGrid : CuentasPersonaMoralResponse
    {
        public string Acciones { get; set; }
    }
}
