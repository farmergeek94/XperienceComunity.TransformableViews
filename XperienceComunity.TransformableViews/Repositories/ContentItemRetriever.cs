using CMS.Base;
using CMS.ContentEngine;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.FormEngine.Internal;
using CMS.Helpers;
using CMS.Websites;
using CMS.Websites.Routing;
using Kentico.Content.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HBS.Xperience.TransformableViewsShared.Services;
using HBS.Xperience.TransformableViewsShared.Models;
using System.Data;
using System.Data.Common;

namespace HBS.Xperience.TransformableViews.Repositories
{
    internal class ContentItemRetriever : IContentItemRetriever
    {
        private readonly IWebPageDataContextRetriever _contextRetriever;
        private readonly IContentQueryExecutor _queryExecutor;
        private readonly IWebsiteChannelContext _channelContext;
        private readonly IProgressiveCache _progressiveCache;
        private readonly ICacheService _cacheService;

        public ContentItemRetriever(IWebPageDataContextRetriever contextRetriever, IContentQueryExecutor queryExecutor, IWebsiteChannelContext channelContext, IProgressiveCache progressiveCache, ICacheService cacheService)
        {
            _contextRetriever = contextRetriever;
            _queryExecutor = queryExecutor;
            _channelContext = channelContext;
            _progressiveCache = progressiveCache;
            _cacheService = cacheService;
        }

        /// <summary>
        /// Get the class columns based on class name
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        internal async Task<IEnumerable<string>> GetClassColumnNames(string className)
        {
            var type = await DataClassInfoProvider.ProviderObject.GetAsync(className);
            return await GetClassColumnsNames(type);
        }

        /// <summary>
        /// Get class columns based on DCI
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal async Task<IEnumerable<string>> GetClassColumnsNames(DataClassInfo? type)
        {
            var form = new FormInfo(type.ClassFormDefinition);
            return form.GetColumnNames();
        }

        /// <summary>
        /// Get the current webpage and all fields.  Return a dynamic.
        /// </summary>
        /// <param name="isAuthenticated"></param>
        /// <returns></returns>
        public async Task<dynamic?> GetWebPage(bool isAuthenticated)
        {
            var page = _contextRetriever.Retrieve().WebPage;

            var columnNames = await GetClassColumnNames(page.ContentTypeName);

            var builder = new ContentItemQueryBuilder()
                .ForContentType(page.ContentTypeName,
                options => options
                .ForWebsite(page.WebsiteChannelName)
                .Where(x => x.WhereEquals(nameof(IWebPageContentQueryDataContainer.WebPageItemID), page.WebPageItemID))
                .TopN(1)
                ).InLanguage(page.LanguageName);

            // Configures the query options for the query executor
            var queryOptions = new ContentQueryExecutionOptions()
            {
                ForPreview = _channelContext.IsPreview,
                IncludeSecuredItems = isAuthenticated || _channelContext.IsPreview
            };

            ExpandoObject? result = await _progressiveCache.LoadAsync(async cs =>
            {
                var pageResult = (await _queryExecutor.GetResult(builder, map => GetColumnValues(map, columnNames), queryOptions))?.FirstOrDefault();
                if (pageResult != null)
                {
                    cs.CacheDependency = _cacheService.GetCacheDependencies(CacheHelper.BuildCacheItemName(new[] { "webpageitem",
                                                                       "byid",
                                                                       page?.WebPageItemID.ToString() }));
                }
                return pageResult;
            }, new CacheSettings(30, true, "GetWebPage", page.ContentTypeName, page.WebPageItemID, page.WebsiteChannelID, page.LanguageID));

            return result;
        }

        /// <summary>
        /// Get the current content items with all the fields 
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="selectedContent"></param>
        /// <returns></returns>
        public async Task<ExpandoObject[]> GetContentItems(Guid? contentType, IEnumerable<Guid> selectedContent)
        {
            return await _progressiveCache.LoadAsync(async cs =>
            {
                // Retrive the DataClass in order to get the FormDefinition.  Have to query it from the table because content types are not loaded from the database. 
                var type = (await DataClassInfoProvider.ProviderObject.Get().WhereEquals(nameof(DataClassInfo.ClassGUID), contentType).GetEnumerableTypedResultAsync()).FirstOrDefault();

                // Parse out the column names from the form definition
                var columNames = await GetClassColumnsNames(type);

                // Builds the query - the content type must match the one configured for the selector
                var query = new ContentItemQueryBuilder()
                                .ForContentType(type.ClassName,
                                      config => config
                                        .Where(where =>
                                        where
                                                .WhereIn(nameof(IContentQueryDataContainer.ContentItemGUID), selectedContent.ToList())
                                        ));

                // builds the expando object columns
                ExpandoObject[] items = (await _queryExecutor.GetResult(query, map => GetColumnValues(map, columNames))).ToArray();

                cs.CacheDependency = _cacheService.GetCacheDependencies(CacheHelper.BuildCacheItemName(new[] { "contentitem",
                                                                       "bycontenttype",
                                                                       type.ClassName }));

                return items;
            }, new CacheSettings(30, true, $"GetContentItems|{contentType}|{string.Join('|', selectedContent)}"));
        }

