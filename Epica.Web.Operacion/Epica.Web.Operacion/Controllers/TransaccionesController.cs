using Epica.Web.Operacion.Models.Common;
using Epica.Web.Operacion.Services.Transaccion;
using Epica.Web.Operacion.Utilities;
using Newtonsoft.Json;
using System.Threading;
using static Epica.Web.Operacion.Controllers.CuentaController;

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

        #region "Funciones"
        public IActionResult Index(string AccountID = "")
        {
            ViewBag.AccountID = AccountID;
            return View();
        }

        public IActionResult Registro()
        {
            return View("~/Views/Transacciones/Registro.cshtml");
        }
        public async Task<IActionResult> Transacciones()
        {
            var recibir = await _transaccionesApiClient.GetTransaccionesAsync(1,100);
            return Json(recibir);
        }

        //public async Task<IActionResult> Transaccion(int idInterno)
        //{
        //    var recibir = await _transaccionesApiClient.GetTransaccionAsync(idInterno);
        //    return Json(recibir);
        //}


        [HttpPost]
        public async Task<JsonResult> Consulta(List<RequestListFilters> filters)
        {
            var request = new RequestList();

            int totalRecord = 0;
            int filterRecord = 0;

            var draw = Request.Form["draw"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            request.Pagina = skip / pageSize + 1;
            request.Registros = pageSize;
            request.Busqueda = Request.Form["search[value]"].FirstOrDefault();
            request.ColumnaOrdenamiento = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            request.Ordenamiento = Request.Form["order[0][dir]"].FirstOrDefault();

            var gridData = new ResponseGrid<ResumenTransaccionResponseGrid>();
            List<TransaccionesResponse> ListPF = new List<TransaccionesResponse>();

            ListPF = await _transaccionesApiClient.GetTransaccionesAsync(1,100);

            //Entorno local de pruebas
            //ListPF = GetList();

            var List = new List<ResumenTransaccionResponseGrid>();
            foreach (var row in ListPF)
            {
                List.Add(new ResumenTransaccionResponseGrid
                {
                    id = row.id,
                    claveRastreo = row.claveRastreo,
                    nombreOrdenante = row.nombreOrdenante,
                    nombreBeneficiario = row.nombreBeneficiario,
                    monto = row.monto,
                    estatus = row.estatus,
                    concepto = row.concepto,
                    idMedioPago = row.idMedioPago,
                    idCuentaAhorro = row.idCuentaAhorro,
                    fechaAlta = row.fechaAlta,
                    Acciones = await this.RenderViewToStringAsync("~/Views/Transacciones/_Acciones.cshtml", row)
                });
            }

            //Aplicacion de Filtros temporal, 
            //var filtroid = filters.FirstOrDefault(x => x.Key == "idInterno");
            //var filtronombreClaveRastreo = filters.FirstOrDefault(x => x.Key == "claveRastreo");
            //var filtroNombreCuenta = filters.FirstOrDefault(x => x.Key == "nombreCuenta");
            //var filtroInstitucion = filters.FirstOrDefault(x => x.Key == "institucion");
            //var filtroMonto = filters.FirstOrDefault(x => x.Key == "monto");
            //var filtroEstatus = filters.FirstOrDefault(x => x.Key == "estatus");
            //var filtroConcepto = filters.FirstOrDefault(x => x.Key == "concepto");
            //var filtroMedioPago = filters.FirstOrDefault(x => x.Key == "medioPago");
            //var filtroTipo = filters.FirstOrDefault(x => x.Key == "tipo");
            //var filtroFecha = filters.FirstOrDefault(x => x.Key == "fecha");

            //if (filtroid.Value != null)
            //{
            //    List = List.Where(x => x.IdInterno == Convert.ToInt32(filtroid.Value)).ToList();
            //}

            //if (filtronombreClaveRastreo.Value != null)
            //{
            //    List = List.Where(x => x.ClaveRastreo.Contains(Convert.ToString(filtronombreClaveRastreo.Value))).ToList();
            //}

            //if (filtroNombreCuenta.Value != null)
            //{
            //    List = List.Where(x => x.NombreCuenta == Convert.ToString(filtroNombreCuenta.Value)).ToList();
            //}

            //if (filtroInstitucion.Value != null)
            //{
            //    List = List.Where(x => x.Institucion == Convert.ToString(filtroInstitucion.Value)).ToList();
            //}

            //if (filtroMonto.Value != null)
            //{
            //    List = List.Where(x => x.Monto.ToString() == Convert.ToString(filtroMonto.Value)).ToList();
            //}

            //if (filtroEstatus.Value != null)
            //{
            //    List = List.Where(x => x.Estatus == Convert.ToString(filtroEstatus.Value)).ToList();
            //}

            //if (filtroConcepto.Value != null)
            //{
            //    List = List.Where(x => x.Concepto == Convert.ToString(filtroConcepto.Value)).ToList();
            //}

            //if (filtroMedioPago.Value != null)
            //{
            //    List = List.Where(x => x.MedioPago.ToString() == Convert.ToString(filtroMedioPago.Value)).ToList();
            //}

            //if (filtroTipo.Value != null)
            //{
            //    List = List.Where(x => x.Tipo.ToString() == Convert.ToString(filtroTipo.Value)).ToList();
            //}

            //if (filtroFecha.Value != null)
            //{
            //    List = List.Where(x => x.Fecha.ToString() == Convert.ToString(filtroFecha.Value)).ToList();
            //}

            gridData.Data = List;
            gridData.RecordsTotal = List.Count;
            gridData.Data = gridData.Data.Skip(skip).Take(pageSize).ToList();
            filterRecord = string.IsNullOrEmpty(request.Busqueda) ? gridData.RecordsTotal ?? 0 : gridData.Data.Count;
            gridData.RecordsFiltered = filterRecord;
            gridData.Draw = draw;

            return Json(gridData);
        }
        #endregion

        #region "Modelos"
        public class RequestListFilters
        {
            [JsonProperty("key")]
            public string Key { get; set; }
            [JsonProperty("value")]
            public string Value { get; set; }
        }
        public class ResumenTransaccionResponse
        {
            public int id { get; set; }
            public string claveRastreo { get; set; }
            public string nombreOrdenante { get; set; }
            public string nombreBeneficiario { get; set; }
            public decimal monto { get; set; }
            public int estatus { get; set; }
            public string concepto { get; set; }
            public int idMedioPago { get; set; }
            public int idCuentaAhorro { get; set; }
            public DateTime fechaAlta { get; set; }
        }
        public class ResumenTransaccionResponseGrid : ResumenTransaccionResponse
        {
            public string Acciones { get; set; }
        }
        #endregion
    }
}