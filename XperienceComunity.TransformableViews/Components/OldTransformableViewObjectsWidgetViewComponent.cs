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
    identifier: OldTransformableViewObjectsWidgetViewComponent.Identifier,
    customViewName: "~/Components/_OldTVObjects.cshtml",
    name: "(Old) Transformable View Objects",
    propertiesType: typeof(OldTransformableViewObjectsWidgetProperties),
    IconClass = "icon-layout")]

namespace HBS.Xperience.TransformableViews.Components
{
    /// <summary>
    /// Widget allowing us to select some specific object type, setup the where, columns, topn and order by and filter accordingly.
    /// </summary>
    public class OldTransformableViewObjectsWidgetViewComponent : ViewComponent
    {
        public const string Identifier = "HBS.TransformableViewObjects";
        private readonly IContentItemRetriever _contentItemRetriever;

        public OldTransformableViewObjectsWidgetViewComponent(IContentItemRetriever contentItemRetriever)
        {
            _contentItemRetriever = contentItemRetriever;
        }
        public async Task<IViewComponentResult> InvokeAsync(TransformableViewObjectsFormComponentModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.ClassName))
            {
                // getting the objects based on what was selected for the views. 
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

    public class OldTransformableViewObjectsWidgetProperties : IWidgetProperties
    {
        [TransformableViewObjectsFormComponent]
        public TransformableViewObjectsFormComponentModel Model { get; set; } = new TransformableViewObjectsFormComponentModel();
    }
}
