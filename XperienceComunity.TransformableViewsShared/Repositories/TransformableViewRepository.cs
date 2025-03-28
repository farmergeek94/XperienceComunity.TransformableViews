using CMS.ContentEngine;
using CMS.Core;
using CMS.DataEngine;
using CMS.DataProviderSQL;
using CMS.Helpers;
using HBS.Xperience.TransformableViewsShared.Services;
using Kentico.Content.Web.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using System.Data;
using System.Threading;

namespace HBS.Xperience.TransformableViewsShared.Repositories
{
    internal class TransformableViewRepository : ITransformableViewRepository
    {
        public Dictionary<string, DateTime> LastViewedDates { get; set; } = [];
        public Dictionary<string, DateTime> LastModified { get; set; } = [];

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly IProgressiveCache _progressiveCache;
        private readonly IContentQueryExecutor _executor;

        public TransformableViewRepository(
        IProgressiveCache progressiveCache,
        IContentQueryExecutor executor
)
        {
            _progressiveCache = progressiveCache;
            _executor = executor;
        }

        public async Task<IEnumerable<SelectListItem>> GetTransformableViewSelectItems(string contentType, string language)
        {
            var names = await GetTransformableViews(language);
            return names.Where(x => x is TransformableDatabaseContentView item && item.TransformableDatabaseViewContentType.Any(c=>c == contentType)).Select(x=>new SelectListItem(x.TransformableDatabaseViewDisplayName, x.TransformableDatabaseViewCodeName));
        }

        public async Task<IEnumerable<SelectListItem>> GetTransformableViewObjectSelectItems(string className, string language)
        {
            var names = await GetTransformableViews(language);
            return names.Where(x=>x is TransformableDatabaseClassView item && item.TransformableDatabaseViewClasses.Any(c=> c == className)).Select(x => new SelectListItem(x.TransformableDatabaseViewDisplayName, x.TransformableDatabaseViewCodeName));
        }

        public async Task<IEnumerable<IHBSTransformableDatabaseView>> GetTransformableViews(string language)
        {
            // using a _semaphore to resolve deadlock issues
            await _semaphore.WaitAsync();
            try
            {
                var views = await _progressiveCache.LoadAsync(async (cs) =>
                {
                    if (cs.Cached)
                    {
                        cs.CacheDependency = CacheHelper.GetCacheDependency(CacheService._defaultKeys);
                    }
                    var builder = new ContentItemQueryBuilder()
                        .ForContentTypes(cs => cs
                            .OfReusableSchema(IHBSTransformableDatabaseView.REUSABLE_FIELD_SCHEMA_NAME)
                            .WithContentTypeFields()
                    );
                    if(!string.IsNullOrEmpty(language))
                    {
                        builder.InLanguage(language);
                    }

                    // Executes the configured query
                    using var dataConnection = ConnectionHelper.GetConnection();
                    IEnumerable<IHBSTransformableDatabaseView> views = [.. (await _executor.GetMappedResult<IHBSTransformableDatabaseView>(builder))];

                    // Set the last modified in order to update the views according as they are changed. 
                    LastModified = views.ToDictionary(x => x.TransformableDatabaseViewCodeName, x => DateTime.Now);
                    return views;
                }, new CacheSettings(86400 * 365, "GetTransformableDatabaseViews", language));

                return views;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<IHBSTransformableDatabaseView?> GetTransformableViews(int id, string language) => (await GetTransformableViews(language)).Where(x => (x is IContentItemFieldsSource item) && item.SystemFields.ContentItemID == id).FirstOrDefault();

        public async Task<IHBSTransformableDatabaseView?> GetTransformableViews(Guid id, string language) => (await GetTransformableViews(language)).Where(x => (x is IContentItemFieldsSource item) && item.SystemFields.ContentItemGUID == id).FirstOrDefault();

        public async Task<IHBSTransformableDatabaseView?> GetTransformableViews(string viewName, string language, bool update = false)
        {
            var views = await GetTransformableViews(language);
            var view = views.Where(x => x.TransformableDatabaseViewCodeName == viewName).FirstOrDefault();
            if (view != null && update)
            {
                LastViewedDates[viewName] = DateTime.Now;
            }

            return view;
        }
    }
}
