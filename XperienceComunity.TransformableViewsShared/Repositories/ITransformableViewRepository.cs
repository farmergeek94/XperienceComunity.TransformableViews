using HBS.TransformableViews;
using HBS.TransformableViews_Experience;
using HBS.Xperience.TransformableViewsShared.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HBS.Xperience.TransformableViewsShared.Repositories
{
    public interface ITransformableViewRepository
    {
        Dictionary<string, DateTime> LastViewedDates { get; set; }
        ITransformableViewItem? GetTransformableView(string viewName, bool update = false);
        Task<IEnumerable<SelectListItem>> GetTransformableViewObjectSelectItems(string className);
        Task<IEnumerable<SelectListItem>> GetTransformableViewSelectItems();
        Task<IEnumerable<TransformableViewInfo>> TransformableViews();
    }
}