using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Models.Response
{
    public class DocumentosUsuarioResponse
    {
        public int IdCliente { get; set; }
        public int IdDocApodLeg{ get; set; }
        public int TipoDocumento { get; set; }
        public string? Documento  { get; set; }
        public string? NumeroIdentificacion { get; set; }
        public string? NombreDocumento { get; set; }
    }
}
