using Epica.Web.Operacion.Helpers;
using Epica.Web.Operacion.Models;
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
using System.Text.RegularExpressions;
using ExcelDataReader;
using System.Data;
using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.IdentityModel.Tokens;

namespace Epica.Web.Operacion.Controllers
{
    [Authorize]
    public class TransaccionesController : Controller
    {
        #region "Locales"
        private readonly UserContextService _userContextService;
        private readonly IReintentadorService _reintentadorService;
        private readonly ITransaccionesApiClient _transaccionesApiClient;//Transacciones
        private readonly ICatalogosApiClient _catalogosApiClient;
        private readonly ILogsApiClient _logsApiClient;
        private readonly IWebHostEnvironment _webHostEnvironment;
        #endregion

        #region "Constructores"
        public TransaccionesController(ITransaccionesApiClient transaccionesApiClient,
            UserContextService userContextService,
            ICatalogosApiClient catalogosApiClient,
            ILogsApiClient logsApiClient,
            IReintentadorService reintentadorService,
            IWebHostEnvironment webHostEnvironment
            )
        {
            _userContextService = userContextService;
            _transaccionesApiClient = transaccionesApiClient;
            _catalogosApiClient = catalogosApiClient;
            _logsApiClient = logsApiClient;
            _webHostEnvironment = webHostEnvironment;
            _reintentadorService = reintentadorService;
        }
        #endregion

        #region "Funciones"

