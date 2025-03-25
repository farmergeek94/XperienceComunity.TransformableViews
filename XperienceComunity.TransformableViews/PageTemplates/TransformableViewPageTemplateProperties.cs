using CMS.ContentEngine;
using CMS.Core;
using CMS.DataEngine;
using HBS.Xperience.TransformableViews.PageTemplates;
using HBS.Xperience.TransformableViewsShared.Library;
using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Admin.Base.Forms;
using XperienceComunity.TransformableViews;

[assembly: RegisterPageTemplate(TransformableViews.PageTemplate, "Transformable View Template", typeof(TransformableViewPageTemplateProperties), "~/PageTemplates/_TVPageTemplate.cshtml", Description = "Page Template for using the transformable views")]

namespace HBS.Xperience.TransformableViews.PageTemplates
{
    /// <summary>
    /// Transformable page template.
    /// </summary>
    public class TransformableViewPageTemplateProperties : IPageTemplateProperties
    {
        [ContentItemSelectorComponent(TransformableDatabaseLayoutView.CONTENT_TYPE_NAME, MinimumItems = 1, MaximumItems = 1, Label = "Layout View", ExplanationText = "Falls back to system layout if left empty.")]
        public IEnumerable<ContentItemReference> Layout { get; set; } = [];

        [ContentItemSelectorComponent(TransformableDatabasePageView.CONTENT_TYPE_NAME, MaximumItems = 1, Label = "Page View")]
        public IEnumerable<ContentItemReference> View { get; set; } = [];
    }
}
