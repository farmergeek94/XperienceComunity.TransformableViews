using CMS.Core;
using CMS.DataEngine;
using HBS.TransformableViews_Experience;
using HBS.Xperience.TransformableViews.PageTemplates;
using HBS.Xperience.TransformableViewsShared.Library;
using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Admin.Base.Forms;

[assembly: RegisterPageTemplate("HBS.TransformableViewPageTemplate", "Transformable View Template", typeof(TransformableViewPageTemplateProperties), "~/PageTemplates/_TVPageTemplate.cshtml", Description = "Page Template for using the transformable views")]

namespace HBS.Xperience.TransformableViews.PageTemplates
{
    /// <summary>
    /// Transformable page template.
    /// </summary>
    public class TransformableViewPageTemplateProperties : IPageTemplateProperties
    {
        [ObjectSelectorComponent(TransformableViewInfo.OBJECT_TYPE, WhereConditionProviderType = typeof(TransformableViewPageLayoutWhere), OrderBy = [nameof(TransformableViewInfo.TransformableViewDisplayName)], MaximumItems = 1, Placeholder = "(Select a Layout)", ExplanationText = "Falls back to system layout if left empty.")]
        public IEnumerable<ObjectRelatedItem> Layout { get; set; } = [];

        [ObjectSelectorComponent(TransformableViewInfo.OBJECT_TYPE, WhereConditionProviderType = typeof(TransformableViewPageWhere), OrderBy = [nameof(TransformableViewInfo.TransformableViewDisplayName)], MaximumItems = 1)]
        public IEnumerable<ObjectRelatedItem> View { get; set; } = [];

    }

    internal class TransformableViewPageWhere : IObjectSelectorWhereConditionProvider
    {

        // Where condition limiting the selectable objects
        public WhereCondition Get()
        {
            var where = new WhereCondition().WhereEquals(nameof(TransformableViewInfo.TransformableViewType), (int)TransformableViewTypeEnum.Page);
            // Note not working currently as the page context is coming back null.
            var pageDataContextRetriever = Service.ResolveOptional<IWebPageDataContextRetriever>();
            if(pageDataContextRetriever != null)
            {
                if(pageDataContextRetriever.TryRetrieve(out WebPageDataContext data)) {
                    where = where.WhereEquals(nameof(TransformableViewInfo.TransformableViewClassName), data.WebPage.ContentTypeName);
                }
            }

            return where;
        }
    }

    internal class TransformableViewPageLayoutWhere : IObjectSelectorWhereConditionProvider
    {
        // Where condition limiting the selectable objects
        public WhereCondition Get() => new WhereCondition().WhereEquals(nameof(TransformableViewInfo.TransformableViewType), (int)TransformableViewTypeEnum.Layout);
    }
}
