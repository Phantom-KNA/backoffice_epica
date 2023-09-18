using System.Text.Json.Serialization;

namespace Epica.Web.Operacion.Models.Response
{
    public class DocumentoShowResponse
    {
        public Stream Documento { get; set; }
        public string? MimeType { get; set; }
        public string? Nombre { get; set; }
    }
}
