using Epica.Web.Operacion.Models.Common;

namespace Epica.Web.Operacion.Utilities
{
    /// <summary>
    /// Clase de extensión para HttpRequest que da los métodos  para el manejo de solicitudes .
    /// </summary>
    public static class HttpRequestExtension
    {
        /// <summary>
        /// Obtiene un objeto de tipo T que representa la solicitud de cuadrícula y asigna el valor de la variable 'draw'.
        /// </summary>
        /// <typeparam name="T">El tipo del objeto de solicitud .</typeparam>
        /// <param name="request">HttpRequest.</param>
        /// <param name="draw">La variable 'draw' que se asignará.</param>
        /// <returns>Un objeto de tipo T que representa la solicitud de cuadrícula.</returns>
        /// <exception cref="Exception">Se lanza cuando el objeto predeterminado es nulo o el formulario de la solicitud está vacío.</exception>
        public static T GridRequest<T>(this HttpRequest request, out string draw) where T : GridRequestViewModel
        {
            try
            {
                draw = request.Form["draw"].FirstOrDefault() ?? string.Empty;
                var sortColumn = request.Form["columns[" + request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(request.Form["start"].FirstOrDefault() ?? "0");
                var page = skip / pageSize + 1;

                var gridRequestDTO = Activator.CreateInstance(typeof(T)) as T;
                if (gridRequestDTO != null)
                {
                    gridRequestDTO.SortColumn = sortColumn;
                    gridRequestDTO.Sort = sortColumnDirection;
                    gridRequestDTO.SearchValue = searchValue;
                    gridRequestDTO.Page = page;
                    gridRequestDTO.PageSize = pageSize;
                    gridRequestDTO.Skip = skip;

                    return gridRequestDTO;
                }

                throw new Exception("Object default is null");
            }
            catch
            {
                throw new Exception("Request Form empty");
            }
        }
    }
}
