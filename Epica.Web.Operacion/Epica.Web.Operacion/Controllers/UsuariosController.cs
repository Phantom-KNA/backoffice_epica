using Epica.Web.Operacion.Config;
using Epica.Web.Operacion.Models.Common;
using Epica.Web.Operacion.Models.ViewModels;
using Epica.Web.Operacion.Models.Entities;
using Epica.Web.Operacion.Services.Transaccion;
using Epica.Web.Operacion.Services.UserResolver;
using Epica.Web.Operacion.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using static Epica.Web.Operacion.Controllers.CuentaController;
using Epica.Web.Operacion.Models.Request;

namespace Epica.Web.Operacion.Controllers;

public class UsuariosController : Controller
{
    #region "Locales"
    private readonly IUsuariosApiClient _usuariosApiClient;
    #endregion

    #region "Constructores"
    public UsuariosController(IUsuariosApiClient usuariosApiClient)
    {
        _usuariosApiClient = usuariosApiClient;
    }
    #endregion

    #region "Funciones"
    public IActionResult Index()
    {
        return View();
    }
    public IActionResult Registro()
    {
        ViewBag.TituloForm = "Crear nuevo usuario";
        return View("~/Views/Usuarios/Registro.cshtml");
    }
    public IActionResult GestionarPermisos()
    {
        return View();
    }

    [HttpPost]
    public async Task<JsonResult> ConsultaPermisos(List<RequestListFilters> filters)
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

        var gridData = new ResponseGrid<UserPermissionResponseGrid>();
        List<UserPermissionResponse> ListPF = new List<UserPermissionResponse>();

        //ListPF = await _usuariosApiClient.GetUsuariosAsync(1, 200);

        //Entorno local de pruebas
        ListPF = await GetListUser();

        var List = new List<UserPermissionResponseGrid>();
        foreach (var row in ListPF)
        {
            List.Add(new UserPermissionResponseGrid
            {
                id = row.id,
                nombreCompleto = row.nombreCompleto,
                listaGen = row.listaGen
            });
        }
        //if (!string.IsNullOrEmpty(request.Busqueda))
        //{
        //    List = List.Where(x =>
        //    (x.id.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
        //    (x.nombreCompleto?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
        //    (x.telefono?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
        //    (x.email?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
        //    (x.CURP?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
        //    (x.organizacion?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
        //    (x.membresia?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
        //    (x.sexo?.ToLower() ?? "").Contains(request.Busqueda.ToLower())
        //    ).ToList();
        //}

        //Aplicacion de Filtros temporal, 
        //var filtroid = filters.FirstOrDefault(x => x.Key == "Id");
        //var filtronombreCliente = filters.FirstOrDefault(x => x.Key == "NombreCliente");
        //var filtroNoCuenta = filters.FirstOrDefault(x => x.Key == "noCuenta");
        //var filtroEstatus = filters.FirstOrDefault(x => x.Key == "estatus");
        //var filtroSaldo = filters.FirstOrDefault(x => x.Key == "saldo");
        //var filtroTipo = filters.FirstOrDefault(x => x.Key == "tipo");

        //if (filtroid.Value != null)
        //{
        //    List = List.Where(x => x.Id == Convert.ToInt32(filtroid.Value)).ToList();
        //}

        //if (filtronombreCliente.Value != null)
        //{
        //    List = List.Where(x => x.cliente.Contains(Convert.ToString(filtronombreCliente.Value))).ToList();
        //}

        //if (filtroNoCuenta.Value != null)
        //{
        //    List = List.Where(x => x.noCuenta == Convert.ToString(filtroNoCuenta.Value)).ToList();
        //}

        //if (filtroEstatus.Value != null)
        //{
        //    List = List.Where(x => x.estatus == Convert.ToString(filtroEstatus.Value)).ToList();
        //}

        //if (filtroSaldo.Value != null)
        //{
        //    List = List.Where(x => x.saldo == Convert.ToString(filtroSaldo.Value)).ToList();
        //}

        //if (filtroTipo.Value != null)
        //{
        //    List = List.Where(x => x.tipo == Convert.ToString(filtroTipo.Value)).ToList();
        //}

        gridData.Data = List;
        gridData.RecordsTotal = List.Count;
        gridData.Data = gridData.Data.Skip(skip).Take(pageSize).ToList();
        filterRecord = string.IsNullOrEmpty(request.Busqueda) ? gridData.RecordsTotal ?? 0 : gridData.Data.Count;
        gridData.RecordsFiltered = filterRecord;
        gridData.Draw = draw;

