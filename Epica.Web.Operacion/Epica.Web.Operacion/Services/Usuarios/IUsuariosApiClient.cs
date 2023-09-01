using Epica.Web.Operacion.Models.Request;

namespace Epica.Web.Operacion.Services.Usuarios
{
    public interface IUsuariosApiClient
    {
        Task<List<LoginResponse>> GetUsuariosRolesAsync();
        Task<List<UserRolPermisosResponse>> GetUsuariosRolesVistaAsync();
        Task<List<UserPermisosResponse>> GetRolPermisosEspecificoAsync(int idRol, string moduloAcceso);
        Task<MensajeResponse> GetAsignarRolPermisos(UserPermisoRequest request);
        Task<List<UsuariosVinculadosResponse>> GetAsignarRolPermisosAsync();
        Task<List<DatosCatalogoResponse>> GetUsuarioPorNombreAsync(string nombre);
        Task<MensajeResponse> GetRegistroAsignacionUsuarioRol(int idRol, int idUsuario);
    }
}
