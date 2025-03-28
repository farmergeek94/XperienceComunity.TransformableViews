using CMS.Base;
using CMS.EventLog;
using CMS.Membership;
using HBS.Xperience.TransformableViews;
using HBS.Xperience.TransformableViewsAdmin.Admin.UIPages;
using HBS.Xperience.TransformableViewsShared.Repositories;
using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.UIPages;
using Microsoft.AspNetCore.Hosting;
using System.Data;
using XperienceComunity.TransformableViewsShared.Services;

[assembly: PageExtender(typeof(TransformableViewListPageExtender))]
namespace HBS.Xperience.TransformableViewsAdmin.Admin.UIPages
{
    /// <summary>
    /// Transformable view admin page
    /// </summary>
    internal class TransformableViewListPageExtender : PageExtender<ContentHubList>
    {
        private readonly ITransformableViewService _transformableViewService;

        public TransformableViewListPageExtender(ITransformableViewRepository transformableViewRepository, ITransformableViewService transformableViewService )
        {
            _transformableViewService = transformableViewService;
        }

        public override Task ConfigurePage()
        {
            Page.PageConfiguration.HeaderActions.AddCommand("Export Data Views", "ExportViews", "xp-arrow-down-line");
            Page.PageConfiguration.HeaderActions.AddCommand("Import Data Views", "ImportViews", "xp-arrow-up-line");

            Page.PageConfiguration.TableActions.AddCommand("Export Data View", "ExportView", "xp-arrow-down-line", actionStateEvaluator: HideIfNottransformableView);
            Page.PageConfiguration.TableActions.AddCommand("Import Data View", "ImportView", "xp-arrow-up-line", actionStateEvaluator: HideIfNottransformableView);
            return base.ConfigurePage();
        }

        // Export a view to the filesystem
        [PageCommand]
        public async Task<ICommandResponse> ExportView(int id)
        {
            _ = await _transformableViewService.ExportViews(Page.ContentLanguageIdentifier.LanguageName, id);

            return Response().AddSuccessMessage("View Exported Successfully");
        }


        [PageCommand]
        public async Task<ICommandResponse> ExportViews()
        {
            _ = await _transformableViewService.ExportViews(Page.ContentLanguageIdentifier.LanguageName);
            return Response().AddSuccessMessage("Views Exported Successfully");
        }

        [PageCommand]
        public async Task<ICommandResponse> ImportView(int id)
        {
            if(await _transformableViewService.ImportViews(Page.ContentLanguageIdentifier.LanguageName, id))
            {
                return Response().AddSuccessMessage("View Imported Successfully");
            }

            return Response().AddErrorMessage("View Not Found on File System");
        }

        [PageCommand]
        public async Task<ICommandResponse> ImportViews()
        {
            _ = await _transformableViewService.ImportViews(Page.ContentLanguageIdentifier.LanguageName);

            return Response().AddSuccessMessage("Views Imported Successfully");
        }

        // Disables the delete action for the default global administrator and public users
        private void HideIfNottransformableView(ActionConfiguration actionConfiguration, IDataContainer rowData)
        {
            string[] contentTypes = [
                TransformableDatabaseLayoutView.CONTENT_TYPE_NAME,
                TransformableDatabasePageView.CONTENT_TYPE_NAME,
                TransformableDatabaseClassView.CONTENT_TYPE_NAME,
                TransformableDatabaseContentView.CONTENT_TYPE_NAME
                ];
            if (actionConfiguration.Visible && !contentTypes.Contains(rowData["ClassName"]))
            {
                actionConfiguration.Visible = false;
            }
        }
    }
}
