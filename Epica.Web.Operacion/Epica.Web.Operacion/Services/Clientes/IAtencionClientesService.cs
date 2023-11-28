using Epica.Web.Operacion.Models.Common;
using Epica.Web.Operacion.Models.Entities;
using Epica.Web.Operacion.Models.Request;
using static Epica.Web.Operacion.Controllers.CuentaController;

namespace Epica.Web.Operacion.Services.Transaccion
{
    public interface IAtencionClientesService
    {
        Task<(List<DatosClienteMoralResponse>, int)> GetPersonasMoralesFilterAsync(int pageNumber, int recordsTotal, int columna, bool ascendente, List<RequestListFilters> filters);

        //Task<ClienteDetailsResponse> GetDetallesGeneralesClienteAsync(int id);
        Task<(List<DatosClienteMoralResponse>, int)> GetPersonasMoralesAsync(int pageNumber, int recordsTotal, int columna, bool ascendente);
        Task<DatosClienteMoralResponse> GetDetallesPersonaMoral(int id);
        Task<(List<EmpresaClienteResponse>, int)> GetEmpleadosPersonaMoralAsync(int pageNumber, int recordsTotal, int id);
    }
}
