﻿using Epica.Web.Operacion.Models.Common;
using Epica.Web.Operacion.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Epica.Web.Operacion.Controllers;

public class CuentaController : Controller
{
    #region "Funciones"
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<JsonResult> Consulta(List<RequestListFilters> filters)
    {
        //Temporalmente vamos a trabajar con datos en local en lo que se trabaja en la api de consumo
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

        var gridData = new ResponseGrid<CuentasResponseGrid>();
        var ListPF = GetList();
        //var List = new List<FoliosCanceladosResponseGrid>();

        var List = new List<CuentasResponseGrid>();
        foreach (var row in ListPF)
        {
            List.Add(new CuentasResponseGrid
            {
                Id = row.Id,
                cliente = row.cliente,
                noCuenta = row.noCuenta,
                saldo = row.saldo,
                estatus = row.estatus,
                tipo = row.tipo,
                Acciones = await this.RenderViewToStringAsync("~/Views/Cuenta/_Acciones.cshtml", row)
            });
        }

        //Aplicacion de Filtros temporal, 
        var filtroid = filters.FirstOrDefault(x => x.Key == "Id");
        var filtronombreCliente = filters.FirstOrDefault(x => x.Key == "NombreCliente");
        var filtroNoCuenta = filters.FirstOrDefault(x => x.Key == "noCuenta");
        var filtroEstatus = filters.FirstOrDefault(x => x.Key == "estatus");
        var filtroSaldo = filters.FirstOrDefault(x => x.Key == "saldo");
        var filtroTipo = filters.FirstOrDefault(x => x.Key == "tipo");

        if (filtroid.Value != null) {
            List = List.Where(x => x.Id == Convert.ToInt32(filtroid.Value)).ToList();
        }

        if (filtronombreCliente.Value != null) {
            List = List.Where(x => x.cliente == Convert.ToString(filtronombreCliente.Value)).ToList();
        }

        if (filtroNoCuenta.Value != null) {
            List = List.Where(x => x.noCuenta == Convert.ToString(filtroNoCuenta.Value)).ToList();
        }

        if (filtroEstatus.Value != null) {
            List = List.Where(x => x.estatus == Convert.ToString(filtroEstatus.Value)).ToList();
        }

        if (filtroSaldo.Value != null) {
            List = List.Where(x => x.saldo == Convert.ToString(filtroSaldo.Value)).ToList();
        }

        if (filtroTipo.Value != null) {
            List = List.Where(x => x.tipo == Convert.ToString(filtroTipo.Value)).ToList();
        }

        //List.Add(new EstadisiticaUsoFormaValoradaResponse() { }, new EstadisiticaUsoFormaValoradaResponse() { });
        gridData.Data = List;
        gridData.RecordsTotal = List.Count;
        gridData.Data = gridData.Data.Skip(skip).Take(pageSize).ToList();
        filterRecord = string.IsNullOrEmpty(request.Busqueda) ? gridData.RecordsTotal ?? 0 : gridData.Data.Count;
        gridData.RecordsFiltered = filterRecord;
        gridData.Draw = draw;

        //var returnObj = new
        //{
        //    draw,
        //    recordsTotal = totalRecord,
        //    recordsFiltered = filterRecord,
        //    data = List
        //};

        return Json(gridData);
    }

    #endregion

    #region "Modelos"
    public class CuentasResponse
    {
        public long Id { get; set; }
        public string noCuenta { get; set; }
        public string cliente { get; set; }
        public string estatus { get; set; }
        public string saldo { get; set; }
        public string tipo { get; set; }
    }

    public class CuentasResponseGrid : CuentasResponse
    {
        public string Acciones { get; set; }
    }

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

    public List<CuentasResponse> GetList()
    {


        var List = new List<CuentasResponse>();
        Random rnd = new Random();
        const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        String[] characters2 = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };

        try
        {
            for (int i = 1; i <= 10; i++)
            {
                var gen = generarnombre(i);

                var pf = new CuentasResponse();
                pf.Id = i;
                pf.cliente = gen.NombreCompleto;
                pf.noCuenta = string.Format("465236478963245{0}", i);
                pf.saldo = rnd.Next(0001, 99999).ToString("C2");
                pf.estatus = "Activo";
                pf.tipo = "Credito";

                List.Add(pf);
            }
        }
        catch (Exception e)
        {
            List = new List<CuentasResponse>();
        }

        return List;
    }
    #endregion
}
