using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Epica.Web.Operacion.Utilities;
/// <summary>
/// Clase de extension para controladores que proporciona métodos para rendelizar vistas parciales a una cadena.
/// </summary>
public static class ControllerExtensions
{
    /// <summary>
    /// Renderice una vista parcial a la cadena.
    /// </summary>
    /// <typeparam name="TModel">El tipo de modelo utilizado por la vista parcial.</typeparam>
    /// <param name="controller">Controlador por el cual se invoca el método.</param>
    /// <param name="viewNamePath">La ruta de la vista parcial.</param>
    /// <param name="model">Modelo utilizado por la vista parcial.</param>
    /// <returns>Una tarea que representa la operacion y  devuelve la vista parcial como una cadena.</returns>
    public static async Task<string> RenderViewToStringAsync<TModel>(this Controller controller, string viewNamePath, TModel model)
    {
        if (string.IsNullOrEmpty(viewNamePath))
            viewNamePath = controller.ControllerContext.ActionDescriptor.ActionName;

        controller.ViewData.Model = model;

        using (StringWriter writer = new StringWriter())
        {
            try
            {
                IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;

                ViewEngineResult viewResult = null;

                if (viewNamePath.EndsWith(".cshtml"))
                    viewResult = viewEngine.GetView(viewNamePath, viewNamePath, false);
                else
                    viewResult = viewEngine.FindView(controller.ControllerContext, viewNamePath, false);

                if (!viewResult.Success)
                    return $"A view with the name '{viewNamePath}' could not be found";

                ViewContext viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);

                return writer.GetStringBuilder().ToString();
            }
            catch (Exception exc)
            {
                return $"Failed - {exc.Message}";
            }
        }
    }

    /// <summary>
    /// Renderiza una vista parcial a una cadena sin un modelo presente.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="controller">El controlador desde el cual se invoca el metodo.</param>
    /// <param name="viewNamePath">La vista de la ruta parcial a renderizar.</param>
    /// <returns>Operacion asincróna y devuelve la vista parcial renderizada. </returns>
    public static async Task<string> RenderViewToStringAsync(this Controller controller, string viewNamePath)
    {
        if (string.IsNullOrEmpty(viewNamePath))
            viewNamePath = controller.ControllerContext.ActionDescriptor.ActionName;

        using (StringWriter writer = new StringWriter())
        {
            try
            {
                IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;

                ViewEngineResult viewResult = null;

                if (viewNamePath.EndsWith(".cshtml"))
                    viewResult = viewEngine.GetView(viewNamePath, viewNamePath, false);
                else
                    viewResult = viewEngine.FindView(controller.ControllerContext, viewNamePath, false);

                if (!viewResult.Success)
                    return $"A view with the name '{viewNamePath}' could not be found";

                ViewContext viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);

                return writer.GetStringBuilder().ToString();
            }
            catch (Exception exc)
            {
                return $"Failed - {exc.Message}";
            }
        }
    }

}
