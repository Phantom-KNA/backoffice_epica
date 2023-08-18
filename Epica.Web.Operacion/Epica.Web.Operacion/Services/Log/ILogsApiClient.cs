using Epica.Web.Operacion.Models.Request;

namespace Epica.Web.Operacion.Services.Log
{
    public interface ILogsApiClient
    {
        Task<MensajeResponse> InsertarLogAsync(LogRequest logRequest);

    }
}