        /// <summary>
        /// Get the expando item columns
        /// </summary>
        /// <param name="map"></param>
        /// <param name="columnNames"></param>
        /// <returns></returns>
        internal ExpandoObject GetColumnValues(IContentQueryDataContainer map, IEnumerable<string> columnNames)
        {
            // Add the inital fields known
            var eOb = new ExpandoObject() as IDictionary<string, object?>;
            eOb.Add(nameof(map.ContentItemID), map.ContentItemID);
            eOb.Add(nameof(map.ContentItemContentTypeID), map.ContentItemContentTypeID);
            eOb.Add(nameof(map.ContentItemGUID), map.ContentItemGUID);
            eOb.Add(nameof(map.ContentItemCommonDataContentLanguageID), map.ContentItemCommonDataContentLanguageID);
            eOb.Add(nameof(map.ContentItemName), map.ContentItemName);
            eOb.Add(nameof(map.ContentTypeName), map.ContentTypeName);
            // go through each of the colums and add them
            foreach (var columnName in columnNames)
            {
                if (eOb.ContainsKey(columnName))
                {
                    continue;
                }
                if (map.TryGetValue(columnName, out dynamic value))
                {
                    // if json parse out the column to a dymanic
                    if (value.GetType() == typeof(string) && (value.IndexOf("[") > -1 || value.IndexOf("{") > -1))
                    {
                        try
                        {
                            dynamic parsed = JsonSerializer.Deserialize<dynamic>(value);
                            eOb.Add(columnName, parsed);
                            continue;
                        }
                        catch
                        {

                        }
                    }
                    eOb.Add(columnName, value);
                }
            }
            return (ExpandoObject)eOb;
        }

        /// <summary>
        /// Get the object items.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic[]> GetObjectItems(TransformableViewObjectsFormComponentModel model)
        {
            var query = new ObjectQuery(model.ClassName);
            if (!string.IsNullOrWhiteSpace(model.Columns))
            {
                query.Columns(model.Columns.Split(","));
            }
            if (!string.IsNullOrWhiteSpace(model.WhereCondition))
            {
                var where = new WhereCondition(model.WhereCondition);
                query.Where(where);
            }
            if (!string.IsNullOrWhiteSpace(model.OrderBy))
            {
                query.OrderBy(model.OrderBy.Split(","));
            }
            if (model.TopN != null)
            {
                query.TopN(model.TopN.Value);
            }

            // Check if it is a registered item.  If it isn't will have to avoid caching for now.
            // To Do allow for caching of pages and content items. 
            var type = ObjectTypeManager.RegisteredTypes.Where(x => x.ObjectClassName.ToLower() == model.ClassName.ToLower()).FirstOrDefault();
            if (type == null)
            {
                return (await GetObjectItemsInternal(query)).ToArray();
            }
            else
            {

                return (await _progressiveCache.LoadAsync(async cs =>
                {
                    var expendables = await GetObjectItemsInternal(query);
                    cs.CacheDependency = _cacheService.GetCacheDependencies(CacheHelper.BuildCacheItemName(new[] { 
                        type.ObjectClassName,
                        "all"
                    }));
                    return expendables;
                }, new CacheSettings(30, true, "GetObjectItems", query.GetFullQueryText()))).ToArray();
            }
        }

        /// <summary>
        /// Get the objects and parse to expando objects.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private async Task<List<dynamic>> GetObjectItemsInternal(ObjectQuery query)
        {
            IEnumerable<IDataContainer> items = await query.GetDataContainerResultAsync(CommandBehavior.Default);

            var columns = items.First().ColumnNames;
            var expendables = new List<dynamic>();
            foreach (var item in items)
            {
                var expando = new ExpandoObject() as IDictionary<string, object>;
                // Loop through each column and 
                foreach (var column in columns)
                {
                    dynamic value = item[column];

                    // if json parse out the column to a dymanic
                    if (value.GetType() == typeof(string) && (value.IndexOf("[") > -1 || value.IndexOf("{") > -1))
                    {
                        try
                        {
                            var parsed = JsonSerializer.Deserialize<dynamic>((string)value);
                            expando.Add(column, parsed);
                        }
                        catch
                        {
                            expando.Add(column, value);
                        }
                    }
                    else
                    {
                        expando.Add(column, value);
                    }
                }
                expendables.Add(expando);
            };
            return expendables;
        }
    }
}
