using Epica.Web.Operacion.Models.Request;

namespace Epica.Web.Operacion.Services.Log
{
    public interface IReintentadorService
    {
        Task<MensajeResponse> GetReenviarTransaccionesAsync(List<string> request);
        Task<MensajeResponse> GetReenviarTransaccionAsync(string claveRastreo);
        Task<MensajeResponse> GetDevolverTransaccionesAsync(List<string> request);
        Task<MensajeResponse> GetDevolverTransaccionAsync(string claveRastreo);
        Task<List<DevolucionesResponse>> GetTransaccionesDevolverReintentar();
        Task<MensajeResponse> GetAgregarTransaccionReintentadorAsync(string claveRastreo);
    }
}
