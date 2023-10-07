using Epica.Web.Operacion.Models.Request;

namespace Epica.Web.Operacion.Models.ViewModels
{
    public class TransaccionEditarViewModel
    {
        public ModificarTransaccionRequest TransacccionDetalles { get; set; }
        public List<DatosCatalogoResponse>? ListaMediosPago { get; set; }
    }
}
