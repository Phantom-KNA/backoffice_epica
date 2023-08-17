namespace Epica.Web.Operacion.Models.Response
{
    public class TransaccionesResponse
    {
        public int id { get; set; }
        public string claveRastreo { get; set; }
        public string nombreOrdenante { get; set; }
        public string nombreBeneficiario { get; set; }
        public decimal monto { get; set; }
        public int estatus { get; set; }
        public string concepto { get; set; }
        public int idMedioPago { get; set; }
        public int idCuentaAhorro { get; set; }
        public string fechaAlta { get; set; }
        public string fechaActualizacion { get; set; }
    }

    public class TransaccionesResponseGrid : TransaccionesResponse
    {
        public string Acciones { get; set; }
    }
}
