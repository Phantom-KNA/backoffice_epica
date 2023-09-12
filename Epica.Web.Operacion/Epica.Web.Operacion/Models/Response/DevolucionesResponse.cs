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
        public int idTransaccion { get; set; }
        public string ClaveRastreo { get; set; }
        public string CuentaOrigen { get; set; }
        public decimal Monto { get; set; }
        public string Concepto { get; set; }
        public string FechaAlta { get; set; }
        public string CuentaDestino { get; set; }
        public string BancoTxt { get; set; }
        public Boolean CanDevolver { get; set; }

    }

    public class DevolucionesResponseGrid : DevolucionesResponse
    {
        public string Acciones { get; set; }
    }
}
