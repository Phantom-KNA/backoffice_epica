using Epica.Web.Operacion.Models.Request;

namespace Epica.Web.Operacion.Services.Transaccion
{
    public interface ITransaccionesApiClient
    {
        Task<TransaccionDetailsResponse> GetTransaccionRastreoCobranzaAsync(string rastreoCobranza);
        Task<List<TransaccionesResponse>> GetTransaccionesAsync(int pageNumber, int recordsTotal);
        Task<TransaccionesResponse> GetTransaccionAsync(int idInterno);
        Task<int> GetTotalTransaccionesAsync();
        Task<TransaccionesDetailsgeneralResponse> GetTransaccionesCuentaAsync(int idCuenta);
        Task<MensajeResponse> GetRegistroTransaccion(RegistrarTransaccionRequest request);
        Task<MensajeResponse> GetModificarTransaccion(ModificarTransaccionRequest request);
        Task<TransaccionDetailsResponse> GetTransaccionDetalleAsync(int idCuenta);
        Task<TransaccionDetailsResponse> GetTransaccionDetalleByCobranzaAsync(string cobranzaRef);
        Task<TransaccionesDetailsgeneralResponse> GetTransaccionesClienteAsync(int idCliente);
        Task<MensajeResponse> GetInsertaTransaccionesBatchAsync(List<CargaBachRequest> request);

    }
}
