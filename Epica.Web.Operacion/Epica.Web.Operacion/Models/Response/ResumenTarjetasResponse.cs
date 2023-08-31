using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Models.Response
{
    public class ResumenTarjetasResponse
    {
        [JsonPropertyName("idCliente")]
        public int? idCliente { get; set; }
        [JsonPropertyName("idCuentaAhorro")]
        public int? idCuentaAhorro { get; set; }
        [JsonPropertyName("nombreCompleto")]
        public string? nombreCompleto { get; set; }
        [JsonPropertyName("tarjeta")]
        public string? tarjeta { get; set; }
        [JsonPropertyName("proxyNumber")]
        public string? proxyNumber { get; set; }
        [JsonPropertyName("clabe")]
        public string? clabe { get; set; }
        [JsonPropertyName("estatus")]
        public int? Estatus { get; set; }
    }
}
