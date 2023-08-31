using Epica.Web.Operacion.Models.Entities;
using static Epica.Web.Operacion.Controllers.CuentaController;

namespace Epica.Web.Operacion.Services.Transaccion
{
    public interface ICuentaApiClient
    {
        Task<List<ResumenCuentasResponse>> GetListaByNumeroCuentaAsync(string noCuenta);
        Task<List<CuentasResponse>> GetCuentasAsync(int pageNumber, int recordsTotal);
        Task<int> GetTotalCuentasAsync();
        Task<List<CobranzaReferenciadaResponse>> GetCobranzaReferenciadaAsync(int id);
        Task<List<CuentasResponse>> GetCuentasByClienteAsync(int idCliente);
        Task<List<DatosCuentaResponse>> GetDetalleCuentasAsync(string idCliente);
        Task<List<CuentaEspecificaResponse>> GetDetalleCuentasSinAsignarAsync(string NumCuenta);
    }
}
