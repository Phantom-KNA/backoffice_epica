using Epica.Web.Operacion.Models.Common;

namespace Epica.Web.Operacion.Utilities
{
    public static class HttpRequestExtension
    {
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
