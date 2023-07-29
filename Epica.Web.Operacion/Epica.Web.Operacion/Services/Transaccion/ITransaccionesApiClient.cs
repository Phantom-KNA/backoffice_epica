namespace Epica.Web.Operacion.Services.Transaccion
{
    public interface ITransaccionesApiClient
    {
        Task<List<TransaccionesResponse>> GetTransaccionesAsync(int pageNumber, int recordsTotal);

        Task<TransaccionesResponse> GetTransaccionAsync(int idInterno);
        Task<int> GetTotalTransaccionesAsync();

    }
}
