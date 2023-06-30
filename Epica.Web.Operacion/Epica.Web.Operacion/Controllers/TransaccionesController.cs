using Epica.Web.Operacion.Services.Transaccion;

namespace Epica.Web.Operacion.Controllers
{
    public class TransaccionesController :  Controller
    {
        #region "Locales"
        private readonly ITransaccionesApiClient _transaccionesApiClient;//Transacciones
        #endregion

        #region "Constructores"
        public TransaccionesController(ITransaccionesApiClient transaccionesApiClient)
        {
            _transaccionesApiClient = transaccionesApiClient;
        }
        #endregion

        #region
        public async Task<IActionResult> Transacciones()
        {
            var recibir = await _transaccionesApiClient.GetTransaccionesAsync();
            return Json(recibir);
        }

        public async Task<IActionResult> Transaccion(int idInterno)
        {
            var recibir = await _transaccionesApiClient.GetTransaccionAsync(idInterno);
            return Json(recibir);
        }

        public IActionResult Index() {
            return View();
        }
        #endregion
    }
}
