using Epica.Web.Operacion.Models.Entities;

namespace Epica.Web.Operacion.Services.Transaccion
{
    public interface ITransaccionesApiClient
    {
        Task<List<Transacciones>> GetTransaccionesAsync();

        Task<Transacciones> GetTransaccionAsync(int idInterno);
    }
}
