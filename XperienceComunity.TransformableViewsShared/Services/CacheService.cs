using CMS.Core;
using CMS.Helpers;
using Kentico.Content.Web.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBS.Xperience.TransformableViewsShared.Services
{
    internal class CacheService : ICacheService
    {
        public static List<string> _defaultKeys = [
            $"contentitem|bycontenttype|{TransformableDatabaseLayoutView.CONTENT_TYPE_NAME}",
            $"contentitem|bycontenttype|{TransformableDatabasePageView.CONTENT_TYPE_NAME}",
            $"contentitem|bycontenttype|{TransformableDatabaseClassView.CONTENT_TYPE_NAME}",
            $"contentitem|bycontenttype|{TransformableDatabaseContentView.CONTENT_TYPE_NAME}"
        ];

        List<string> _keys => [.. _defaultKeys];

        string? _language = null;

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

        public string? GetCachedLanguage()
        {
            if (_language != null)
            {
                return _language;
            }
            var languageProvider = Service.Resolve<IPreferredLanguageRetriever>();
            try
            {
                return _language = languageProvider.Get();
            }
            catch
            {
                return null;
            }
        }
    }
}
