using Epica.Web.Operacion.Models.Entities;
using Epica.Web.Operacion.Models.Request;
using Epica.Web.Operacion.Models.Response;

namespace Epica.Web.Operacion.Models.ViewModels
{
    public class ClientesRegistroViewModel
    {
        public RegistroModificacionClienteRequest? ClientesDetalles { get; set; }
        public List<DatosCatalogoResponse>? ListaEmpresas { get; set; }
        public List<DatosCatalogoResponse>? ListaRoles { get; set; }
        public List<DatosCatalogoResponse>? ListaOcupaciones { get; set; }
        public List<DatosCatalogoResponse>? ListaPaises { get; set; }
        public List<DatosCatalogoResponse>? ListaNacionalidades { get; set; }
    }

}
