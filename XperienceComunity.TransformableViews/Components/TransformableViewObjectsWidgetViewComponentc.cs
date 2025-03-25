using CMS.ContentEngine;
using CMS.Membership;
using HBS.Xperience.TransformableViews.Components;
using HBS.Xperience.TransformableViews.Repositories;
using HBS.Xperience.TransformableViewsShared.Models;
using HBS.Xperience.TransformableViewsShared.Repositories;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XperienceComunity.TransformableViews.Repositories.GeneralSelectors;

[assembly: RegisterWidget(
    identifier: TransformableViewObjectsWidgetViewComponent.Identifier,
    customViewName: "~/Components/_TVObjects.cshtml",
    name: "Transformable View Objects",
    propertiesType: typeof(TransformableViewObjectsWidgetProperties),
    IconClass = "icon-layout")]

namespace HBS.Xperience.TransformableViews.Components
{
    /// <summary>
    /// Widget allowing us to select some specific object type, setup the where, columns, topn and order by and filter accordingly.
    /// </summary>
    public class TransformableViewObjectsWidgetViewComponent : ViewComponent
    {
        public const string Identifier = "HBS.TransformableViewObjectsWidget";
        private readonly IContentItemRetriever _contentItemRetriever;
        private readonly ITransformableViewRepository _transformableViewRepository;

        public TransformableViewObjectsWidgetViewComponent(IContentItemRetriever contentItemRetriever, ITransformableViewRepository transformableViewRepository)
        {
            _contentItemRetriever = contentItemRetriever;
            _transformableViewRepository = transformableViewRepository;
        }
        public async Task<IViewComponentResult> InvokeAsync(TransformableViewObjectsWidgetProperties model)
        {
            if (!string.IsNullOrWhiteSpace(model.ClassName))
            {
                var view = await _transformableViewRepository.GetTransformableViews(model.View.FirstOrDefault()?.Identifier ?? Guid.Empty);
                var pModel = new TransformableViewObjectsFormComponentModel
                {
                    ClassName = model.ClassName,
                    Columns = string.Join(',', model.Columns),
                    WhereCondition = model.WhereCondition,
                    OrderBy = string.Join(',', model.OrderBy),
                    TopN = model.TopN,
                    ViewClassNames = model.ViewClassNames,
                    ViewCustomContent = model.ViewCustomContent,
                    ViewTitle = model.ViewTitle,
                };
                // getting the objects based on what was selected for the views. 
                var items = await _contentItemRetriever.GetObjectItems(pModel);

                var viewModel = new TransformableViewModel()
                {
                    ViewTitle = model.ViewTitle,
                    ViewClassNames = model.ViewClassNames,
                    ViewCustomContent = model.ViewCustomContent,
                    Items = items.ToArray()
                };
                return View($"TransformableView/{view?.TransformableDatabaseViewCodeName}", viewModel);
                //return Content(string.Empty);
            }
            return Content(string.Empty);
        }
    }

    public class TransformableViewObjectsWidgetProperties :IWidgetProperties
    {

        [SingleGeneralSelectorComponent(
            dataProviderType: typeof(ClassGeneralSelectorDataProvider),
            Label = "Class",
            Placeholder = "(Choose a Class)", Order = 1)]
        public string ClassName { get; set; } = string.Empty;

        [GeneralSelectorComponent(dataProviderType: typeof(ColumnsGeneralSelectorDataProvider),
            Label = "Columns",
            Placeholder = "(Select Columns)", Order = 2)]
        public IEnumerable<string> Columns { get; set; } = [];

        [TextInputComponent(Label = "Where Condition", Order = 3)]
        public string WhereCondition { get; set; } = string.Empty;

        [GeneralSelectorComponent(dataProviderType: typeof(ColumnsGeneralSelectorDataProvider),
            Label = "Order By",
            Placeholder = "(Select Columns)", Order = 4)]
        public IEnumerable<string> OrderBy { get; set; } = [];

        [NumberInputComponent(Label = "TopN", Order = 5)]
        public int? TopN { get; set; } = null;

        [ContentItemSelectorComponent(
            TransformableDatabaseClassView.CONTENT_TYPE_NAME,
            Label = "View", Order = 899, MaximumItems = 1, MinimumItems = 1)]
        public IEnumerable<ContentItemReference> View { get; set; } = [];

        [TextInputComponent(Label = "View Title", Order = 900)]
        public string ViewTitle { get; set; } = "";
        [TextInputComponent(Label = "View Classes", Order = 901)]
        public string ViewClassNames { get; set; } = "";
        [TextAreaComponent(Label = "View Custom Content", Order = 902)]
        public string ViewCustomContent { get; set; } = "";
    }
}
