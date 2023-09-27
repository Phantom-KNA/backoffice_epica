using Epica.Web.Operacion.Models.Common;
using Epica.Web.Operacion.Models.Entities;
using static Epica.Web.Operacion.Controllers.CuentaController;

namespace Epica.Web.Operacion.Services.Transaccion
{
    public interface ICuentaApiClient
    {
        Task<List<ResumenCuentasResponse>> GetListaByNumeroCuentaAsync(string noCuenta);
        Task<(List<CuentasResponse>, int)> GetCuentasAsync(int pageNumber, int recordsTotal, int columna, bool ascendente);
        Task<(List<CuentasResponse>, int)> GetCuentasFilterAsync(int pageNumber, int recordsTotal, int columna, bool ascendente, List<RequestListFilters> filters);
        Task<int> GetTotalCuentasAsync();
        Task<List<CobranzaReferenciadaResponse>> GetCobranzaReferenciadaAsync(int id);
        Task<List<CuentasResponse>> GetCuentasByClienteAsync(int idCliente);
        Task<List<DatosCuentaResponse>> GetDetalleCuentasAsync(string idCliente);
        Task<List<CuentaEspecificaResponse>> GetDetalleCuentasSinAsignarAsync(string NumCuenta);
        Task<List<CuentasResponse>> GetCuentasFiltroAsync(string Valor, int TipoFiltro);
        Task<MensajeResponse> BloqueoCuentaAsync(int idCuenta, int estatus, int nip, string softoken);
        Task<MensajeResponse> BloqueoCuentaSpeiOutAsync(int idCuenta, int estatus, int nip, string softoken);
    }
}
