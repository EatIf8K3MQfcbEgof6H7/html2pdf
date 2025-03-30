using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Abstractions;

using System.IO;

public class ViewRenderService : IViewRenderService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ICompositeViewEngine _viewEngine;
    private readonly ITempDataProvider _tempDataProvider;

    public ViewRenderService(IServiceProvider serviceProvider, ICompositeViewEngine viewEngine, ITempDataProvider tempDataProvider)
    {
        _serviceProvider = serviceProvider;
        _viewEngine = viewEngine;
        _tempDataProvider = tempDataProvider;
    }

    public async Task<string> RenderToStringAsync(string viewName, object model)
    {
        var actionContext = new ActionContext(
            new DefaultHttpContext { RequestServices = _serviceProvider },
            new Microsoft.AspNetCore.Routing.RouteData(),
            new ActionDescriptor()
        );

        using var sw = new StringWriter();
        var viewResult = _viewEngine.FindView(actionContext, viewName, false);

        if (viewResult.View == null)
            throw new ArgumentNullException($"{viewName} not found.");

        var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
        {
            Model = model
        };

        var tempData = new TempDataDictionary(actionContext.HttpContext, _tempDataProvider);
        var viewContext = new ViewContext(actionContext, viewResult.View, viewDictionary, tempData, sw, new HtmlHelperOptions());

        await viewResult.View.RenderAsync(viewContext);
        return sw.ToString();
    }
}