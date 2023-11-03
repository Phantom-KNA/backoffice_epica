using Epica.Web.Operacion.Models.Common;
using Epica.Web.Operacion.Models.Request;

namespace Epica.Web.Operacion.Services.Transaccion
{
    public interface IAbonoServices
    {
        Task<MensajeResponse> PatchAutorizadorSpeiInAsync(string claveRastreo, bool rechazar);
        Task<(List<TransaccionSPEIINResponse>, int)> GetAbonosAsync(int pageNumber, int recordsTotal, int columna, bool ascendente);
        Task<(List<TransaccionSPEIINResponse>, int)> GetAbonosFilterAsync(int pageNumber, int recordsTotal, int columna, bool ascendente, List<RequestListFilters> filters);      
    }
}
