namespace Epica.Web.Operacion.Helpers
{
    public static class HoraHelper
    {
        public static DateTime GetHoraCiudadMexico()
        {
            try
            {
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                DateTime horaCiudadMexico = TimeZoneInfo.ConvertTime(DateTime.Now, tz);
                return horaCiudadMexico;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener la hora de ciudad de México " + ex.Message);
                return DateTime.Now;
            }
        }
    }
}
