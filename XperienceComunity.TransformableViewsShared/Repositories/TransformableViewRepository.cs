using CMS.ContentEngine;
using CMS.Helpers;
using HBS.Xperience.TransformableViewsShared.Services;
using Kentico.Content.Web.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using System.Data;

namespace HBS.Xperience.TransformableViewsShared.Repositories
{
    internal class TransformableViewRepository(
    IProgressiveCache progressiveCache,
    IContentQueryExecutor executor,
    IPreferredLanguageRetriever languageRetriever,
    ICacheService cacheService
) : ITransformableViewRepository
    {
        public Dictionary<string, DateTime> LastViewedDates { get; set; } = [];

        public async Task<IEnumerable<SelectListItem>> GetTransformableViewSelectItems(string contentType)
        {
            var names = await GetTransformableViews();
            return names.Where(x => x is TransformableDatabaseContentView item && item.TransformableDatabaseViewContentType.Any(c=>c == contentType)).Select(x=>new SelectListItem(x.TransformableDatabaseViewDisplayName, x.TransformableDatabaseViewCodeName));
        }

        public async Task<IEnumerable<SelectListItem>> GetTransformableViewObjectSelectItems(string className)
        {
            var names = await GetTransformableViews();
            return names.Where(x=>x is TransformableDatabaseClassView item && item.TransformableDatabaseViewClasses.Any(c=> c == className)).Select(x => new SelectListItem(x.TransformableDatabaseViewDisplayName, x.TransformableDatabaseViewCodeName));
        }

        public async Task<IEnumerable<IHBSTransformableDatabaseView>> GetTransformableViews()
        {
            var language = languageRetriever.Get();

            var views = await progressiveCache.LoadAsync(async (cs) =>
            {
                if (cs.Cached)
                {
                    cs.CacheDependency = CacheHelper.GetCacheDependency(cacheService.GetDefaultKeys());
                }
                var builder = new ContentItemQueryBuilder()
                    .ForContentTypes(cs => cs
                        .OfReusableSchema(IHBSTransformableDatabaseView.REUSABLE_FIELD_SCHEMA_NAME)
                        .WithContentTypeFields()
                ).InLanguage(language);
                // Executes the configured query
                IEnumerable<IHBSTransformableDatabaseView> views = await executor.GetMappedResult<IHBSTransformableDatabaseView>(builder);
                return views;
            }, new CacheSettings(86400 * 365, "GetTransformableDatabaseViews", language));

            return views;
        }

        public async Task<IHBSTransformableDatabaseView?> GetTransformableViews(int id) => (await GetTransformableViews()).Where(x => (x is IContentItemFieldsSource item) && item.SystemFields.ContentItemID == id).FirstOrDefault();

        public async Task<IHBSTransformableDatabaseView?> GetTransformableViews(Guid id) => (await GetTransformableViews()).Where(x => (x is IContentItemFieldsSource item) && item.SystemFields.ContentItemGUID == id).FirstOrDefault();

        public async Task<IHBSTransformableDatabaseView?> GetTransformableViews(string viewName, bool update = false)
        {
            var views = await GetTransformableViews();
            var view = views.Where(x => x.TransformableDatabaseViewCodeName == viewName).FirstOrDefault();
            if (view != null && update)
            {
                LastViewedDates[viewName] = DateTime.Now;
            }

            return view;
        }
    }
}
