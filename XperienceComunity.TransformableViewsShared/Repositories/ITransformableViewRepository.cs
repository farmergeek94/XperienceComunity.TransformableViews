using CMS.ContentEngine;
using HBS.Xperience.TransformableViewsShared.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HBS.Xperience.TransformableViewsShared.Repositories
{
    public interface ITransformableViewRepository
    {
        Dictionary<string, DateTime> LastViewedDates { get; set; }
        Dictionary<string, DateTime> LastModified { get; set; }

        Task<IEnumerable<SelectListItem>> GetTransformableViewObjectSelectItems(string className, string language);
        Task<IHBSTransformableDatabaseView?> GetTransformableViews(int id, string language);
        Task<IHBSTransformableDatabaseView?> GetTransformableViews(Guid id, string language);
        Task<IHBSTransformableDatabaseView?> GetTransformableViews(string viewName, string language, bool update = false);
        Task<IEnumerable<IHBSTransformableDatabaseView>> GetTransformableViews(string language);
        Task<IEnumerable<SelectListItem>> GetTransformableViewSelectItems(string contentType, string language);
    }
}