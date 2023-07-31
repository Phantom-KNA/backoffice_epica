namespace Epica.Web.Operacion.Models
{
    public class DashboardSummaryModel
    {
        public int UsuariosTotal { get; set; }
        public int TransaccionesTotal { get; set; }
        public int CuentasTotal { get; set; }
        public DateTime FechaActual { get; set; }
    }
}
