using System.Net;

namespace Epica.Web.Operacion.Helpers
{
    public static class PublicIpHelper
    {
        public static string GetPublicIp()
        {
            try
            {
                string publicIp = new WebClient().DownloadString("https://ipinfo.io/ip").Trim();

                return publicIp;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener la dirección IP pública: " + ex.Message);
                return null;
            }
        }
    }
}
