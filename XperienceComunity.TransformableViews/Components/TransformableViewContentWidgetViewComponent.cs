using CMS.ContentEngine;
using CMS.ContentEngine.Internal;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Websites.PageBuilder.Internal;
using HBS.TransformableViews_Experience;
using HBS.Xperience.TransformableViews.Components;
using HBS.Xperience.TransformableViews.Repositories;
using HBS.Xperience.TransformableViewsShared.Library;
using HBS.Xperience.TransformableViewsShared.Models;
using HBS.Xperience.TransformableViewsShared.Repositories;
using Kentico.Forms.Web.Mvc;
using Kentico.Forms.Web.Mvc.Internal;
using Kentico.Forms.Web.Mvc.Widgets.Internal;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc.Internal;
using Kentico.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Admin.Base.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

[assembly: RegisterWidget(
    identifier: TransformableViewContentWidgetViewComponent.Identifier,
    customViewName: "~/Components/_TVContentWidget.cshtml",
    name: "Transformable View Widget",
    propertiesType: typeof(TransformableViewContentWidgetProperties),
    IconClass = "icon-braces-octothorpe")]

namespace HBS.Xperience.TransformableViews.Components
{
    public class TransformableViewContentWidgetViewComponent : ViewComponent
    {
        public const string Identifier = "HBS.TransformableViewWidgets";

        private readonly ITransformableViewRepository _transformableViewRepository;
        private readonly IContentQueryExecutor _contentQueryExecutor;
        private readonly IContentItemRetriever _webPageRetriever;

        public TransformableViewContentWidgetViewComponent(ITransformableViewRepository transformableViewRepository, IContentQueryExecutor contentQueryExecutor, IContentItemRetriever webPageRetriever)
        {
            _transformableViewRepository = transformableViewRepository;
            _contentQueryExecutor = contentQueryExecutor;
            _webPageRetriever = webPageRetriever;
        }
        public async Task<IViewComponentResult> InvokeAsync(TransformableViewContentWidgetProperties properties)
        {
            if (properties.ContentType.Any())
            {
                var viewModel = new TransformableViewModel()
                {
                    ViewTitle = properties.ViewTitle,
                    ViewClassNames = properties.ViewClassNames,
                    ViewCustomContent = properties.ViewCustomContent,
                    Items = await _webPageRetriever.GetContentItems(properties.ContentType.First().ObjectGuid, properties.SelectedContent.Select(x => x.Identifier))
                };

                // Return the database view with the model. 
                return View(properties.View.FirstOrDefault()?.ObjectCodeName, viewModel);
            }
            return Content(string.Empty);
        }
    }

    public class TransformableViewContentWidgetProperties : IWidgetProperties
    {
        [ObjectSelectorComponent(DataClassInfo.OBJECT_TYPE, WhereConditionProviderType = typeof(TransformableViewContentTypeWhere), OrderBy = ["ClassDisplayName"], Label = "Content Type", IdentifyObjectByGuid = true)]
        public IEnumerable<ObjectRelatedItem> ContentType { get; set; } = [];

        [ContentItemSelectorComponent(typeof(TransformableViewContentTypeFilter), Label = "Selected Content Items", Order = 1, AllowContentItemCreation = false, MinimumItems = 1)]
        [VisibleIfNotEmpty(nameof(ContentType))]
        public IEnumerable<ContentItemReference> SelectedContent { get; set; } = [];

        [TextInputComponent(Label = "View Title")]
        public string ViewTitle { get; set; } = string.Empty;


        [TextInputComponent(Label = "View CSS Classes")]
        public string ViewClassNames { get; set; } = string.Empty;

        [TextAreaComponent(Label = "View Custom Content")]
        public string ViewCustomContent { get; set; } = string.Empty;

        [ObjectSelectorComponent(TransformableViewInfo.OBJECT_TYPE, WhereConditionProviderType = typeof(TransformableViewWhere), OrderBy = ["TransformableViewDisplayName"], Label = "View")]
        public IEnumerable<ObjectRelatedItem> View { get; set; } = [];
    }

    public class TransformableViewContentTypeFilter : IContentTypesFilter
    {
        public IEnumerable<Guid> AllowedContentTypeIdentifiers
        {
            get
            {
                var httpContextAccessor = Service.ResolveOptional<IHttpContextAccessor>();
                var form = httpContextAccessor.HttpContext?.Request.Form;
                if (form != null)
                {
                    if (form.TryGetValue("command", out StringValues command))
                    {
                        // Get the form data passed back to the filter. 
                        if (form.TryGetValue("data", out StringValues data))
                        {
                            try
                            {
                                var formData = JsonSerializer.Deserialize<TransformableViewContentWidgetPropertiesForm>(data, new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                });

                                if (formData?.Form != null)
                                {
                                    // Get the content type value for filtering.
                                    var contentType = formData?.Form.ContentType.FirstOrDefault();

                                    return contentType != null && contentType.ObjectGuid.HasValue ? [contentType.ObjectGuid.Value] : [];
                                }
                            }
                            catch (Exception)
                            {
                                return [];
                            }
                        }
                    }
                }
                return [];
            }
        }
    }

    public class TransformableViewContentTypeWhere : IObjectSelectorWhereConditionProvider
    {
        // Where condition limiting the content type to only reuasble content types
        public WhereCondition Get() => new WhereCondition().WhereEquals(nameof(DataClassInfo.ClassType), "Content").WhereEquals(nameof(DataClassInfo.ClassContentTypeType), "Reusable");
    }

    public class TransformableViewWhere : IObjectSelectorWhereConditionProvider
    {
        // Where Limiting the View type to transformable.
        public WhereCondition Get()
        {
            var where = new WhereCondition().WhereEquals(nameof(TransformableViewInfo.TransformableViewType), (int)TransformableViewTypeEnum.Transformable);
            var httpContextAccessor = Service.ResolveOptional<IHttpContextAccessor>();
            var form = httpContextAccessor.HttpContext?.Request.Form;
            if (form != null)
            {
                if (form.TryGetValue("command", out StringValues command))
                {
                    // Get the form data passed back to the filter. 
                    if (form.TryGetValue("data", out StringValues data))
                    {
                        try
                        {
                            var formData = JsonSerializer.Deserialize<TransformableViewContentWidgetPropertiesForm>(data, new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });

                            if (formData?.Form != null)
                            {
                                // Get the content type value for filtering.
                                var contentType = formData?.Form.ContentType.FirstOrDefault();

                                var dataClass = DataClassInfoProvider.GetClasses().WhereEquals(nameof(DataClassInfo.ClassGUID), contentType.ObjectGuid.HasValue ? contentType.ObjectGuid.Value : Guid.Empty).FirstOrDefault();

                                if (dataClass != null)
                                {
                                    where = where.WhereEquals(nameof(TransformableViewInfo.TransformableViewClassName), dataClass.ClassName);
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            return where;
        }
    }

    /// <summary>
    /// Class wrapper around the FormData field passed back in request.
    /// </summary>
    public class TransformableViewContentWidgetPropertiesForm
    {
        public TransformableViewContentWidgetProperties Form => FormData == null ? FieldValues : FormData;
        public TransformableViewContentWidgetProperties FormData { get; set; }
        public TransformableViewContentWidgetProperties FieldValues { get; set; }
    }
}
