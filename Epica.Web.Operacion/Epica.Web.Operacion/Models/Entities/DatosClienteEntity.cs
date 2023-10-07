using System.ComponentModel.DataAnnotations;

namespace Epica.Web.Operacion.Models.Entities
{
    public class DatosClienteEntity
    {
        public int id_cliente { get; set; }
        public string? nombre { get; set; }
        public string? apellido_paterno { get; set; }
        public string? apellido_materno { get; set; }
        public string? telefono { get; set; }
        public string? telefono_recados { get; set; }
        public string? email { get; set; }
        public string? curp { get; set; }
        public string? organizacion { get; set; }
        public DateTime fecha_nacimiento { get; set; }
        public string? entidad_nacimiento { get; set; }
        public int? id_nacionalidad { get; set; }
        public string? membresia { get; set; }
        public string? sexo { get; set; }
        public string? ine { get; set; }
        public string? rfc { get; set; }
        public string? pais_nacimiento { get; set; }
        public string? observaciones { get; set; }
        public int? id_ocupacion { get; set; }
        public string? nacionalidad { get; set; }
        public string? fiel { get; set; }
        public string? nss { get; set; }
        public string? colonia { get; set; }
        public string? calle_numero { get; set; }
        public string? numero_int { get; set; }
        public string? municipio { get; set; }
        public string? estado { get; set; }
        public string? cp { get; set; }
        public string? calle { get; set; }
        public string? calle_secundaria { get; set; }
        public string? calle_secundaria_2 { get; set; }
        public int? tipo_vivienda { get; set; }
        public string? tiempo_vivienda { get; set; }
        public string? tipo_trabajador { get; set; }
        public string? puesto { get; set; }
        public double? salario_neto_mensual { get; set; }
        public string? antiguedad_laboral { get; set; }
        public int? active { get; set; }
        public int? id_empresa { get; set; }
        public string? rol { get; set; }
        public string? monto_maximo { get; set; }
        public int? id_pais { get; set; }
        public string? estatusWeb { get; set; }
    }
}
