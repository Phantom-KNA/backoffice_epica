using Epica.Web.Operacion.Models.Entities;

namespace Epica.Web.Operacion.Services.UserResolver;

public interface IUserResolver
{
    ApplicationUser GetUser();

    public string GetToken();

    bool HasPermission(long id);
}
