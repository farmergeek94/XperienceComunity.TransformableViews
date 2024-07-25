using HBS.Xperience.TransformableViews.Components;
using HBS.Xperience.TransformableViews.Repositories;
using HBS.Xperience.TransformableViewsShared.Library;
using HBS.Xperience.TransformableViewsShared.Models;
using HBS.Xperience.TransformableViewsShared.Repositories;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: RegisterWidget(
    identifier: TransformableViewObjectsWidgetViewComponent.Identifier,
    customViewName: "~/Components/_TVObjects.cshtml",
    name: "Transformable View Objects",
    propertiesType: typeof(TransformableViewObjectsWidgetProperties),
    IconClass = "icon-layout")]

namespace HBS.Xperience.TransformableViews.Components
{
    public class TransformableViewObjectsWidgetViewComponent : ViewComponent
    {
        public const string Identifier = "HBS.TransformableViewObjects";
        private readonly IContentItemRetriever _contentItemRetriever;

        public TransformableViewObjectsWidgetViewComponent(IContentItemRetriever contentItemRetriever)
        {
            _contentItemRetriever = contentItemRetriever;
        }
        public async Task<IViewComponentResult> InvokeAsync(TransformableViewObjectsFormComponentModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.ClassName))
            {
                var items = await _contentItemRetriever.GetObjectItems(model);

                var viewModel = new TransformableViewModel()
                {
                    ViewTitle = model.ViewTitle,
                    ViewClassNames = model.ViewClassNames,
                    ViewCustomContent = model.ViewCustomContent,
                    Items = items.ToArray()
                };
                return View(model.View, viewModel);
                //return Content(string.Empty);
            }
            return Content(string.Empty);
        }
    }

    public class TransformableViewObjectsWidgetProperties : IWidgetProperties
    {
        [TransformableViewObjectsFormComponent]
        public TransformableViewObjectsFormComponentModel Model { get; set; } = new TransformableViewObjectsFormComponentModel();
    }
}
