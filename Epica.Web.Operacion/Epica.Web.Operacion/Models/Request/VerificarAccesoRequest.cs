namespace Epica.Web.Operacion.Models.Request
{
    public class VerificarAccesoRequest
    {
        public string IdCliente { get; set; }
        public string Nip { get; set; }
        public string Token { get; set; }
    }
}
