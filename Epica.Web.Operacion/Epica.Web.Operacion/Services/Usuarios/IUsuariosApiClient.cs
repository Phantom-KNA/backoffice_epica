namespace Epica.Web.Operacion.Services.Usuarios
{
    public interface IUsuariosApiClient
    {
        Task<List<LoginResponse>> GetUsuariosRolesAsync();

    }
}