        #region Consultar Transacciones
        [Authorize]
        public IActionResult Index(string AccountID = "")
        {
            var loginResponse = _userContextService.GetLoginResponse();
            var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Transacciones" && modulo.Ver == 0);
            if (validacion == true)
            {
                CargarDocumentoTransaccionesViewModel CargarInfo = new CargarDocumentoTransaccionesViewModel();
                ViewBag.Carga = CargarInfo;
                ViewBag.AccountID = AccountID;
                return View(loginResponse);
            }


            return NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<JsonResult> Consulta(List<RequestListFilters> filters, string idAccount = "")
        {
            var request = new RequestList();
            var loginResponse = _userContextService.GetLoginResponse();

            int totalRecord = 0;
            int filterRecord = 0;
            int paginacion = 0;
            int columna = 0;
            bool tipoFiltro = false;

            var draw = Request.Form["draw"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            request.Pagina = skip / pageSize + 1;
            request.Registros = pageSize;
            request.Busqueda = Request.Form["search[value]"].FirstOrDefault();
            request.ColumnaOrdenamiento = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            request.Ordenamiento = Request.Form["order[0][dir]"].FirstOrDefault();

            if (request.ColumnaOrdenamiento != null)
            {
                if (request.ColumnaOrdenamiento == "cuetaOrigenOrdenante")
                {
                    columna = 1;
                }
                else if (request.ColumnaOrdenamiento == "vinculo")
                {
                    columna = 2;
                }
                else if (request.ColumnaOrdenamiento == "NombreCuenta")
                {
                    columna = 3;
                }
                else if (request.ColumnaOrdenamiento == "Institucion")
                {
                    columna = 4;
                }
                else if (request.ColumnaOrdenamiento == "Concepto")
                {
                    columna = 5;
                }
                else if (request.ColumnaOrdenamiento == "Monto")
                {
                    columna = 6;
                }
                else if (request.ColumnaOrdenamiento == "FechaInstruccion")
                {
                    columna = 7;
                }
                else if (request.ColumnaOrdenamiento == "FechaAutorizacion")
                {
                    columna = 8;
                }
                else if (request.ColumnaOrdenamiento == "Estatus")
                {
                    columna = 9;
                }
            }

            if (request.Ordenamiento != null)
            {
                if (request.Ordenamiento == "asc")
                {
                    tipoFiltro = true;
                }
                else
                {
                    tipoFiltro = false;
                }
            }

            var gridData = new ResponseGrid<ResumenTransaccionResponseGrid>();
            List<TransaccionesResponse> ListPF = new List<TransaccionesResponse>();

            if (!string.IsNullOrEmpty(request.Busqueda))
            {

                filters.RemoveAll(x => x.Key == "claveRastreo");

                filters.Add(new RequestListFilters
                {
                    Key = "claveRastreo",
                    Value = request.Busqueda
                });
            }

            //Validar si hay algun filtro con valor ingresado
            var validaFiltro = filters.Where(x => x.Value != null).ToList();

            if (validaFiltro.Count != 0)
            {
                (ListPF, paginacion) = await _transaccionesApiClient.GetTransaccionesFilterAsync(Convert.ToInt32(request.Pagina), Convert.ToInt32(request.Registros), columna, tipoFiltro, filters);

            }
            else
            {
                (ListPF, paginacion) = await _transaccionesApiClient.GetTransaccionesAsync(Convert.ToInt32(request.Pagina), Convert.ToInt32(request.Registros), columna, tipoFiltro);

            }

            var List = new List<ResumenTransaccionResponseGrid>();
            foreach (var row in ListPF)
            {
                var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Transacciones" && modulo.Editar == 0);

                if (validacion == true)
                {
                    row.permiso = true;
                }
                else
                {
                    row.permiso = false;
                }

                row.claveRastreo = row.claveRastreo == null ? "N/A" : row.claveRastreo;
                List.Add(new ResumenTransaccionResponseGrid
                {
                    id = row.idTransaccion,
                    idCliente = row.idCliente,
                    claveRastreo = row.claveRastreo == null ? "N/A" : row.claveRastreo,
                    vinculo = row.claveRastreo + "|" + row.idCliente.ToString(),
                    nombreOrdenante = row.nombreOrdenante,
                    nombreBeneficiario = row.nombreBeneficiario,
                    monto = row.monto,
                    estatus = row.estatus,
                    concepto = row.concepto,
                    idMedioPago = row.idMedioPago,
                    idCuentaAhorro = row.idCuentaAhorro,
                    //fechaAlta = row.fechaAlta,
                    fechaInstruccion = row.fechaInstruccion,
                    fechaAutorizacion = row.fechaAutorizacion,
                    alias = row.alias,
                    clabeCobranza = row.clabeCobranza,
                    cuetaOrigenOrdenante = row.cuetaOrigenOrdenante,
                    descripcionMedioPago = row.descripcionMedioPago,
                    Acciones = await this.RenderViewToStringAsync("~/Views/Transacciones/_Acciones.cshtml", row)
                });
            }
            //if (!string.IsNullOrEmpty(request.Busqueda))
            //{
            //    List = List.Where(x =>
            //    (x.id.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            //    (x.claveRastreo?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            //    (x.nombreOrdenante?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            //    (x.nombreBeneficiario?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            //    (x.monto.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            //    (x.estatus.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            //    (x.concepto?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            //    (x.idMedioPago.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            //    (x.idCuentaAhorro.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            //    (x.fechaAutorizacion.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            //    (x.fechaInstruccion.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower())
            //    ).ToList();
            //}

            gridData.Data = List;
            gridData.RecordsTotal = paginacion;
            gridData.RecordsFiltered = paginacion;
            gridData.Draw = draw;

            return Json(gridData);
        }

        [Authorize]
        public async Task<IActionResult> DetalleTransaccion(string Id, string Estatus, string ClabeCobranza)
        {
            var result = new JsonResultDto();

            TransaccionDetailsResponse detalleTransaccion = new TransaccionDetailsResponse();

            try
            {
                detalleTransaccion = await _transaccionesApiClient.GetTransaccionDetalleAsync(Convert.ToInt32(Id));

                if (detalleTransaccion != null)
                {
                    result.Error = false;
                    result.Result = await this.RenderViewToStringAsync("~/Views/Transacciones/_Detalle.cshtml", detalleTransaccion.value);

                    var loginResponse = _userContextService.GetLoginResponse();
                    var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Transacciones" && modulo.Editar == 0);
                    if (validacion == true)
                    {
                        result.permiso = true;
                    }
                }
                else
                {
                    result.Error = true;
                    result.ErrorDescription = "ERROR";
                    return Json(result);
                }
            }
            catch (Exception)
            {
                result.Error = true;
                result.ErrorDescription = "Error1";
            }
            return Json(result);
        }
        #endregion

        #region Registrar Transacciones
        [Authorize]
        public async Task<ActionResult> Registro()
        {
            var loginResponse = _userContextService.GetLoginResponse();
            var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Transacciones" && modulo.Insertar == 0);
            if (validacion == true)
            {
                var listaMediosPago = await _catalogosApiClient.GetMediosPagoAsync();

                TransaccionesRegistroViewModel transaccionesRegistroViewModel = new TransaccionesRegistroViewModel
                {
                    TransacccionDetalles = new RegistrarTransaccionRequest(),
                    ListaMediosPago = listaMediosPago
                };

                ViewBag.Accion = "RegistrarTransaccion";
                ViewBag.TituloForm = "Registrar Transacción";

                return View(transaccionesRegistroViewModel);
            }


            return NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RegistrarTransaccion(TransaccionesRegistroViewModel model)
        {
            var loginResponse = _userContextService.GetLoginResponse();
            var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Transacciones" && modulo.Insertar == 0);
            if (validacion == true)
            {
                try
                {
                    var response = await _transaccionesApiClient.GetRegistroTransaccion(model.TransacccionDetalles);

                    string detalle = response.Detalle;
                    int idRegistro = 0;
                    try
                    {
                        string pattern = @"\d+";
                        Match match = Regex.Match(detalle, pattern);
                        idRegistro = int.Parse(match.Value);
                    }
                    catch (FormatException)
                    {
                        idRegistro = 0;
                    }

                    LogRequest logRequest = new LogRequest
                    {
                        IdUser = loginResponse.IdUsuario.ToString(),
                        Modulo = "Transacciones",
                        Fecha = HoraHelper.GetHoraCiudadMexico(),
                        NombreEquipo = loginResponse.NombreDispositivo,
                        Accion = "Insertar",
                        Ip = loginResponse.DireccionIp,
                        Envio = JsonConvert.SerializeObject(model.TransacccionDetalles),
                        Respuesta = response.Error.ToString(),
                        Error = response.Error ? JsonConvert.SerializeObject(response.Detalle) : string.Empty,
                        IdRegistro = idRegistro
                    };
                    await _logsApiClient.InsertarLogAsync(logRequest);

                    if (response.Codigo == "200")
                    {
                        return Ok(new { success = true });
                    }
                    else
                    {
                        return Ok(new { success = false, response.Detalle });
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest();
                }
            }

            return NotFound();
        }
        #endregion

        #region Modificar Transacciones
        [Authorize]
        public async Task<ActionResult> Modificar(int id)
        {
            var loginResponse = _userContextService.GetLoginResponse();
            var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Transacciones" && modulo.Editar == 0);
            if (validacion == true)
            {
                try
                {
                    var transaccionResponse = await _transaccionesApiClient.GetTransaccionDetalleAsync(id);
                    ModificarTransaccionRequest transaccionDetalles = new ModificarTransaccionRequest()
                    {
                        ClaveRastreo = transaccionResponse.value.ClaveRastreo ?? "",
                        Concepto = transaccionResponse.value.Concepto ?? "",
                        NombreOrdenante = transaccionResponse.value.nombreOrdenante ?? "",
                        FechaOperacion = transaccionResponse.value.FechaAlta,
                        CuentaOrigenOrdenante = transaccionResponse.value.cuetaOrigenOrdenante ?? "",
                        NoCuentaOrdenante = transaccionResponse.value.cuetaOrigenOrdenante ?? "",
                        IdTrasaccion = id,
                        NombreBeneficiario = transaccionResponse.value.NombreBeneficiario ?? "",
                        CuentaDestinoBeneficiario = transaccionResponse.value.CuentaDestinoBeneficiario ?? "",
                        NoCuentaBeneficiario = transaccionResponse.value.CuentaDestinoBeneficiario ?? ""
                    };

                    var listaMediosPago = await _catalogosApiClient.GetMediosPagoAsync();

                    if (listaMediosPago == null)
                    {
                        return RedirectToAction("Error404", "Error");
                    }

                    TransaccionEditarViewModel transaccionesEditarViewModel = new TransaccionEditarViewModel
                    {
                        TransacccionDetalles = transaccionDetalles,
                        ListaMediosPago = listaMediosPago
                    };

                    ViewBag.Accion = "ModificarTransaccion";
                    ViewBag.TituloForm = "Modificar Transacción";
                    return View("~/Views/Transacciones/Editar.cshtml", transaccionesEditarViewModel);
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
        public async Task<IActionResult> ModificarTransaccion(TransaccionEditarViewModel model)
        {
            var loginResponse = _userContextService.GetLoginResponse();
            var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Transacciones" && modulo.Editar == 0);
            if (validacion == true)
            {
                try
                {
                    var response = await _transaccionesApiClient.GetModificarTransaccion(model.TransacccionDetalles);

                    LogRequest logRequest = new LogRequest
                    {
                        IdUser = loginResponse.IdUsuario.ToString(),
                        Modulo = "Transacciones",
                        Fecha = HoraHelper.GetHoraCiudadMexico(),
                        NombreEquipo = loginResponse.NombreDispositivo,
                        Accion = "Editar",
                        Ip = loginResponse.DireccionIp,
                        Envio = JsonConvert.SerializeObject(model.TransacccionDetalles),
                        Respuesta = response.Error.ToString(),
                        Error = response.Error ? JsonConvert.SerializeObject(response.Detalle) : string.Empty,
                        IdRegistro = model.TransacccionDetalles.IdTrasaccion
                    };
                    await _logsApiClient.InsertarLogAsync(logRequest);

                    if (response.Codigo == "200")
                    {
                        return Ok(new { success = true });
                    }
                    else
                    {
                        return Ok(new { success = false, response.Detalle });
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest();
                }
            }

            return NotFound();
        }
        #endregion

        #region Devoluciones

        [Authorize]
        public IActionResult Devoluciones()
        {
            var loginResponse = _userContextService.GetLoginResponse();
            var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Transacciones" && modulo.Ver == 0);
            if (validacion == true)
            {
                return View(loginResponse);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<JsonResult> ConsultaDevoluciones(List<RequestListFilters> filters)
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

            var gridData = new ResponseGrid<DevolucionesResponseGrid>();
            List<DevolucionesResponse> ListPF = new List<DevolucionesResponse>();

            ListPF = await _reintentadorService.GetTransaccionesDevolverReintentar();

            var List = new List<DevolucionesResponseGrid>();
            foreach (var row in ListPF)
            {
                List.Add(new DevolucionesResponseGrid
                {
                    Id = row.Id,
                    ClaveRastreo = row.ClaveRastreo,
                    CuentaOrigen = row.CuentaOrigen,
                    Monto = row.Monto,
                    Concepto = row.Concepto,
                    FechaAlta = row.FechaAlta,
                    CuentaDestino = row.CuentaDestino,
                    BancoTxt = row.BancoTxt,
                    Acciones = await this.RenderViewToStringAsync("~/Views/Transacciones/_AccionesDevoluciones.cshtml", row)
                });
            }
            if (!string.IsNullOrEmpty(request.Busqueda))
            {
                List = List.Where(x =>
                (x.Id.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
                (x.ClaveRastreo?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
                (x.CuentaOrigen?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
                (x.Monto.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
                (x.Concepto?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
                (x.FechaAlta?.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
                (x.CuentaDestino?.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
                (x.BancoTxt?.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower())
                ).ToList();
            }
            //Aplicacion de Filtros temporal, 
            //var filtroCuentaOrdenante = filters.FirstOrDefault(x => x.Key == "cuentaOrdenante");
            //var filtroNombreOrdenante = filters.FirstOrDefault(x => x.Key == "nombreOrdenante");
            //var filtroNombreBeneficiario = filters.FirstOrDefault(x => x.Key == "nombreBeneficiario");
            //var filtroConcepto = filters.FirstOrDefault(x => x.Key == "concepto");
            //var filtroMonto = filters.FirstOrDefault(x => x.Key == "monto");
            //var filtroFecha = filters.FirstOrDefault(x => x.Key == "fecha");
            //var filtroEstatus = filters.FirstOrDefault(x => x.Key == "estatus");

            //if (filtroCuentaOrdenante.Value != null)
            //{
            //    List = List.Where(x => x.cuetaOrigenOrdenante.Contains(Convert.ToString(filtroCuentaOrdenante.Value))).ToList();
            //}

            //if (filtronombreClaveRastreo.Value != null)
            //{
            //    List = List.Where(x => x.claveRastreo.Contains(Convert.ToString(filtronombreClaveRastreo.Value))).ToList();
            //}

            //if (filtroNombreOrdenante.Value != null)
            //{
            //    List = List.Where(x => x.nombreOrdenante == Convert.ToString(filtroNombreOrdenante.Value)).ToList();
            //}

            //if (filtroNombreBeneficiario.Value != null)
            //{
            //    List = List.Where(x => x.nombreBeneficiario == Convert.ToString(filtroNombreBeneficiario.Value)).ToList();
            //}

            //if (filtroConcepto.Value != null)
            //{
            //    List = List.Where(x => x.concepto == Convert.ToString(filtroConcepto.Value)).ToList();
            //}

            //if (filtroMonto.Value != null)
            //{
            //    List = List.Where(x => x.monto.ToString() == Convert.ToString(filtroMonto.Value)).ToList();
            //}

            //if (filtroFecha.Value != null)
            //{
            //    List = List.Where(x => x.fechaAlta.ToString() == Convert.ToString(filtroFecha.Value)).ToList();
            //}

            //if (filtroEstatus.Value != null)
            //{
            //    if (filtroEstatus.Value == "1")
            //    {

            //        var estatusList = new[] { "1", "2" };
            //        List = List.Where(x => estatusList.Contains(Convert.ToString(x.estatus))).ToList();

            //    }
            //    else
            //    {
            //        List = List.Where(x => x.estatus == Convert.ToInt32(filtroEstatus.Value)).ToList();
            //    }
            //}

            gridData.Data = List;
            gridData.RecordsTotal = List.Count;
            gridData.Data = gridData.Data.Skip(skip).Take(pageSize).ToList();
            filterRecord = string.IsNullOrEmpty(request.Busqueda) ? gridData.RecordsTotal ?? 0 : gridData.Data.Count;
            gridData.RecordsFiltered = filterRecord;
            gridData.Draw = draw;

            return Json(gridData);
        }

        [HttpPost]
        public async Task<JsonResult> ReenviarTransacciones(List<string> clavesRastreo)
        {
            MensajeResponse response = new MensajeResponse();

            try
            {
                response = await _reintentadorService.GetReenviarTransaccionesAsync(clavesRastreo);
            } catch (Exception ex) {
                response.Error = true;
            }

            return Json(response);
        }

        [HttpPost]
        public async Task<JsonResult> ReenviarTransaccion(string clavesRastreo)
        {
            MensajeResponse response = new MensajeResponse();

            try
            {
                response = await _reintentadorService.GetReenviarTransaccionAsync(clavesRastreo);
            }
            catch (Exception ex)
            {
                response.Error = true;
            }

            return Json(response);
        }

        [HttpPost]
        public async Task<JsonResult> DevolverTransacciones(List<string> clavesRastreo)
        {
            MensajeResponse response = new MensajeResponse();

            try
            {
                response = await _reintentadorService.GetDevolverTransaccionesAsync(clavesRastreo);
            }
            catch (Exception ex)
            {
                response.Error = true;
            }

            return Json(response);
        }

        [HttpPost]
        public async Task<JsonResult> DevolverTransaccion(string clavesRastreo)
        {
            MensajeResponse response = new MensajeResponse();

            try
            {
                response = await _reintentadorService.GetDevolverTransaccionAsync(clavesRastreo);
            }
            catch (Exception ex)
            {
                response.Error = true;
            }

            return Json(response);
        }

        [HttpPost]
        public async Task<JsonResult> AgregarTransaccionReintentador(string ClaveRastreo)
        {
            MensajeResponse response = new MensajeResponse();

            try
            {
                response = await _reintentadorService.GetAgregarTransaccionReintentadorAsync(ClaveRastreo);
            }
            catch (Exception ex)
            {
                response.Error = true;
            }

            return Json(response);
        }

        #endregion

        #region Carga Masiva

        [HttpPost]
        public async Task<JsonResult> CargarDocumentoMasivoTransacciones(CargarDocumentoTransaccionesViewModel model)
        {

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            MensajeResponse response = new MensajeResponse();
            List<CargaBachRequest> ListaBach = new List<CargaBachRequest>();
            var loginResponse = _userContextService.GetLoginResponse();

            try
            {
                var filenameTemp = Path.GetFileNameWithoutExtension(Path.GetTempFileName());
                var extension = Path.GetExtension(model.documento.FileName);
                var pathTemp = Path.GetTempPath();
                var FilenameSave = filenameTemp + extension;
                DataSet resultExcel = new DataSet();
                DataTable tablaDatos = new DataTable();
                var filePath = pathTemp + "" + FilenameSave;
                List<string> listaClavesRastreo = new List<string>();

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.documento.CopyToAsync(stream);
                    stream.Close();
                }

                using (var streamExcel = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    IExcelDataReader reader = ExcelDataReader.ExcelReaderFactory.CreateOpenXmlReader(streamExcel);
                    resultExcel = reader.AsDataSet(new ExcelDataSetConfiguration());
                    reader.Close();
                    tablaDatos = resultExcel.Tables[0];
                }

                //Validamos el encabezado, que no posea mas de ocho columnas
                var ValidaEncabezado = tablaDatos.Rows[0];
                if (ValidaEncabezado.ItemArray.Length != 8)
                {
                    response.Error = true;
                    response.message = "El Archivo Excel no cuenta con la estructura correcta para poder ser procesado o el archivo tiene un formato diferente al indicado.";
                    return Json(response);
                }

                if (tablaDatos.Rows.Count == 1)
                {
                    response.Error = true;
                    response.message = "El Archivo Excel no cuenta con registros válidos.";
                    return Json(response);
                }

                for (var rowCell = 0; rowCell <= tablaDatos.Rows.Count - 1; rowCell++)
                {
                    var rowData = tablaDatos.Rows[rowCell];

                    //Remover Encabezado
                    if (rowCell < 1)
                    {
                        continue;
                    }

                    try
                    {
                        CargaBachRequest RegistrarTransaccion = new CargaBachRequest();

                        RegistrarTransaccion.estatus = 2;

                        if (rowData.ItemArray[0].ToString().ToLower() == "in")
                        {
                            RegistrarTransaccion.TipoOperacion = 1;
                            RegistrarTransaccion.Comision = 0;
                        }
                        else if (rowData.ItemArray[0].ToString().ToLower() == "out")
                        {
                            RegistrarTransaccion.TipoOperacion = 2;
                            RegistrarTransaccion.Comision = 0;
                        }
                        else if (rowData.ItemArray[0].ToString().ToLower() == "comision")
                        {
                            RegistrarTransaccion.TipoOperacion = 3;
                            RegistrarTransaccion.Comision = 1;
                        }
                        else
                        {
                            RegistrarTransaccion.observaciones = "El Tipo de Operación no es válido.";
                            RegistrarTransaccion.estatus = 1;
                        }

                        bool validarCuentaBeneficiario = ValidaCadena(rowData.ItemArray[1].ToString(), 18);
                        if (validarCuentaBeneficiario == true)
                        {
                            RegistrarTransaccion.observaciones = "La cuenta Clabe no es válida.";
                            RegistrarTransaccion.estatus = 1;
                        }
                        else
                        {
                            RegistrarTransaccion.CuentaBeneficiario = rowData.ItemArray[1].ToString();
                        }

                        bool validarCuentaOrdenante = ValidaCadena(rowData.ItemArray[7].ToString(), 16);
                        if (validarCuentaOrdenante == true)
                        {
                            RegistrarTransaccion.observaciones = "La Cuenta Ordenante no es válida.";
                            RegistrarTransaccion.estatus = 1;
                        }
                        else
                        {
                            RegistrarTransaccion.Ordenante = rowData.ItemArray[7].ToString();
                        }

                        bool validarMonto = ValidarMontos(rowData.ItemArray[2].ToString());
                        if (validarMonto == true)
                        {
                            RegistrarTransaccion.observaciones = "El monto no es válida.";
                            RegistrarTransaccion.estatus = 1;
                        }
                        else
                        {
                            RegistrarTransaccion.Monto = Convert.ToDouble(rowData.ItemArray[2]);
                        }

                        bool validarconceptoPago = ValidarCaracteresEspeciales(rowData.ItemArray[3].ToString(), 30);
                        if (validarconceptoPago == true)
                        {
                            RegistrarTransaccion.observaciones = "El concepto de pago no es válido.";
                            RegistrarTransaccion.estatus = 1;
                        }
                        else
                        {
                            RegistrarTransaccion.ConceptoPago = rowData.ItemArray[3].ToString();
                        }

                        bool validarclaverastreo = ValidarCaracteresEspeciales(rowData.ItemArray[5].ToString(), 25);
                        if (validarclaverastreo == true)
                        {
                            RegistrarTransaccion.observaciones = "La clave de rastreo no es válido.";
                            RegistrarTransaccion.estatus = 1;
                        }
                        else
                        {
                            RegistrarTransaccion.ClaveRastreo = rowData.ItemArray[5].ToString();
                        }

                        bool validaFecha = ValidarFecha(rowData.ItemArray[4].ToString());
                        if (validaFecha == true)
                        {
                            RegistrarTransaccion.observaciones = "La fecha no es válida.";
                            RegistrarTransaccion.estatus = 1;
                        }
                        else
                        {
                            RegistrarTransaccion.FechaOperacion = Convert.ToDateTime(rowData.ItemArray[4]);
                        }

                        int result;
                        bool validaMedioPago = int.TryParse(rowData.ItemArray[6].ToString(), out result);
                        if (validaMedioPago == false)
                        {
                            RegistrarTransaccion.observaciones = "El medio de pago no es válida.";
                            RegistrarTransaccion.estatus = 1;
                        }
                        else
                        {
                            RegistrarTransaccion.MedioPago = Convert.ToInt32(rowData.ItemArray[6]);
                        }

                        var match = listaClavesRastreo.FirstOrDefault(x => x.Contains(rowData.ItemArray[5].ToString()));

                        if (match != null)
                        {
                            RegistrarTransaccion.observaciones = "La clave de Rastreo ya existe en el documento cargado.";
                            RegistrarTransaccion.estatus = 1;
                        }

                        //para este punto, las transacciones seran validas 
                        RegistrarTransaccion.IdUsuario = Convert.ToInt32(loginResponse.IdUsuario.ToString());

                        ListaBach.Add(RegistrarTransaccion);
                        listaClavesRastreo.Add(RegistrarTransaccion.ClaveRastreo);

                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }

                if (ListaBach.Count == 0)
                {
                    response.Error = true;
                    response.message = "No se pudo cargar las transacciones, debido a que los datos proporcionados no son válidos.";
                }
                else
                {
                    response = await _transaccionesApiClient.GetInsertaTransaccionesBatchAsync(ListaBach);
                }

            }
            catch (Exception ex)
            {
                response.Error = true;
                response.message = "Hubo un problema al cargar las transacciones, verifique que el documento sea válido.";
            }

            return Json(response);
        }

        public IActionResult GetBlobDownloadFormat()
        {
            var net = new System.Net.WebClient();

            string webRootPath = _webHostEnvironment.WebRootPath;
            string path = Path.Combine(webRootPath, "Formatos");

            string FolderPath = Path.Combine(path, "Formato_Carga_Masiva_Transacciones.xlsx"); // <-- Error here
            var data = net.DownloadData(FolderPath);
            var content = new System.IO.MemoryStream(data);
            var contentType = "APPLICATION/octet-stream";
            var fileName = "Formato_Carga_Masiva_Transacciones.xlsx";
            return File(content, contentType, fileName);
        }

        [Authorize]
        public async Task<IActionResult> ConsultarCargaMasiva()
        {
            var loginResponse = _userContextService.GetLoginResponse();
            var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Transacciones" && modulo.Ver == 0);

            if (validacion == true)
            {
                ViewBag.CatalogoMediosPago = await _catalogosApiClient.GetMediosPagoAsync();
                int valida = await _transaccionesApiClient.GetTotalTransaccionesBatchAsync(loginResponse.IdUsuario);

                if (valida == 0) {
                    ViewBag.ValidaCarga = "True";
                } else {
                    ViewBag.ValidaCarga = "False";
                }
                return View(loginResponse);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<JsonResult> ConsultaCargaMasiva(List<RequestListFilters> filters)
        {
            var request = new RequestList();
            var loginResponse = _userContextService.GetLoginResponse();

            int totalRecord = 0;
            int filterRecord = 0;
            int paginacion = 0;
            int columna = 0;
            bool tipoFiltro = false;

            var draw = Request.Form["draw"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            request.Pagina = skip / pageSize + 1;
            request.Registros = pageSize;
            request.Busqueda = Request.Form["search[value]"].FirstOrDefault();
            request.ColumnaOrdenamiento = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            request.Ordenamiento = Request.Form["order[0][dir]"].FirstOrDefault();

            if (request.ColumnaOrdenamiento != null)
            {
                if (request.ColumnaOrdenamiento == "claveRastreo") {
                    columna = 1;
                } else if (request.ColumnaOrdenamiento == "Ordenante") {
                    columna = 2;
                } else if (request.ColumnaOrdenamiento == "CuentaBeneficiario") {
                    columna = 3;
                } else if (request.ColumnaOrdenamiento == "Monto") {
                    columna = 4;
                } else if (request.ColumnaOrdenamiento == "ConceptoPago") {
                    columna = 5;
                } else if (request.ColumnaOrdenamiento == "FechaOperacion") {
                    columna = 6;
                } else if (request.ColumnaOrdenamiento == "TipoOperacion") {
                    columna = 7;
                } else if (request.ColumnaOrdenamiento == "MedioPago") {
                    columna = 8;
                } else if (request.ColumnaOrdenamiento == "observaciones") {
                    columna = 9;
                }
            }

            if (request.Ordenamiento != null)
            {
                if (request.Ordenamiento == "asc") {
                    tipoFiltro = true;
                } else {
                    tipoFiltro = false;
                }
            }

            var gridData = new ResponseGrid<CargaBachRequestGrid>();
            List<CargaBachRequest> ListPF = new List<CargaBachRequest>();

            if (!string.IsNullOrEmpty(request.Busqueda)) {

                filters.RemoveAll(x => x.Key == "claveRastreo");

                filters.Add(new RequestListFilters
                {
                    Key = "claveRastreo",
                    Value = request.Busqueda
                });
            }

            //Validar si hay algun filtro con valor ingresado
            var validaFiltro = filters.Where(x => x.Value != null).ToList();

            if (validaFiltro.Count != 0) {
                (ListPF, paginacion) = await _transaccionesApiClient.GetTransaccionesMasivaFilterAsync(Convert.ToInt32(request.Pagina), Convert.ToInt32(request.Registros), loginResponse.IdUsuario, columna, tipoFiltro, filters);
            } else {
                (ListPF, paginacion) = await _transaccionesApiClient.GetTransaccionesMasivaAsync(Convert.ToInt32(request.Pagina), Convert.ToInt32(request.Registros), loginResponse.IdUsuario, columna, tipoFiltro);
            }

            var List = new List<CargaBachRequestGrid>();
            foreach (var row in ListPF)
            {
                List.Add(new CargaBachRequestGrid
                {
                    Id = row.Id,
                    ClaveRastreo = row.ClaveRastreo,
                    FechaOperacion = row.FechaOperacion,
                    Monto = row.Monto,
                    CuentaBeneficiario = row.CuentaBeneficiario,
                    ConceptoPago = row.ConceptoPago,
                    descripcionOperacion = row.descripcionOperacion,
                    descripcionMedioPago = row.descripcionMedioPago,
                    Comision = row.Comision,
                    Ordenante = row.Ordenante,
                    observaciones = row.observaciones,
                    Acciones = await this.RenderViewToStringAsync("~/Views/Transacciones/_AccionesCargaMasiva.cshtml", row)
                });
            }
            //if (!string.IsNullOrEmpty(request.Busqueda))
            //{
            //    List = List.Where(x =>
            //    (x.idTransaccion.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            //    (x.ClaveRastreo?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            //    (x.CuentaOrigen?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            //    (x.Monto.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            //    (x.Concepto?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            //    (x.FechaAlta?.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            //    (x.CuentaDestino?.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            //    (x.BancoTxt?.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower())
            //    ).ToList();
            //}

            //var filtroCuentaOrdenante = filters.FirstOrDefault(x => x.Key == "cuentaOrdenante");
            //var filtroNombreOrdenante = filters.FirstOrDefault(x => x.Key == "nombreOrdenante");
            //var filtroNombreBeneficiario = filters.FirstOrDefault(x => x.Key == "nombreBeneficiario");
            //var filtroConcepto = filters.FirstOrDefault(x => x.Key == "concepto");
            //var filtroMonto = filters.FirstOrDefault(x => x.Key == "monto");
            //var filtroFecha = filters.FirstOrDefault(x => x.Key == "fecha");
            //var filtroEstatus = filters.FirstOrDefault(x => x.Key == "estatus");

            //if (filtroCuentaOrdenante.Value != null)
            //{
            //    List = List.Where(x => x.cuetaOrigenOrdenante.Contains(Convert.ToString(filtroCuentaOrdenante.Value))).ToList();
            //}

            //if (filtronombreClaveRastreo.Value != null)
            //{
            //    List = List.Where(x => x.claveRastreo.Contains(Convert.ToString(filtronombreClaveRastreo.Value))).ToList();
            //}

            //if (filtroNombreOrdenante.Value != null)
            //{
            //    List = List.Where(x => x.nombreOrdenante == Convert.ToString(filtroNombreOrdenante.Value)).ToList();
            //}

            //if (filtroNombreBeneficiario.Value != null)
            //{
            //    List = List.Where(x => x.nombreBeneficiario == Convert.ToString(filtroNombreBeneficiario.Value)).ToList();
            //}

            //if (filtroConcepto.Value != null)
            //{
            //    List = List.Where(x => x.concepto == Convert.ToString(filtroConcepto.Value)).ToList();
            //}

            //if (filtroMonto.Value != null)
            //{
            //    List = List.Where(x => x.monto.ToString() == Convert.ToString(filtroMonto.Value)).ToList();
            //}

            //if (filtroFecha.Value != null)
            //{
            //    List = List.Where(x => x.fechaAlta.ToString() == Convert.ToString(filtroFecha.Value)).ToList();
            //}

            //if (filtroEstatus.Value != null)
            //{
            //    if (filtroEstatus.Value == "1")
            //    {

            //        var estatusList = new[] { "1", "2" };
            //        List = List.Where(x => estatusList.Contains(Convert.ToString(x.estatus))).ToList();

            //    }
            //    else
            //    {
            //        List = List.Where(x => x.estatus == Convert.ToInt32(filtroEstatus.Value)).ToList();
            //    }
            //}

            gridData.Data = List;
            gridData.RecordsTotal = paginacion;
            filterRecord = string.IsNullOrEmpty(request.Busqueda) ? gridData.RecordsTotal ?? 0 : gridData.Data.Count;
            gridData.RecordsFiltered = filterRecord;
            gridData.Draw = draw;

            return Json(gridData);
        }

        [HttpPost]
        public async Task<JsonResult> EliminarTransaccionBatch(int idRegistrio)
        {
            var loginResponse = _userContextService.GetLoginResponse();
            MensajeResponse response = new MensajeResponse();

            try
            {
                response = await _transaccionesApiClient.GetEliminarTransaccionBatchAsync(idRegistrio);
                var validaButton = await _transaccionesApiClient.GetTotalTransaccionesBatchAsync(Convert.ToInt32(loginResponse.IdUsuario.ToString()));

                response.Detalle = validaButton.ToString();
            }
            catch (Exception ex)
            {
                response.Detalle = ex.Message;
            }

            return Json(response);
        }

        public async Task<IActionResult> EditarTransaccionBatch(int idRegistro)
        {
            var result = new JsonResultDto();

            CargaBachRequest detalleTransaccion = new CargaBachRequest();

            try
            {
                var listaMediosPago = await _catalogosApiClient.GetMediosPagoAsync();
                detalleTransaccion = await _transaccionesApiClient.GetTransaccionBatchAsync(idRegistro);

                //detalleTransaccion.FechaFormat = detalleTransaccion.FechaOperacion.ToString("yyyy-MM-dd");

                detalleTransaccion.ListaMediosPago = listaMediosPago;
                if (detalleTransaccion != null)
                {
                    result.Error = false;
                    result.Result = await this.RenderViewToStringAsync("~/Views/Transacciones/_EditarTransaccionMasiva.cshtml", detalleTransaccion);
                }
                else
                {
                    result.Error = true;
                    result.ErrorDescription = "ERROR";
                    return Json(result);
                }
            }
            catch (Exception)
            {
                result.Error = true;
                result.ErrorDescription = "Error1";
            }
            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> EditarTransaccionBatch(CargaBachRequest model)
        {
            var loginResponse = _userContextService.GetLoginResponse();
            string response = "";
            MensajeResponse respuesta = new MensajeResponse();
            try
            {
                model.estatus = 2;

                bool validarCuentaBeneficiario = ValidaCadena(model.CuentaBeneficiario, 18);
                if (validarCuentaBeneficiario == true)
                {
                    model.observaciones = "La cuenta Clabe no es válida.";
                    model.estatus = 1;
                }

                bool validarCuentaOrdenante = ValidaCadena(model.Ordenante, 16);
                if (validarCuentaOrdenante == true)
                {
                    model.observaciones = "La Cuenta Ordenante no es válida.";
                    model.estatus = 1;
                }

                bool validarMonto = ValidarMontos(model.Monto.ToString());
                if (validarMonto == true)
                {
                    model.observaciones = "El monto no es válida.";
                    model.estatus = 1;
                }

                bool validarconceptoPago = ValidarCaracteresEspeciales(model.ConceptoPago, 30);
                if (validarconceptoPago == true)
                {
                    model.observaciones = "El concepto de pago no es válido.";
                    model.estatus = 1;
                }

                bool validarclaverastreo = ValidarCaracteresEspeciales(model.ClaveRastreo, 25);
                if (validarclaverastreo == true)
                {
                    model.observaciones = "La clave de rastreo no es válido.";
                    model.estatus = 1;
                }

                bool validaFecha = ValidarFecha(model.FechaOperacion.ToString());
                if (validaFecha == true)
                {
                    model.observaciones = "La fecha no es válida.";
                    model.estatus = 1;
                }

                int validaExistencia = await _transaccionesApiClient.GetValidaExistenciaTranBatch(loginResponse.IdUsuario, model.ClaveRastreo);
                if (validaExistencia > 1) {
                    model.observaciones = "La clave de Rastreo ya existe en el documento cargado.";
                    model.estatus = 1;
                }

                response = await _transaccionesApiClient.GetModificaTransaccionBatchAsync(model);

                if (response == "EXITO")
                {
                    respuesta.Error = false;
                } else
                {
                    respuesta.Error = true;
                }
            }
            catch (Exception ex)
            {
                respuesta.Error = true;
            }

            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ProcesarTransaccionesMasiva(int idUsuario)
        {
            var loginResponse = _userContextService.GetLoginResponse();
            string response = "";
            int paginacion = 0;
            MensajeResponse respuesta = new MensajeResponse();
            try
            {
                //List<CargaBachRequest> ListPF = new List<CargaBachRequest>();

                //(ListPF, paginacion) = await _transaccionesApiClient.GetTransaccionesMasivaAsync(loginResponse.IdUsuario);

                //var List = new List<InsertarTransaccionRequest>();
                //foreach (var row in ListPF)
                //{
                //    List.Add(new InsertarTransaccionRequest
                //    {
                //        Id = row.Id,
                //        ClaveRastreo = row.ClaveRastreo,
                //        FechaOperacion = row.FechaOperacion.ToString("yyyy-MM-dd HH:mm:ss"),
                //        NoCuentaBeneficiario = row.CuentaBeneficiario,
                //        Monto = row.Monto,
                //        Concepto = row.ConceptoPago,
                //        TipoOperacion = row.TipoOperacion,
                //        MedioPago = row.MedioPago,
                //        Comision = row.Comision,
                //        CuentaOrdenante = row.Ordenante,
                //        IdUsuario = loginResponse.IdUsuario
                //    });
                //}

                var validaButton = await _transaccionesApiClient.GetTotalTransaccionesBatchAsync(idUsuario);

                if (validaButton == 0) {
                    respuesta.Error = true;
                    respuesta.message = "No existen transacciones pendientes para procesar";
                } else {
                    respuesta = await _transaccionesApiClient.GetProcesaTransaccion(loginResponse.IdUsuario);

                    var transaccionesPendientes = await _transaccionesApiClient.GetTotalTransaccionesBatchAsync(idUsuario);
                    respuesta.Detalle = transaccionesPendientes.ToString();
                }      
            }
            catch (Exception ex)
            {
                respuesta.Error = true;
            }

            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> EliminarTransaccionesBatch(int idUsuario)
        {
            //var loginResponse = _userContextService.GetLoginResponse();
            MensajeResponse response = new MensajeResponse();

            try
            {
                response = await _transaccionesApiClient.GetEliminarTransaccionesBatchAsync(idUsuario);
                var validaButton = await _transaccionesApiClient.GetTotalTransaccionesBatchAsync(idUsuario);

                response.Detalle = validaButton.ToString();
            }
            catch (Exception ex)
            {
                response.Detalle = ex.Message;
            }

            return Json(response);
        }
        #endregion

        #region Auxiliar
        public async Task<IActionResult> DescargarVoucherTransaccion(int CuentaAhorro, int Transaccion)
        {
            VoucherResponse? respuesta = new VoucherResponse();

            try
            {
                respuesta = await _transaccionesApiClient.GetVoucherTransaccionAsync(CuentaAhorro, Transaccion, _userContextService.GetLoginResponse().Token);

                if (respuesta == null)
                {
                    return RedirectToAction("index", "home");
                }

                byte[] bytes = Convert.FromBase64String(respuesta.archivo);
                MemoryStream Archivo = new MemoryStream(bytes);

                var contentType = "APPLICATION/octet-stream";
                var fileName = string.Format("comprobante_{0}_{1}.pdf", CuentaAhorro.ToString(), Transaccion.ToString());

                return File(Archivo, contentType, fileName);

            }
            catch (Exception ex)
            {
                return RedirectToAction("index", "home");
            }

        }
        #endregion

        #endregion

        #region "Modelos"

        public class ResumenTransaccionResponse
        {
            public int id { get; set; }
            public int idCliente { get; set; }
            public string claveRastreo { get; set; }
            public string vinculo { get; set; }
            public string nombreOrdenante { get; set; }
            public string cuetaOrigenOrdenante { get; set; }
            public string clabeCobranza { get; set; }
            public string nombreBeneficiario { get; set; }
            public decimal monto { get; set; }
            public int estatus { get; set; }
            public string concepto { get; set; }
            public int idMedioPago { get; set; }
            public int idCuentaAhorro { get; set; }
            public string fechaAlta { get; set; }
            public string fechaActualizacion { get; set; }
            public string fechaInstruccion { get; set; }
            public string fechaAutorizacion { get; set; }
            public string alias { get; set; }
            public string descripcionMedioPago { get; set; }

        }
        public class ResumenTransaccionResponseGrid : ResumenTransaccionResponse
        {
            public string Acciones { get; set; }
        }
        #endregion

        #region "Funciones Auxiliares"

        private bool ValidarCaracteresEspeciales(string cadena, int tamaño = 0) {

            bool error = false;

            try
            {

                var regexItem = new Regex("[^a-zA-Z0-9\\s]+");
                bool validSpecialCharacter = regexItem.IsMatch(cadena);

                if (validSpecialCharacter == true) {
                    error = true;
                } 

                if (cadena.Length > tamaño) {
                    error = true;
                }

            } catch (Exception ex) {
                error = true;
            }

            return error;
        }

        private bool ValidaCadena(string cadena, int tamaño = 0) {

            bool error = false;

            try
            {
                //Validar que la cadena tenga solo numeros
                bool isNumeric = Regex.IsMatch(cadena, @"^\d+$");
                if (isNumeric != true) {
                    error = true;
                }

                //Validar que la cadena tenga el tamaño maximo
                if (cadena.Length > tamaño) {
                    error = true;
                } else if (cadena.Length < tamaño) {
                    error = true;
                }


            } catch (Exception ex)  {
                return true;
            }

            return error;
        }

        private bool ValidarMontos(string cadena)
        {

            bool error = false;

            try
            {
                //Validar que la cadena tenga solo numeros
                bool isNumeric = Regex.IsMatch(cadena, "^\\d+\\.?\\d*$");
                if (isNumeric != true)
                {
                    error = true;
                }

                //Validar que la cadena tenga un monto valido
                var montoGeneral = Convert.ToDouble(cadena.Replace(",",""));

                if (montoGeneral == 0) {
                    error = true;
                } else if (montoGeneral <= -1) {
                    error = true;
                }


            }
            catch (Exception ex)
            {
                return true;
            }

            return error;
        }

        private bool ValidarFecha(string cadena)
        {

            bool error = false;

            try
            {
                //Validar que la cadena tenga una secuencia de fecha valido
                DateTime temp;
                if (!DateTime.TryParse(cadena, out temp)){
                    error = true;
                }

            }
            catch (Exception ex)
            {
                return true;
            }

            return error;
        }
        #endregion
    }
}