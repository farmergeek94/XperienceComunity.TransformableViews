using CMS.ContentEngine;
using CMS.ContentEngine.Internal;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Websites.PageBuilder.Internal;
using HBS.Xperience.TransformableViews.Components;
using HBS.Xperience.TransformableViews.Repositories;
using HBS.Xperience.TransformableViewsShared.Library;
using HBS.Xperience.TransformableViewsShared.Models;
using HBS.Xperience.TransformableViewsShared.Repositories;
using HBS.Xperience.TransformableViewsShared.Services;
using Kentico.Content.Web.Mvc.Routing;
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
using XperienceComunity.TransformableViews.Models;

[assembly: RegisterWidget(
    identifier: TransformableViewContentWidgetViewComponent.Identifier,
    customViewName: "~/Components/_TVContentWidget.cshtml",
    name: "Transformable Content",
    propertiesType: typeof(TransformableViewContentWidgetProperties),
    IconClass = "icon-braces-octothorpe")]

namespace HBS.Xperience.TransformableViews.Components
{
    /// <summary>
    /// Widget that allows you to select any content item(s) of a type and match it to a view
    /// </summary>
    public class TransformableViewContentWidgetViewComponent : ViewComponent
    {
        public const string Identifier = "HBS.TransformableViewWidgets";

        private readonly ITransformableViewRepository _transformableViewRepository;
        private readonly IContentQueryExecutor _contentQueryExecutor;
        private readonly IContentItemRetriever _webPageRetriever;
        private readonly ICacheService _cacheService;

        public TransformableViewContentWidgetViewComponent(ITransformableViewRepository transformableViewRepository, IContentQueryExecutor contentQueryExecutor, IContentItemRetriever webPageRetriever, ICacheService cacheService)
        {
            _transformableViewRepository = transformableViewRepository;
            _contentQueryExecutor = contentQueryExecutor;
            _webPageRetriever = webPageRetriever;
            _cacheService = cacheService;
        }
        public async Task<IViewComponentResult> InvokeAsync(TransformableViewContentWidgetProperties properties)
        {
            if (properties.ContentType.Any() && properties.View.Any())
            {
                var viewModel = new TransformableViewModel()
                {
                    ViewTitle = properties.ViewTitle,
                    ViewClassNames = properties.ViewClassNames,
                    ViewCustomContent = properties.ViewCustomContent,
                    Items = await _webPageRetriever.GetContentItems(properties.ContentType.First().ObjectGuid, properties.SelectedContent.Select(x => x.Identifier))
                };

                var view = await _transformableViewRepository.GetTransformableViews(properties.View.FirstOrDefault()?.Identifier ?? Guid.Empty, _cacheService.GetCachedLanguage());
                if(view != null)
                    // Return the database view with the model. 
                    return View($"TransformableView/{view.TransformableDatabaseViewCodeName}", viewModel);
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

        [ContentItemSelectorComponent(TransformableDatabaseContentView.CONTENT_TYPE_NAME, Label = "View", MaximumItems = 1, MinimumItems = 1)]
        public IEnumerable<ContentItemReference> View { get; set; } = [];
    }

    /// <summary>
    /// Filter that allows us to limit the content type allowed based on another field that has been selected on the form.
    /// </summary>
    public class TransformableViewContentTypeFilter : IContentTypesFilter
    {
        /// <summary>
        /// Get the allowed content type identifier for filtering 
        /// </summary>
        public IEnumerable<Guid> AllowedContentTypeIdentifiers
        {
            get
            {
                // Get the http context
                var httpContextAccessor = Service.ResolveOptional<IHttpContextAccessor>();
                var form = httpContextAccessor?.HttpContext?.Request.Form;
                if (form != null)
                {
                    // make sure there is a command
                    if (form.TryGetValue("command", out StringValues command))
                    {
                        // Get the form data passed back to the filter. 
                        if (form.TryGetValue("data", out StringValues data))
                        {
                            try
                            {
                                // parse the form data down to a readable properties format
                                var formData = JsonSerializer.Deserialize<WidgetPropertiesForm<TransformableViewContentWidgetProperties>>(data, new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                });

                                if (formData?.Form != null)
                                {
                                    // Get the content type value for filtering.
                                    var contentType = formData?.Form.ContentType.FirstOrDefault();
                                    // return te content type
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
}
