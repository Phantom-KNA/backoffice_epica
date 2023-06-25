using System.Security.Claims;

namespace Epica.Web.Operacion.Models.Entities;

/// <summary>
/// Clase del modelo genererico para la creacion de un token
/// </summary>
public class GenericToken
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public List<Claim> Claims { get; set; }
    public string KeySecret { get; set; }
    public int TimeExpire { get; set; }
}
