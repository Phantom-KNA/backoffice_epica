using System.Net;

namespace Epica.Web.Operacion.Helpers
{
    public static class PublicIpHelper
    {
        public static async Task<String> GetPublicIp()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetStringAsync("https://ipinfo.io");
                    string[] responseParts = response.Split('\n');
                    foreach (var part in responseParts)
                    {
                        if (part.Contains("ip"))
                        {
                            string externalIPv4 = part.Split(':')[1].Trim();
                            string ipv4 = externalIPv4.Replace("\"", "").Replace(",", "");
                            return ipv4;
                        }
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
