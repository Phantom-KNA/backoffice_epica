using System.Collections.Generic;

namespace Epica.Web.Operacion.Models.Response
{
    public class DevolucionesDetailsgeneralResponse
    {
        public string name { get; set; }
        public List<TransaccionesResponse>  value { get; set; }

    }

    public class DevolucionesResponse
    {
        public int Id { get; set; }
        public string? ClaveRastreo { get; set; }
        public string? CuentaOrigen { get; set; }
        public string? Concepto { get; set; }
        public int? IdBanco { get; set; }
        public string? BancoTxt { get; set; }
        public int? TipoCuenta { get; set; }
        public string? CuentaDestino { get; set; }
        public string? BeneficiarioDestino { get; set; }
        public string? RfcDestino { get; set; }
        public decimal? Monto { get; set; }
        public decimal? Iva { get; set; }
        public string? ReferenciaCobro { get; set; }
        public string? ReferenciaNumerica { get; set; }
        public string? TimeStamp { get; set; }
        public string? Uuid { get; set; }
        public int? Estatus { get; set; }
        public string? Adicional { get; set; }
        public int? IdUsuarioAlquimia { get; set; }
        public string? Instancia { get; set; }
        public string? CuentaEje { get; set; }
        public string? CorreoBeneficiario { get; set; }
        public string? EstatusTercero { get; set; }
        public int? IdTransaccionSpei { get; set; }
        public string? JsonEnvio { get; set; }
        public DateTime? FechaAlta { get; set; }
        public string? FechaActualizacion { get; set; }
        public string? FechaEnviada { get; set; }
        public string? FechaLiquidada { get; set; }
        public int IdDevolucion { get; set; }
        public bool CanDevolver { get; set; }

    }

    public class DevolucionesResponseGrid : DevolucionesResponse
    {
        public string Acciones { get; set; }
    }
}
