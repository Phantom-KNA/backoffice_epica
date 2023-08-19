using Epica.Web.Operacion.Models.Entities;
using Epica.Web.Operacion.Models.Request;
using static Epica.Web.Operacion.Controllers.CuentaController;

namespace Epica.Web.Operacion.Services.Transaccion
{
    public interface IClientesApiClient
    {
        Task<List<ClienteResponse>> GetClientesAsync(int pageNumber, int recordsTotal);
        Task<int> GetTotalClientesAsync();
        Task<DatosClienteResponse> GetClienteAsync(int id);
        Task<List<DocumentosClienteResponse>> GetDocumentosClienteAsync(int id);
        Task<BloqueoWebResponse> GetBloqueoWeb(BloqueoWebClienteRequest request);
        Task<BloqueoTotalResponse> GetBloqueoTotal(BloqueoTotalClienteRequest request);
        Task<RegistrarModificarClienteResponse> GetRegistroCliente(RegistroModificacionClienteRequest request);
        Task<ClienteDetailsResponse> GetDetallesCliente(int id);
    }
}
