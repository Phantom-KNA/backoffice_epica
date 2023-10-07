using Epica.Web.Operacion.Models.Request;

namespace Epica.Web.Operacion.Models.ViewModels
{
    public class TarjetasRegistroTarjetaClienteViewModel
    {
        public RegistrarTarjetaRequest TarjetasDetalles { get; set; }
        public List<DatosCatalogoResponse> ListaClientes { get; set; }
    }
}
