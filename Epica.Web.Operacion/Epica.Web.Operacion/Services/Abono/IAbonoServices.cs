using Epica.Web.Operacion.Models.Common;
using Epica.Web.Operacion.Models.Request;

namespace Epica.Web.Operacion.Services.Transaccion
{
    public interface IAbonoServices
    {
        Task<(List<TransaccionSPEIINResponse>, int)> GetAbonosAsync(int pageNumber, int recordsTotal, int columna, bool ascendente);
        Task<(List<TransaccionesResponse>, int)> GetTransaccionesFilterAsync(int pageNumber, int recordsTotal, int columna, bool ascendente, List<RequestListFilters> filters);
        
    }
}
