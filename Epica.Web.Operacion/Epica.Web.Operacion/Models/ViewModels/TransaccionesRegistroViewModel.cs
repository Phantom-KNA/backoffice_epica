using Epica.Web.Operacion.Models.Request;

namespace Epica.Web.Operacion.Models.ViewModels
{
    public class TransaccionesRegistroViewModel
    {
        public RegistrarTransaccionRequest TransacccionDetalles { get; set; }
        public List<DatosCatalogoResponse>? ListaMediosPago { get; set; }

    }
}
