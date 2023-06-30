using Epica.Web.Operacion.Models.Entities;
using static Epica.Web.Operacion.Controllers.CuentaController;

namespace Epica.Web.Operacion.Services.Transaccion
{
    public interface ICuentaApiClient
    {
        Task<List<CuentasResponse>> GetCuentasAsync();
    }
}
