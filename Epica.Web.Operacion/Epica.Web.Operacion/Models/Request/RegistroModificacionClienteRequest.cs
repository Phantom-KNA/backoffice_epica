using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Models.Request
{
    public class RegistroModificacionClienteRequest
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? ApellidoPaterno { get; set; }      
        public string? ApellidoMaterno { get; set; }
        public string? Email { get; set; }
        public string? Curp { get; set; }
        public string? RFC { get; set; }
        public string? INE { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string? Observaciones { get; set; }
        public string? PaisNacimiento { get; set; }
        public string? Sexo { get; set; }
        public int IdOcupacion { get; set; }
        public int IdNacionalidad { get; set; }
        public string? Fiel { get; set; }
        public int IdPais { get; set; }
        public string? IngresoMensual { get; set; }
        public string? MontoMaximo { get; set; }
        public string? Telefono { get; set; }
        public string? TelefonoTipo { get; set; }
        public string? Calle { get; set; }
        public string? CalleNumero { get; set; }
        public string? EntreCallePrimera { get; set; }
        public string? EntreCalleSegunda { get; set; }
        public string? Colonia { get; set; }
        public string? DelegacionMunicipio { get; set; }
        public string? CodigoPostal { get; set; }
        public string? CiudadEstado { get; set; }
        public string? NoInterior { get; set; }
        public string? Puesto { get; set; }
        public int Empresa { get; set; }
        public int ApoderadoLegal { get; set; }
        public string? Rol { get; set; }
    }
}
