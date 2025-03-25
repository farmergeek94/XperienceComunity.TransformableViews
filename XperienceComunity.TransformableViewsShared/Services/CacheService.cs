using CMS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBS.Xperience.TransformableViewsShared.Services
{
    internal class CacheService : ICacheService
    {
        private List<string> _defaultKeys = [
            $"{TransformableDatabaseLayoutView.CONTENT_TYPE_NAME}|all",
            $"{TransformableDatabasePageView.CONTENT_TYPE_NAME}|all",
            $"{TransformableDatabaseClassView.CONTENT_TYPE_NAME}|all",
            $"{TransformableDatabaseContentView.CONTENT_TYPE_NAME}|all"
        ];

        List<string> _keys => [.. _defaultKeys];

        public List<string> GetDefaultKeys() => _defaultKeys;

        public ICacheService Add(IEnumerable<string> keys)
        {
            _keys.AddRange(keys);
            return this;
        }

        public string[] GetDependenciesList()
        {
            return _keys.ToArray();
        }

        public CMSCacheDependency GetCacheDependencies(string key)
        {
            _keys.Add(key);
            return CacheHelper.GetCacheDependency([key, .. _defaultKeys]);
        }
        public CMSCacheDependency GetCacheDependencies(IEnumerable<string> keys)
        {
            _keys.AddRange(keys);
            var list = keys.ToList();
            list.AddRange(_defaultKeys);
            return CacheHelper.GetCacheDependency(list.ToArray());
        }
    }
}
