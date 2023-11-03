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
    public class AutorizadorController : Controller
    {
        #region "Locales"
        private readonly UserContextService _userContextService;
        private readonly IReintentadorService _reintentadorService;
        private readonly IAbonoServices _abonoService;//Transacciones
        private readonly ICatalogosApiClient _catalogosApiClient;
        private readonly ILogsApiClient _logsApiClient;
        private readonly IWebHostEnvironment _webHostEnvironment;
        #endregion

        #region "Constructores"
        public AutorizadorController(IAbonoServices abonoServices,
            UserContextService userContextService,
            ICatalogosApiClient catalogosApiClient,
            ILogsApiClient logsApiClient,
            IReintentadorService reintentadorService,
            IWebHostEnvironment webHostEnvironment
            )
        {
            _userContextService = userContextService;
            _abonoService = abonoServices;
            _catalogosApiClient = catalogosApiClient;
            _logsApiClient = logsApiClient;
            _webHostEnvironment = webHostEnvironment;
            _reintentadorService = reintentadorService;
        }
        #endregion

        #region "Funciones"

        #region Consultar Abonos
        [Authorize]
        public IActionResult Index()
        {
            var loginResponse = _userContextService.GetLoginResponse();
            var validacion = loginResponse?.AccionesPorModulo.Any(modulo => modulo.ModuloAcceso == "Transacciones" && modulo.Ver == 0);
            if (validacion == true)
            {
                return View(loginResponse);
            }


            return NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<JsonResult> Consulta(List<RequestListFilters> filters)
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
                if (request.ColumnaOrdenamiento == "ClaveRastreo")
                {
                    columna = 1;
                }
                else if (request.ColumnaOrdenamiento == "Monto")
                {
                    columna = 2;
                }
                else if (request.ColumnaOrdenamiento == "CuentaOrdenante")
                {
                    columna = 3;
                }
                else if (request.ColumnaOrdenamiento == "NombreOrdenante")
                {
                    columna = 4;
                }
                else if (request.ColumnaOrdenamiento == "DescripcionEstatusAutorizacion")
                {
                    columna = 5;
                }
                else if (request.ColumnaOrdenamiento == "DescripcionEstatusTransaccion")
                {
                    columna = 6;
                }
                else if (request.ColumnaOrdenamiento == "Concepto")
                {
                    columna = 7;
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

            var gridData = new ResponseGrid<ResumenTransaccionSPEIINResponseGrid>();
            List<TransaccionSPEIINResponse> ListPF = new List<TransaccionSPEIINResponse>();

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

            if (validaFiltro.Count != 0) {
                (ListPF, paginacion) = await _abonoService.GetAbonosFilterAsync(Convert.ToInt32(request.Pagina), Convert.ToInt32(request.Registros), columna, tipoFiltro, filters);
            } else {
                (ListPF, paginacion) = await _abonoService.GetAbonosAsync(Convert.ToInt32(request.Pagina), Convert.ToInt32(request.Registros), columna, tipoFiltro);
            }

            var List = new List<ResumenTransaccionSPEIINResponseGrid>();
            foreach (var row in ListPF)
            {
                List.Add(new ResumenTransaccionSPEIINResponseGrid
                {
                    Id = row.Id,
                    ClaveRastreo = !string.IsNullOrWhiteSpace(row.ClaveRastreo) ? row.ClaveRastreo : "-",
                    Monto = row.Monto,
                    CuentaOrdenante = !string.IsNullOrWhiteSpace(row.CuentaOrdenante) ? row.CuentaOrdenante : "-",
                    NombreOrdenante = !string.IsNullOrWhiteSpace(row.NombreOrdenante) ? row.NombreOrdenante : "-",
                    Concepto = !string.IsNullOrWhiteSpace(row.Concepto) ? row.Concepto : "-",
                    DescripcionEstatusAutorizacion = !string.IsNullOrWhiteSpace(row.DescripcionEstatusAutorizacion) ? row.DescripcionEstatusAutorizacion : "-",
                    descripcioEstatusTransaccion = !string.IsNullOrWhiteSpace(row.descripcioEstatusTransaccion) ? row.descripcioEstatusTransaccion : "-",
                    Acciones = await this.RenderViewToStringAsync("~/Views/Autorizador/_Acciones.cshtml", row)
                });
            }

            gridData.Data = List;
            gridData.RecordsTotal = paginacion;
            gridData.RecordsFiltered = paginacion;
            gridData.Draw = draw;

            return Json(gridData);
        }

        [Authorize]
        [HttpPatch]
        public async Task<IActionResult> AcreditarAbono(string claveRastreo)
        {
            bool rechazar = false;
            var response = await _abonoService.PatchAutorizadorSpeiInAsync(claveRastreo, rechazar);

            if (response.Error == false)
            {
                return Ok(new { success = true, response.message });
            }
            else
            {
                return Ok(new { success = false, response.message });
            }
        }
        [Authorize]
        [HttpPatch]
        public async Task<IActionResult> RechazarAbono(string claveRastreo)
        {
            bool rechazar = true;
            var response = await _abonoService.PatchAutorizadorSpeiInAsync(claveRastreo, rechazar);

            if (response.Error == false)
            {
                return Ok(new { success = true, response.message });
            }
            else
            {
                return Ok(new { success = false, response.message });
            }
        }
        #endregion

        #endregion

        #region "Modelos"

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