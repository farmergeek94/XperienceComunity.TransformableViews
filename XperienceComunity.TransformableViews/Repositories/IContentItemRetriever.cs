using HBS.Xperience.TransformableViewsShared.Models;
using System.Dynamic;

namespace HBS.Xperience.TransformableViews.Repositories
{
    public interface IContentItemRetriever
    {
        /// <summary>
        /// Get the content items and return them as dynamic.
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="selectedContent"></param>
        /// <returns>ExpandoObject[]</returns>
        Task<ExpandoObject[]> GetContentItems(Guid? contentType, IEnumerable<Guid> selectedContent);

        /// <summary>
        /// Get the object items and return them as dynamic
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<dynamic[]> GetObjectItems(TransformableViewObjectsFormComponentModel model);

        // Get the webpage as a dynamic object
        Task<dynamic?> GetWebPage(bool isAuthenticated);
    }
}