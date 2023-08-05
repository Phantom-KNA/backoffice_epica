using Epica.Web.Operacion.Models.Entities;
using static Epica.Web.Operacion.Controllers.CuentaController;

namespace Epica.Web.Operacion.Services.Transaccion
{
    public interface IUsuariosApiClient
    {
        Task<List<UserResponse>> GetUsuariosAsync(int pageNumber, int recordsTotal);
        Task<int> GetTotalUsuariosAsync();
        Task<UserResponse> GetUsuarioAsync(int id);
        Task<List<DocumentosUserResponse>> GetDocumentosUsuarioAsync(int id);

    }
}
