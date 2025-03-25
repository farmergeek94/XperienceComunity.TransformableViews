using CMS.Helpers;

namespace HBS.Xperience.TransformableViewsShared.Services
{
    public interface ICacheService
    {
        ICacheService Add(IEnumerable<string> keys);
        List<string> GetDefaultKeys();
        CMSCacheDependency GetCacheDependencies(IEnumerable<string> keys);
        CMSCacheDependency GetCacheDependencies(string key);
        string[] GetDependenciesList();
    }
}