using CMS.Helpers;
using HBS.TransformableViews_Experience;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBS.Xperience.TransformableViewsShared.Services
{
    internal class CacheService : ICacheService
    {
        public CacheService() { 
        }
        List<string> _keys = new List<string>()
        {
            $"{TransformableViewInfo.OBJECT_TYPE}|all"
        };
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
            return CacheHelper.GetCacheDependency([key, $"{TransformableViewInfo.OBJECT_TYPE}|all"]);
        }
        public CMSCacheDependency GetCacheDependencies(IEnumerable<string> keys)
        {
            _keys.AddRange(keys);
            var list = keys.ToList();
            list.Add($"{TransformableViewInfo.OBJECT_TYPE}|all");
            return CacheHelper.GetCacheDependency(list.ToArray());
        }
    }
}