        return Json(gridData);
    }


    public async Task<IActionResult> GestionarDocumentos(string AccountID = "")
    {
        if (AccountID == "")
        {
            return RedirectToAction("Index");
        }

        UserResponse GetDatosUsuario = await _usuariosApiClient.GetUsuarioAsync(Convert.ToInt32(AccountID));

        ViewBag.AccountID = AccountID;

        if (GetDatosUsuario.nombreCompleto == null) {
            ViewBag.Nombre = "S/N";
        } else {
            ViewBag.Nombre = GetDatosUsuario.nombreCompleto;
        }

        return View();
    }

    public async Task<ActionResult> Detalles(int id)
    {
        UserResponse user = await _usuariosApiClient.GetUsuarioAsync(id);
        UsuariosDetallesViewModel usuariosDetallesViewModel = new UsuariosDetallesViewModel
        {
            Id = user.id,
            Nombre = user.nombreCompleto,
            Telefono = user.telefono,
            Email = user.email,
            Curp = user.CURP,
            Empresa = user.organizacion,
            Sexo = user.sexo,
        };

        return View("~/Views/Usuarios/Detalles.cshtml", usuariosDetallesViewModel);
    }

    public async Task<ActionResult> Modificar(int id)
    {
        ViewBag.TituloForm = "Modificar usuario";
        try
        {
            UserResponse user = await _usuariosApiClient.GetUsuarioAsync(id);
            UsuariosDetallesViewModel usuariosDetallesViewModel = new UsuariosDetallesViewModel
            {
                Id = user.id,
                Nombre = user.nombreCompleto,
                Telefono = user.telefono,
                Email = user.email,
                Curp = user.CURP,
                Empresa = user.organizacion,
                Sexo = user.sexo,
            };

            if (usuariosDetallesViewModel == null)
            {
                return RedirectToAction("Error404", "Error"); 
            }

            return View("~/Views/Usuarios/Registro.cshtml", usuariosDetallesViewModel);
        }
        catch (Exception)
        {
            return RedirectToAction("Error", "Error");
        }
    }

    [Route("Usuarios/Detalle/Cuentas")]
    public IActionResult Cuentas(long id)
    {
        ViewBag.UrlView = "Cuentas";
        var Info = GetList().FirstOrDefault(x => x.id == id);
        ViewBag.Info = Info;
        ViewBag.Nombre = Info.nombreCompleto;
        return View("~/Views/Usuarios/Detalle/Cuenta/DetalleCuentas.cshtml");
    }

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

        var gridData = new ResponseGrid<UserResponseGrid>();
        List<UserResponse> ListPF = new List<UserResponse>();

        ListPF = await _usuariosApiClient.GetUsuariosAsync(1,200);

        //Entorno local de pruebas
        //ListPF = GetList();

        var List = new List<UserResponseGrid>();
        foreach (var row in ListPF)
        {
            List.Add(new UserResponseGrid
            {
                id = row.id,
                nombreCompleto = row.nombreCompleto,
                telefono = row.telefono,
                email = row.email,
                CURP = row.CURP,
                organizacion = row.organizacion,
                membresia = row.membresia,
                sexo = row.sexo,
                estatus = row.estatus,
                Acciones = await this.RenderViewToStringAsync("~/Views/Usuarios/_Acciones.cshtml", row)
            });
        }
        if (!string.IsNullOrEmpty(request.Busqueda))
        {
            List = List.Where( x =>
            (x.id.ToString().ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.nombreCompleto?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.telefono?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.email?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.CURP?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) || 
            (x.organizacion?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.membresia?.ToLower() ?? "").Contains(request.Busqueda.ToLower()) ||
            (x.sexo?.ToLower() ?? "").Contains(request.Busqueda.ToLower())
            ).ToList();
        }

        //Aplicacion de Filtros temporal, 
        //var filtroid = filters.FirstOrDefault(x => x.Key == "Id");
        //var filtronombreCliente = filters.FirstOrDefault(x => x.Key == "NombreCliente");
        //var filtroNoCuenta = filters.FirstOrDefault(x => x.Key == "noCuenta");
        //var filtroEstatus = filters.FirstOrDefault(x => x.Key == "estatus");
        //var filtroSaldo = filters.FirstOrDefault(x => x.Key == "saldo");
        //var filtroTipo = filters.FirstOrDefault(x => x.Key == "tipo");

        //if (filtroid.Value != null)
        //{
        //    List = List.Where(x => x.Id == Convert.ToInt32(filtroid.Value)).ToList();
        //}

        //if (filtronombreCliente.Value != null)
        //{
        //    List = List.Where(x => x.cliente.Contains(Convert.ToString(filtronombreCliente.Value))).ToList();
        //}

        //if (filtroNoCuenta.Value != null)
        //{
        //    List = List.Where(x => x.noCuenta == Convert.ToString(filtroNoCuenta.Value)).ToList();
        //}

        //if (filtroEstatus.Value != null)
        //{
        //    List = List.Where(x => x.estatus == Convert.ToString(filtroEstatus.Value)).ToList();
        //}

        //if (filtroSaldo.Value != null)
        //{
        //    List = List.Where(x => x.saldo == Convert.ToString(filtroSaldo.Value)).ToList();
        //}

        //if (filtroTipo.Value != null)
        //{
        //    List = List.Where(x => x.tipo == Convert.ToString(filtroTipo.Value)).ToList();
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
    public async Task<JsonResult> ConsultarSubCuentas()
    {
        var ListPF = await _usuariosApiClient.GetUsuariosAsync(1,200);
        //var ListPF = GetList();
        return Json(ListPF);
    }

    [HttpPost]
    public async Task<JsonResult> ConsultarListadoDocumentos(string idAccount)
    {

        var gridData = new ResponseGrid<DocumentosUserResponseGrid>();

        try
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

            List<DocumentosUserResponse> ListPF = new List<DocumentosUserResponse>();
            ListPF = await _usuariosApiClient.GetDocumentosUsuarioAsync(Convert.ToInt32(idAccount));

            var List = new List<DocumentosUserResponseGrid>();
            foreach (var row in ListPF)
            {
                List.Add(new DocumentosUserResponseGrid
                {
                    idCliente = row.idCliente,
                    idDocApodLeg = row.idDocApodLeg,
                    tipoDocumento = row.tipoDocumento,
                    documento = row.documento,
                    numeroIdentificacion = row.numeroIdentificacion,
                    nombreDocumento = row.nombreDocumento,
                    Acciones = await this.RenderViewToStringAsync("~/Views/Usuarios/_AccionesDocumentos.cshtml", row)
                });
            }

            gridData.Data = List;
            gridData.RecordsTotal = List.Count;
            gridData.Data = gridData.Data.Skip(skip).Take(pageSize).ToList();
            filterRecord = string.IsNullOrEmpty(request.Busqueda) ? gridData.RecordsTotal ?? 0 : gridData.Data.Count;
            gridData.RecordsFiltered = filterRecord;
            gridData.Draw = draw;

        } catch (Exception ex) {
            return Json(gridData);
        }

        return Json(gridData);
    }

    [HttpPost]
    public async Task<JsonResult> GestionarEstatusUsuario(int id, string Estatus)
    {

        BloqueoWebResponse bloqueoWebResponse = new BloqueoWebResponse();
        try
        {
            if (Estatus == "True") {

                BloqueoWebUsuarioRequest bloqueoWebRequest = new BloqueoWebUsuarioRequest();
                bloqueoWebRequest.idCliente = id;
                bloqueoWebRequest.estatus = 1;

                bloqueoWebResponse = await _usuariosApiClient.GetBloqueoWeb(bloqueoWebRequest);

            } else if (Estatus == "False") {

                BloqueoWebUsuarioRequest bloqueoWebRequest = new BloqueoWebUsuarioRequest();
                bloqueoWebRequest.idCliente = id;
                bloqueoWebRequest.estatus = 2;

                bloqueoWebResponse = await _usuariosApiClient.GetBloqueoWeb(bloqueoWebRequest);

            } else {
                //Deteccion de posible cambio en codigo HTML a través de inspeccionar elemento
                return Json(BadRequest());
            }

            if (bloqueoWebResponse.error == false) {
                return Json(Ok());
            } else {
                return Json(BadRequest());
            }

        } catch (Exception ex) {
            return Json(BadRequest());
        }
    }

    #endregion

    #region "Modelos"

    public class GenerarNombreResponse
    {
        public string Nombre { get; set; }
        public string NombreCompleto { get; set; }
    }

    public class RequestListFilters
    {
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }
    #endregion

    #region "Funciones Auxiliares"

    private GenerarNombreResponse generarnombre(int i)
    {
        var res = new GenerarNombreResponse();
        Random rnd = new Random((int)DateTime.Now.Ticks);

        i = i - 1;

        String[] nombres = { "Juan", "Pablo", "Paco", "Jose", "Alberto", "Roberto", "Genaro", "Andrea", "Maria", "Carmen" };
        String[] apellidos = { "Sanchez", "Perez", "Lopez", "Torres", "Alvarez", "Martinez", "Guzman", "Rodriguez", "Flores", "Vazquez" };
        String[] apellidosM = { "Vazquez", "Flores", "Rodriguez", "Guzman", "Martinez", "Alvarez", "Torres", "Lopez", "Perez", "Sanchez" };

        var genNombre = nombres[i];

        String gen = genNombre + " " + apellidos[i] + " " + apellidos[i];

        res.Nombre = genNombre;
        res.NombreCompleto = gen;

        return res;
    }

    private string generarNombreArchivo(int i)
    {
        Random rnd = new Random((int)DateTime.Now.Ticks);

        i = i - 1;

        String[] nombres = { "INE", "Comprobante de Domicilio", "RFC", "CURP", "CARNET" };
        var genNombre = nombres[i];

        String gen = genNombre;

        return gen;
    }

    public static string GenNumeroTelefono(int length)
    {
        if (length > 0)
        {
            var sb = new StringBuilder();

            var rnd = SeedRandom();
            for (int i = 0; i < length; i++)
            {
                sb.Append(rnd.Next(0, 9).ToString());
            }

            return sb.ToString();
        }

        return string.Empty;
    }

    public static string GenerarEmail()
    {
        //return string.Format("{0}@{1}.com", GenerateRandomAlphabetString(10), GenerateRandomAlphabetString(10));
        return string.Format("{0}@{1}.com", GenerateRandomAlphabetString(10), "gmail");
    }
    /// <summary>
    /// Gets a string from the English alphabet at random
    /// </summary>
    public static string GenerateRandomAlphabetString(int length)
    {
        string allowedChars = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var rnd = SeedRandom();

        char[] chars = new char[length];
        for (int i = 0; i < length; i++)
        {
            chars[i] = allowedChars[rnd.Next(allowedChars.Length)];
        }

        return new string(chars);
    }
    private static Random SeedRandom()
    {
        return new Random(Guid.NewGuid().GetHashCode());
    }

    public List<UserResponse> GetList()
    {


        var List = new List<UserResponse>();
        Random rnd = new Random();
        const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        String[] characters2 = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };

        try
        {
            for (int i = 1; i <= 10; i++)
            {
                var gen = generarnombre(i);

                var pf = new UserResponse();
                pf.id = i;
                pf.nombreCompleto = gen.NombreCompleto;
                pf.telefono = GenNumeroTelefono(10);
                pf.email = GenerarEmail();
                pf.CURP = "DOOM982834HMCRNS07";
                pf.organizacion = "Doom Organizacion";
                pf.membresia = "Negocios";
                pf.sexo = "Masculino";
                pf.estatus = 1;

                List.Add(pf);
            }
        }
        catch (Exception e)
        {
            List = new List<UserResponse>();
        }

        return List;
    }

    public async Task<List<DocumentosUserResponse>> GetListArchivos(int idUser)
    {
        var List = new List<DocumentosUserResponse>();

        try
        {
            for (int i = 1; i <= 4; i++)
            {
                var gen = generarNombreArchivo(i);

                var pf = new DocumentosUserResponse();
                pf.idCliente = i;
                pf.idDocApodLeg = 100 + i;
                pf.tipoDocumento = i;
                pf.documento = GenNumeroTelefono(5);
                pf.numeroIdentificacion = GenNumeroTelefono(20);
                pf.nombreDocumento = generarNombreArchivo(i);

                List.Add(pf);
            }
        }
        catch (Exception e)
        {
            List = new List<DocumentosUserResponse>();
        }

        return List;
    }

    public async Task<List<UserPermissionResponse>> GetListUser()
    {
        var List = new List<UserPermissionResponse>();
        Random rnd = new Random();
        try
        {
            for (int i = 1; i <= 5; i++)
            {
                var gen = generarnombre(i);

                var pf = new UserPermissionResponse();
                pf.id = i;
                pf.nombreCompleto = gen.NombreCompleto;
                pf.listaGen = generarListaPermisos();
                List.Add(pf);
            }
        }
        catch (Exception e)
        {
            List = new List<UserPermissionResponse>();
        }

        return List;
    }

    private List<ListUserPermissionResponse> generarListaPermisos()
    {
        var res = new List<ListUserPermissionResponse>();
        String[] catalogoVista = { "Usuarios", "Transacciones", "Tarjetas", "Cuentas"};

        for (int i = 0; i <= 3; i++)
        {
            var genPermisos = catalogoVista[i];

            var pf = new ListUserPermissionResponse();
            pf.id = i;
            pf.vista = genPermisos;
            pf.Escritura = false;
            pf.Lectura = true;
            pf.Eliminar = false;
            pf.Actualizar = true;

            res.Add(pf);
        }

        return res;
    }
    #endregion
}
