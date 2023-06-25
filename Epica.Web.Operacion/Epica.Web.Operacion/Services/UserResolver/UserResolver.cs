using Epica.Web.Operacion.Models.Entities;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Epica.Web.Operacion.Services.UserResolver;

public class UserResolver : IUserResolver
{
    private readonly string _token;
    private readonly ApplicationUser _applicationUser;

    public UserResolver(IHttpContextAccessor context)
    {
        _applicationUser = new ApplicationUser();
        if (context.HttpContext.User != null && context.HttpContext.User.Claims.Any())
        {
            _token = ObtenerTokenWebAPI(context.HttpContext);

            if (string.IsNullOrWhiteSpace(_token))
            {
                if (context.HttpContext.User.Claims.Any(x => x.Type == "UserData"))
                {
                    _applicationUser = JsonConvert.DeserializeObject<ApplicationUser>(context.HttpContext.User.Claims.First(e => e.Type == "UserData")?.Value);
                }
            }
            else
            {
                _applicationUser = ObtenerInformacionUsuario(_token);
            }
        }
    }

    public ApplicationUser GetUser()
    {
        return _applicationUser;
    }

    public string GetToken()
    {
        return _token;
    }

    public bool HasPermission(long id)
    {
        return _applicationUser.Permisos.Any(x => x == id);
    }

    /// <summary>
    /// Método encargado de obtener el token de la  web api
    /// </summary>
    /// <param name="currentUser">Instancia actual del HTTP</param>
    /// <returns></returns>
    private string ObtenerTokenWebAPI(HttpContext currentUser)
    {
        string token = "";
        try
        {
            if (currentUser.User.HasClaim(claim => claim.Type == "TokenWebApp"))
            {
                var tokenAPI = currentUser.User.Claims.FirstOrDefault(claim => claim.Type == "TokenWebApp").Value;
                if (!string.IsNullOrEmpty(tokenAPI))
                    token = tokenAPI;
            }
        }
        catch (Exception)
        {
            return token;
        }

        return token;
    }

    private ApplicationUser ObtenerInformacionUsuario(string token)
    {
        ApplicationUser usuario = null;
        try
        {
            var jsonTokenData = new JwtSecurityTokenHandler().ReadJwtToken(token) as JwtSecurityToken;

            var jsonUsuario = jsonTokenData.Claims.FirstOrDefault(claim => claim.Type == "UserData").Value;

            if (!string.IsNullOrEmpty(jsonUsuario))
            {
                usuario = JsonConvert.DeserializeObject<ApplicationUser>(jsonUsuario);
            }
        }
        catch (Exception)
        {
            return usuario;
        }

        return usuario;
    }
}
