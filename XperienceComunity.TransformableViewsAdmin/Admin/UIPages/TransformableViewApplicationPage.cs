


using HBS.Xperience.TransformableViewsAdmin.Admin.UIPages;
using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.UIPages;

[assembly: UIApplication(
    identifier: TransformableViewApplicationPage.IDENTIFIER,
    type: typeof(TransformableViewApplicationPage),
    slug: "transformable-views",
    name: "Transformable Views",
    category: BaseApplicationCategories.CONFIGURATION,
    icon: Icons.Layouts,
    templateName: TemplateNames.SECTION_LAYOUT)]



namespace HBS.Xperience.TransformableViewsAdmin.Admin.UIPages
{
    internal class TransformableViewApplicationPage : ApplicationPage
    {
        public const string IDENTIFIER = "HBS.Xperience.TransformableViews";
    }
}
