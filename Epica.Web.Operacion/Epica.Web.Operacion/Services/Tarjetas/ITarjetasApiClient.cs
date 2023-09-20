using Epica.Web.Operacion.Models.Entities;
using Epica.Web.Operacion.Models.Request;
using static Epica.Web.Operacion.Controllers.CuentaController;

namespace Epica.Web.Operacion.Services.Transaccion
{
    public interface ITarjetasApiClient
    {
        Task<ResumenTarjetasResponse> GetBuscarTarjetasasync(string noTarjeta);
        Task<List<TarjetasResponse>> GetTarjetasClientesAsync(int idCliente);
        Task<List<TarjetasResponse>> GetTarjetasAsync(int pageNumber, int recordsTotal);
        Task<MensajeResponse> GetRegistroTarjetaAsync(RegistrarTarjetaRequest request);
        Task<MensajeResponse> GetBloqueoTarjeta(string numeroTarjeta, int status, string token);
    }
}
