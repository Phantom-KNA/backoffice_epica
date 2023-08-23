﻿using Epica.Web.Operacion.Models.Request;

namespace Epica.Web.Operacion.Services.Transaccion
{
    public interface ITransaccionesApiClient
    {
        Task<List<TransaccionesResponse>> GetTransaccionesAsync(int pageNumber, int recordsTotal);
        Task<TransaccionesResponse> GetTransaccionAsync(int idInterno);
        Task<int> GetTotalTransaccionesAsync();
        Task<List<TransaccionesResponse>> GetTransaccionesCuentaAsync(int idCuenta);
        Task<MensajeResponse> GetRegistroTransaccion(RegistrarTransaccionRequest request);
        Task<MensajeResponse> GetModificarTransaccion(RegistrarTransaccionRequest request);
        Task<TransaccionDetailsResponse> GetTransaccionDetalleAsync(int idCuenta);
        Task<TransaccionDetailsResponse> GetTransaccionDetalleByCobranzaAsync(string cobranzaRef);

    }
}
