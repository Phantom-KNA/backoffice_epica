using Epica.Web.Operacion.Models.Common;
using Epica.Web.Operacion.Models.Entities;
using Epica.Web.Operacion.Models.Request;
using static Epica.Web.Operacion.Controllers.CuentaController;

namespace Epica.Web.Operacion.Services.Transaccion
{
    public interface ITarjetasApiClient
    {
        Task<ResumenTarjetasResponse> GetBuscarTarjetasasync(string noTarjeta);
        Task<List<TarjetasResponse>> GetTarjetasClientesAsync(int idCliente);
        Task<(List<TarjetasResponse>, int)> GetTarjetasAsync(int pageNumber, int recordsTotal, int columna, bool ascendente);
        Task<(List<TarjetasResponse>, int)> GetTarjetasFilterAsync(int pageNumber, int recordsTotal, int columna, bool ascendente, List<RequestListFilters> filters);
        Task<MensajeResponse> GetRegistroTarjetaAsync(RegistrarTarjetaRequest request);
        Task<MensajeResponse> GetBloqueoTarjeta(string numeroTarjeta, int status, string token);
    }
}
