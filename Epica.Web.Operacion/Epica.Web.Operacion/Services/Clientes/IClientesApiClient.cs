using Epica.Web.Operacion.Models.Entities;
using Epica.Web.Operacion.Models.Request;
using static Epica.Web.Operacion.Controllers.CuentaController;

namespace Epica.Web.Operacion.Services.Transaccion
{
    public interface IClientesApiClient
    {
        Task<List<DatosClienteEntity>> GetDetallesClientesByNombresAsync(string nombres);
        Task<object> GetClientesbyNombreAsync(string nombreCliente);
        Task<List<ClienteResponse>> GetClientesAsync(int pageNumber, int recordsTotal);
        Task<int> GetTotalClientesAsync();
        Task<ClienteDetailsResponse> GetClienteAsync(int id);
        Task<DocumentosClienteDetailsResponse> GetDocumentosClienteAsync(int id);
        Task<BloqueoWebResponse> GetBloqueoWeb(BloqueoWebClienteRequest request);
        Task<BloqueoTotalResponse> GetBloqueoTotal(BloqueoTotalClienteRequest request);
        Task<MensajeResponse> GetRegistroCliente(RegistroModificacionClienteRequest request);
        Task<ClienteDetailsResponse> GetDetallesCliente(int id);
        Task<MensajeResponse> GetModificarCliente(RegistroModificacionClienteRequest request);
        Task<MensajeResponse> GetRegistroAsignacionCuentaCliente(AsignarCuentaResponse request);
        Task<MensajeResponse> GetRegistroDesvincularCuentaCliente(DesvincularCuentaResponse request);
        //Task<RegistrarModificarClienteResponse> GetModificaCliente(RegistroModificacionClienteRequest request);
        Task<MensajeResponse> GetRestablecerContraseñaCorreo(string correo);
        Task<MensajeResponse> GetRestablecerContraseñaTelefono(string telefono);
    }
}
