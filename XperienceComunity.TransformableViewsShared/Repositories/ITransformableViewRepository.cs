using HBS.TransformableViews;
using HBS.TransformableViews_Experience;
using HBS.Xperience.TransformableViewsShared.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HBS.Xperience.TransformableViewsShared.Repositories
{
    public interface ITransformableViewRepository
    {
        Dictionary<string, DateTime> LastViewedDates { get; set; }
        bool DeleteViewsOnImport { get; }

        Task<bool> ExportViews(int id = 0);
        ITransformableViewItem? GetTransformableView(string viewName, bool update = false);
        Task<IEnumerable<SelectListItem>> GetTransformableViewObjectSelectItems(string className);
        Task<IEnumerable<SelectListItem>> GetTransformableViewSelectItems();
        Task<bool> ImportSingleViewInternal(int id);
        Task<bool> ImportViews(int id = 0);
        Task<bool> ImportViewInternal();
        Task<IEnumerable<TransformableViewInfo>> TransformableViews();
    }
}