using Epica.Web.Operacion.Helpers;
using Epica.Web.Operacion.Models.Common;
using Epica.Web.Operacion.Models.Entities;
using Epica.Web.Operacion.Models.Request;
using Epica.Web.Operacion.Models.ViewModels;
using Epica.Web.Operacion.Services.Catalogos;
using Epica.Web.Operacion.Services.Log;
using Epica.Web.Operacion.Services.Transaccion;
using Epica.Web.Operacion.Utilities;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using static Epica.Web.Operacion.Controllers.CuentaController;

namespace Epica.Web.Operacion.Controllers
{
    [Authorize]
    public class TransaccionesController :  Controller
    {
        private readonly UserContextService _userContextService;
        #region "Locales"
        private readonly ITransaccionesApiClient _transaccionesApiClient;//Transacciones
        private readonly ICatalogosApiClient _catalogosApiClient;
        private readonly ILogsApiClient _logsApiClient;
        #endregion

        #region "Constructores"
        public TransaccionesController(ITransaccionesApiClient transaccionesApiClient,
            UserContextService userContextService,
            ICatalogosApiClient catalogosApiClient,
            ILogsApiClient logsApiClient
            )
        {
            _userContextService = userContextService;
            _transaccionesApiClient = transaccionesApiClient;
            _catalogosApiClient = catalogosApiClient;
            _logsApiClient = logsApiClient;
        }
        #endregion

        #region "Funciones"
        [Authorize]
        public IActionResult Index(string AccountID = "")
        {
            var loginResponse = _userContextService.GetLoginResponse();
            var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Transacciones" && modulo.Ver == 0);
            if (validacion == true || validacion != null)
            {
                ViewBag.AccountID = AccountID;
                return View(loginResponse);
            }


            return NotFound();
        }

        [Authorize]
        public async Task<ActionResult> Registro()
        {
            var loginResponse = _userContextService.GetLoginResponse();
            var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Transacciones" && modulo.Insertar == 0);
            if (validacion == true || validacion != null)
            {
                var listaMediosPago = await _catalogosApiClient.GetMediosPagoAsync();

                TransaccionesRegistroViewModel transaccionesRegistroViewModel = new TransaccionesRegistroViewModel
                {
                    TransacccionDetalles = new RegistrarTransaccionRequest(),
                    ListaMediosPago = listaMediosPago
                };

                ViewBag.Accion = "RegistrarTransaccion";
                ViewBag.TituloForm = "Crear nueva transacción";

                return View(transaccionesRegistroViewModel);
            }


            return NotFound();
        }

        [Authorize]
        public async Task<IActionResult> Transacciones()
        {
            var loginResponse = _userContextService.GetLoginResponse();
            var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Transacciones" && modulo.Ver == 0);
            if (validacion == true || validacion != null)
            {
                var recibir = await _transaccionesApiClient.GetTransaccionesAsync(1, 100);
                return Json(recibir);
            }
            return NotFound();

        }

