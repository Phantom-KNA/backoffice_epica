using Epica.Web.Operacion.Config;
using Epica.Web.Operacion.Models.Common;
using Epica.Web.Operacion.Services.Transaccion;
using Epica.Web.Operacion.Services.UserResolver;
using Epica.Web.Operacion.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace Epica.Web.Operacion.Controllers;

public class UsuariosController : Controller
{
    #region "Locales"
    private readonly ICuentaApiClient _cuentaApiClient;
    #endregion

    #region "Constructores"
    public UsuariosController(ICuentaApiClient cuentaApiClient)
    {
        _cuentaApiClient = cuentaApiClient;
    }
    #endregion

    #region "Funciones"
    public IActionResult Index()
    {
        return View();
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

        //ListPF = await _cuentaApiClient.GetCuentasAsync();

        //Entorno local de pruebas
        ListPF = GetList();

        var List = new List<UserResponseGrid>();
        foreach (var row in ListPF)
        {
            List.Add(new UserResponseGrid
            {
                IdCliente = row.IdCliente,
                Nombre = row.Nombre,
                Telefono = row.Telefono,
                Email = row.Email,
                Curp = row.Curp,
                Organizacion = row.Organizacion,
                TipoMembresia = row.TipoMembresia,
                Sexo = row.Sexo,
                Estatus = row.Estatus,
                Acciones = await this.RenderViewToStringAsync("~/Views/Usuarios/_Acciones.cshtml", row)
            });
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
        var ListPF = await _cuentaApiClient.GetCuentasAsync();
        //var ListPF = GetList();
        return Json(ListPF);
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
                pf.IdCliente = i;
                pf.Nombre = gen.NombreCompleto;
                pf.Telefono = GenNumeroTelefono(10);
                pf.Email = GenerarEmail();
                pf.Curp = "DOOM982834HMCRNS07";
                pf.Organizacion = "Doom Organizacion";
                pf.TipoMembresia = "Negocios";
                pf.Sexo = "Masculino";
                pf.Estatus = true;

                List.Add(pf);
            }
        }
        catch (Exception e)
        {
            List = new List<UserResponse>();
        }

        return List;
    }
    #endregion
}
