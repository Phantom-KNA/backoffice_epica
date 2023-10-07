using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Models.Request
{
    public class RegistroModificacionClienteRequest
    {
        [JsonPropertyName("idCliente")]
        public int IdCliente { get; set; }
        [JsonPropertyName("nombre")]
        public string? Nombre { get; set; }
        [JsonPropertyName("apellidoPaterno")]
        public string? ApellidoPaterno { get; set; }
        [JsonPropertyName("apellidoMaterno")]
        public string? ApellidoMaterno { get; set; }
        [JsonPropertyName("email")]
        public string? Email { get; set; }
        [JsonPropertyName("curp")]
        public string? Curp { get; set; }
        [JsonPropertyName("rfc")]
        public string? RFC { get; set; }
        [JsonPropertyName("ine")]
        public string? INE { get; set; }
        [JsonPropertyName("fechaNacimiento")]
        public DateTime? FechaNacimiento { get; set; }
        [JsonPropertyName("observaciones")]
        public string? Observaciones { get; set; }
        [JsonPropertyName("paisNacimiento")]
        public string? PaisNacimiento { get; set; }
        [JsonPropertyName("sexo")]
        public string? Sexo { get; set; }
        [JsonPropertyName("idOcupacion")]
        public int IdOcupacion { get; set; }
        [JsonPropertyName("idNacionalidad")]
        public int IdNacionalidad { get; set; }
        [JsonPropertyName("fiel")]
        public string? Fiel { get; set; }
        [JsonPropertyName("idPais")]
        public int IdPais { get; set; }
        [JsonPropertyName("ingresoMensual")]
        public string? IngresoMensual { get; set; }
        [JsonPropertyName("montoMaximo")]
        public string? MontoMaximo { get; set; }
        [JsonPropertyName("telefono")]
        public string? Telefono { get; set; }
        [JsonPropertyName("telefonoTipo")]
        public string? TelefonoTipo { get; set; }
        [JsonPropertyName("calle")]
        public string? Calle { get; set; }
        [JsonPropertyName("calleNumero")]
        public string? CalleNumero { get; set; }
        [JsonPropertyName("entreCallePrimera")]
        public string? EntreCallePrimera { get; set; }
        [JsonPropertyName("entreCalleSegunda")]
        public string? EntreCalleSegunda { get; set; }
        [JsonPropertyName("colonia")]
        public string? Colonia { get; set; }
        [JsonPropertyName("delegacionMunicipio")]
        public string? DelegacionMunicipio { get; set; }
        [JsonPropertyName("codigoPostal")]
        public string? CodigoPostal { get; set; }
        [JsonPropertyName("ciudadEstado")]
        public string? CiudadEstado { get; set; }
        [JsonPropertyName("noInterior")]
        public string? NoInterior { get; set; }
        [JsonPropertyName("puesto")]
        public string? Puesto { get; set; }
        [JsonPropertyName("empresa")]
        public int Empresa { get; set; }
        [JsonPropertyName("apoderadoLegal")]
        public int ApoderadoLegal { get; set; }
        [JsonPropertyName("rol")]
        public string? Rol { get; set; }
    }
}
