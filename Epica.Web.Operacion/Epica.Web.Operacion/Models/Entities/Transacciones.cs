namespace Epica.Web.Operacion.Models.Entities
{
    public class Transacciones
    {
        public int idInterno { get; set; }
        public string claveRastreo { get; set; }
        public string nombreCuenta { get; set; }
        public string institucion { get; set; }
        public decimal monto { get; set; }
        public string estatus { get; set; }
        public string concepto { get; set; }
        public int medioPago { get; set; }
        public int tipo { get; set; }
        public DateTime fecha { get; set; }
    } 
}
