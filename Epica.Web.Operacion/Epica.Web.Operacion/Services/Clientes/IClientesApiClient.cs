﻿using Epica.Web.Operacion.Models.Entities;
using Epica.Web.Operacion.Models.Request;
using static Epica.Web.Operacion.Controllers.CuentaController;

namespace Epica.Web.Operacion.Services.Transaccion
{
    public interface IClientesApiClient
    {
        Task<object> GetClientesbyNombreAsync(string nombreCliente);
        Task<List<ClienteResponse>> GetClientesAsync(int pageNumber, int recordsTotal);
        Task<int> GetTotalClientesAsync();
        Task<ClienteDetailsResponse> GetClienteAsync(int id);
        Task<List<DocumentosClienteResponse>> GetDocumentosClienteAsync(int id);
        Task<BloqueoWebResponse> GetBloqueoWeb(BloqueoWebClienteRequest request);
        Task<BloqueoTotalResponse> GetBloqueoTotal(BloqueoTotalClienteRequest request);
        Task<MensajeResponse> GetRegistroCliente(RegistroModificacionClienteRequest request);
        Task<ClienteDetailsResponse> GetDetallesCliente(int id);
        Task<MensajeResponse> GetModificarCliente(RegistroModificacionClienteRequest request);
        //Task<RegistrarModificarClienteResponse> GetModificaCliente(RegistroModificacionClienteRequest request);

    }
}
