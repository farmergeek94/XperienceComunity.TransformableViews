using CMS.ContentEngine;
using HBS.Xperience.TransformableViewsShared.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HBS.Xperience.TransformableViewsShared.Repositories
{
    public interface ITransformableViewRepository
    {
        Dictionary<string, DateTime> LastViewedDates { get; set; }

        Task<IEnumerable<SelectListItem>> GetTransformableViewObjectSelectItems(string className);
        Task<IEnumerable<IHBSTransformableDatabaseView>> GetTransformableViews();
        Task<IHBSTransformableDatabaseView?> GetTransformableViews(int id);
        Task<IHBSTransformableDatabaseView?> GetTransformableViews(Guid id);
        Task<IHBSTransformableDatabaseView?> GetTransformableViews(string viewName, bool update = false);
        Task<IEnumerable<SelectListItem>> GetTransformableViewSelectItems(string contentType);
    }
}