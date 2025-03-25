using CMS.EventLog;
using HBS.Xperience.TransformableViews;
using HBS.Xperience.TransformableViewsAdmin.Admin.UIPages;
using HBS.Xperience.TransformableViewsShared.Repositories;
using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.UIPages;
using Microsoft.AspNetCore.Hosting;
using XperienceComunity.TransformableViewsShared.Services;

[assembly: PageExtender(typeof(TransformableViewPageExtender))]
namespace HBS.Xperience.TransformableViewsAdmin.Admin.UIPages
{
    /// <summary>
    /// Transformable view admin page
    /// </summary>
    internal class TransformableViewPageExtender : PageExtender<ContentHubList>
    {
        private readonly ITransformableViewService _transformableViewService;

        public TransformableViewPageExtender(ITransformableViewRepository transformableViewRepository, ITransformableViewService transformableViewService )
        {
            _transformableViewService = transformableViewService;
        }

        public override Task ConfigurePage()
        {
            Page.PageConfiguration.HeaderActions.AddCommand("Export Data Views", "ExportViews", "xp-arrow-down-line");
            Page.PageConfiguration.HeaderActions.AddCommand("Import Data Views", "ImportViews", "xp-arrow-up-line");

            Page.PageConfiguration.TableActions.AddCommand("Export Data View", "ExportView", "xp-arrow-down-line");
            Page.PageConfiguration.TableActions.AddCommand("Import Data View", "ImportView", "xp-arrow-up-line");
            return base.ConfigurePage();
        }

        // Export a view to the filesystem
        [PageCommand]
        public async Task<ICommandResponse> ExportView(int id)
        {
            _ = await _transformableViewService.ExportViews(id);

            return Response().AddSuccessMessage("View Exported Successfully");
        }


        [PageCommand]
        public async Task<ICommandResponse> ExportViews()
        {
            _ = await _transformableViewService.ExportViews();
            return Response().AddSuccessMessage("Views Exported Successfully");
        }

        [PageCommand]
        public async Task<ICommandResponse> ImportView(int id)
        {
            if(await _transformableViewService.ImportViews(id))
            {
                return Response().AddSuccessMessage("View Imported Successfully");
            }

            return Response().AddErrorMessage("View Not Found on File System");
        }

        [PageCommand]
        public async Task<ICommandResponse> ImportViews()
        {
            _ = await _transformableViewService.ImportViews();

            return Response().AddSuccessMessage("Views Imported Successfully");
        }
    }
}