        [Authorize]
        [HttpPost]
        public async Task<JsonResult> Consulta(List<RequestListFilters> filters, string idAccount = "")
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
            if (!string.IsNullOrEmpty(request.Busqueda))
            {
                List = List.Where(x =>
                (x.id.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
                (x.claveRastreo?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
                (x.nombreOrdenante?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
                (x.nombreBeneficiario?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
                (x.monto.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
                (x.estatus.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
                (x.concepto?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
                (x.idMedioPago.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
                (x.idCuentaAhorro.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
                (x.fechaAlta.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) 
                ).ToList();
            }
            //Aplicacion de Filtros temporal, 
            var filtronombreClaveRastreo = filters.FirstOrDefault(x => x.Key == "claveRastreo");
            var filtroNombreOrdenante = filters.FirstOrDefault(x => x.Key == "nombreOrdenante");
            var filtroNombreBeneficiario = filters.FirstOrDefault(x => x.Key == "nombreBeneficiario");
            var filtroConcepto = filters.FirstOrDefault(x => x.Key == "concepto");
            var filtroMonto = filters.FirstOrDefault(x => x.Key == "monto");
            var filtroFecha = filters.FirstOrDefault(x => x.Key == "fecha");
            var filtroEstatus = filters.FirstOrDefault(x => x.Key == "estatus");

            if (filtronombreClaveRastreo.Value != null)
            {
                List = List.Where(x => x.claveRastreo.Contains(Convert.ToString(filtronombreClaveRastreo.Value))).ToList();
            }

            if (filtroNombreOrdenante.Value != null)
            {
                List = List.Where(x => x.nombreOrdenante == Convert.ToString(filtroNombreOrdenante.Value)).ToList();
            }

            if (filtroNombreBeneficiario.Value != null)
            {
                List = List.Where(x => x.nombreBeneficiario == Convert.ToString(filtroNombreBeneficiario.Value)).ToList();
            }

            if (filtroConcepto.Value != null)
            {
                List = List.Where(x => x.concepto == Convert.ToString(filtroConcepto.Value)).ToList();
            }

            if (filtroMonto.Value != null)
            {
                List = List.Where(x => x.monto.ToString() == Convert.ToString(filtroMonto.Value)).ToList();
            }

            if (filtroFecha.Value != null)
            {
                List = List.Where(x => x.fechaAlta.ToString() == Convert.ToString(filtroFecha.Value)).ToList();
            }

            if (filtroEstatus.Value != null)
            {
                if (filtroEstatus.Value == "1") {

                    var estatusList = new[] { "1", "2" };
                    List = List.Where(x => estatusList.Contains(Convert.ToString(x.estatus))).ToList();

                } else {
                    List = List.Where(x => x.estatus == Convert.ToInt32(filtroEstatus.Value)).ToList();
                }
            }

            gridData.Data = List;
            gridData.RecordsTotal = List.Count;
            gridData.Data = gridData.Data.Skip(skip).Take(pageSize).ToList();
            filterRecord = string.IsNullOrEmpty(request.Busqueda) ? gridData.RecordsTotal ?? 0 : gridData.Data.Count;
            gridData.RecordsFiltered = filterRecord;
            gridData.Draw = draw;

            return Json(gridData);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RegistrarTransaccion(TransaccionesRegistroViewModel model)
        {
            var loginResponse = _userContextService.GetLoginResponse();
            var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Transacciones" && modulo.Insertar == 0);
            if (validacion == true || validacion != null)
            {
                RegistrarModificarTransaccionResponse response = new RegistrarModificarTransaccionResponse();

                try
                {
                    response = await _transaccionesApiClient.GetRegistroTransaccion(model.TransacccionDetalles);

                    LogRequest logRequest = new LogRequest
                    {
                        IdUser = loginResponse.IdUsuario.ToString(),
                        Modulo = "Transacciones",
                        Fecha = DateTime.Now,
                        NombreEquipo = Environment.MachineName,
                        Accion = "Insertar",
                        Ip = PublicIpHelper.GetPublicIp() ?? "0.0.0.0",
                        Envio = JsonConvert.SerializeObject(model.TransacccionDetalles),
                        Respuesta = response.error.ToString(),
                        Error = response.error ? JsonConvert.SerializeObject(response.detalle) : string.Empty,
                        IdRegistro = 0
                    };
                    await _logsApiClient.InsertarLogAsync(logRequest);

                    if (response.codigo == "200")
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Registro");
                    }
                }
                catch (Exception ex)
                {
                    return View();
                }
            }

            return NotFound();
        }

        [Authorize]
        public async Task<ActionResult> Modificar(int id)
        {
            var loginResponse = _userContextService.GetLoginResponse();
            var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Transacciones" && modulo.Editar == 0);
            if (validacion == true || validacion != null)
            {
                try
                {
                    var transaccionResponse = await _transaccionesApiClient.GetTransaccionAsync(id);
                    RegistrarTransaccionRequest transaccionDetalles = new RegistrarTransaccionRequest()
                    {
                        ClaveRastreo = transaccionResponse.claveRastreo,
                        Concepto = transaccionResponse.concepto,
                        NombreBeneficiario = transaccionResponse.nombreBeneficiario,
                        Monto = transaccionResponse.monto,
                        medioPago = transaccionResponse.idMedioPago,
                        NombreOrdenante = transaccionResponse.nombreOrdenante,
                        IdTrasaccion = transaccionResponse.id
                    };
                    var listaMediosPago = await _catalogosApiClient.GetMediosPagoAsync();

                    if (listaMediosPago == null)
                    {
                        return RedirectToAction("Error404", "Error");
                    }

                    TransaccionesRegistroViewModel transaccionesRegistroViewModel = new TransaccionesRegistroViewModel
                    {
                        TransacccionDetalles = transaccionDetalles,
                        ListaMediosPago = listaMediosPago
                    };

                    ViewBag.Accion = "ModificarTransaccion";
                    ViewBag.TituloForm = "Modificar transacción";
                    return View("~/Views/Transacciones/Registro.cshtml", transaccionesRegistroViewModel);
                }
                catch (Exception)
                {
                    return RedirectToAction("Error", "Error");
                }
            }

            return NotFound();

        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ModificarTransaccion(TransaccionesRegistroViewModel model)
        {
            var loginResponse = _userContextService.GetLoginResponse();
            var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Transacciones" && modulo.Editar == 0);
            if (validacion == true || validacion != null)
            {
                RegistrarModificarTransaccionResponse response = new RegistrarModificarTransaccionResponse();

                try
                {
                    response = await _transaccionesApiClient.GetModificarTransaccion(model.TransacccionDetalles);

                    LogRequest logRequest = new LogRequest
                    {
                        IdUser = loginResponse.IdUsuario.ToString(),
                        Modulo = "Clientes",
                        Fecha = DateTime.Now,
                        NombreEquipo = Environment.MachineName,
                        Accion = "Editar",
                        Ip = PublicIpHelper.GetPublicIp() ?? "0.0.0.0",
                        Envio = JsonConvert.SerializeObject(model.TransacccionDetalles),
                        Respuesta = response.error.ToString(),
                        Error = response.error ? JsonConvert.SerializeObject(response.detalle) : string.Empty,
                        IdRegistro = model.TransacccionDetalles.IdTrasaccion
                    };
                    await _logsApiClient.InsertarLogAsync(logRequest);

                    if (response.codigo == "200")
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Registro");
                    }
                }
                catch (Exception ex)
                {
                    return View();
                }
            }

            return NotFound();
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
            public string fechaAlta { get; set; }
            public string fechaActualizacion { get; set; }
        }
        public class ResumenTransaccionResponseGrid : ResumenTransaccionResponse
        {
            public string Acciones { get; set; }
        }
        #endregion
    }
}