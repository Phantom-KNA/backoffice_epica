using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Models.Response
{
    public class DatosClienteMoralResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("nombre")]
        public string? Nombre { get; set; }
        [JsonPropertyName("tipo")]
        public string? TipoEmpresa { get; set; }
        [JsonPropertyName("rfc")]
        public string? RFC { get; set; }
        [JsonPropertyName("numeroIdentificacion")]
        public string? NumeroIdentificacion { get; set; }
        [JsonPropertyName("fechaCreacion")]
        public string? FechaCreacion { get; set; }
        [JsonPropertyName("giro")]
        public string? Giro { get; set; }
        [JsonPropertyName("direccion")]
        public string? Direccion { get; set; }
        [JsonPropertyName("estado")]
        public string? Estado { get; set; }
        [JsonPropertyName("municipio")]
        public string? Municipio { get; set; }
        [JsonPropertyName("colonia")]
        public string? Colonia { get; set; }
        [JsonPropertyName("calle")]
        public string? Calle { get; set; }
        [JsonPropertyName("primeraCalle")]
        public string? PrimeraCalle { get; set; }
        [JsonPropertyName("segundaCalle")]
        public string? SegundaCalle { get; set; }
        [JsonPropertyName("numeroExterior")]
        public string? NumeroExterior { get; set; }
        [JsonPropertyName("numeroInterior")]
        public string? NumeroInterior { get; set; }
        [JsonPropertyName("codigoPostal")]
        public string? CodigoPostal { get; set; }
        [JsonPropertyName("telefono")]
        public string? Telefono { get; set; }
        [JsonPropertyName("email")]
        public string? Email { get; set; }
        [JsonPropertyName("claveRegistroIMSS")]
        public string? RegistroIMSS { get; set; }
        [JsonPropertyName("regimenFiscal")]
        public string? RegimenFiscal { get; set; }
        public string? vinculo { get; set; }
    }

    public class DatosClienteMoralGrid : DatosClienteMoralResponse
    {
        public string Acciones { get; set; }
    }
}
