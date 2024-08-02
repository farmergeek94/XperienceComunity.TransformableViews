using CMS.Base;
using CMS.DataEngine;
using CMS.Helpers;
using HBS.TransformableViews;
using HBS.TransformableViews_Experience;
using HBS.Xperience.TransformableViewsShared.Library;
using HBS.Xperience.TransformableViewsShared.Models;
using HBS.Xperience.TransformableViewsShared.Services;
using Kentico.Xperience.Admin.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using XperienceComunity.TransformableViewsShared.Services;

namespace HBS.Xperience.TransformableViewsShared.Repositories
{
    public class TransformableViewRepository : ITransformableViewRepository
    {
        private readonly IProgressiveCache _progressiveCache;
        private readonly ITransformableViewInfoProvider _transformableViewInfoProvider;
        private readonly IEncryptionService _encryptionService;
        private readonly IViewSettingsService _viewSettingsService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TransformableViewRepository(IProgressiveCache progressiveCache, ITransformableViewInfoProvider transformableViewInfoProvider, IEncryptionService encryptionService, IViewSettingsService viewSettingsService, IWebHostEnvironment webHostEnvironment)
        {
            _progressiveCache = progressiveCache;
            _transformableViewInfoProvider = transformableViewInfoProvider;
            _encryptionService = encryptionService;
            _viewSettingsService = viewSettingsService;
            _webHostEnvironment = webHostEnvironment;
        }

        public Dictionary<string, DateTime> LastViewedDates { get; set; } = new Dictionary<string, DateTime>();
        public bool DeleteViewsOnImport => _viewSettingsService.DeleteViewsOnImport;

        public ITransformableViewItem? GetTransformableView(string viewName, bool update = false)
        {
            var view = _progressiveCache.Load(cs =>
            {                
                var view = _transformableViewInfoProvider.Get().Where(w => w.WhereEquals(nameof(TransformableViewInfo.TransformableViewName), viewName)).ToArray().FirstOrDefault();

                if (cs.Cached)
                {
                    cs.CacheDependency = CacheHelper.GetCacheDependency(new string[] {
                        view == null ? $"{TransformableViewInfo.OBJECT_TYPE}|all" : $"{TransformableViewInfo.OBJECT_TYPE}|byid|{view?.TransformableViewID}"
                    });
                }
                return view == null ? null : _encryptionService.DecryptView(view);
            }, new CacheSettings(86400, "GetTransformableViewInfo", viewName));

            if (view != null && update)
            {
                LastViewedDates[viewName] = DateTime.Now;
            }

            return view;
        }

        public async Task<IEnumerable<SelectListItem>> GetTransformableViewSelectItems()
        {
            var names = await TransformableViewNames();
            return names.Where(x => x.TransformableViewTypeEnum == TransformableViewTypeEnum.Transformable).Select(x=>new SelectListItem(x.TransformableViewDisplayName, x.TransformableViewName));
        }

        public async Task<IEnumerable<SelectListItem>> GetTransformableViewObjectSelectItems(string className)
        {
            var names = await TransformableViewNames();
            return names.Where(x=>x.TransformableViewTypeEnum == TransformableViewTypeEnum.Listing && x.TransformableViewClassName == className).Select(x => new SelectListItem(x.TransformableViewDisplayName, x.TransformableViewName));
        }

        private async Task<IEnumerable<TransformableViewInfo>> TransformableViewNames()
        {
            return await _progressiveCache.LoadAsync(async (cs) =>
            {
                if (cs.Cached)
                {
                    cs.CacheDependency = CacheHelper.GetCacheDependency([
                            $"{TransformableViewInfo.OBJECT_TYPE}|all"
                        ]);
                }
                return await _transformableViewInfoProvider.Get()
                .Columns(nameof(TransformableViewInfo.TransformableViewName), nameof(TransformableViewInfo.TransformableViewDisplayName), nameof(TransformableViewInfo.TransformableViewType), nameof(TransformableViewInfo.TransformableViewClassName)).GetEnumerableTypedResultAsync();
            }, new CacheSettings(86400 * 365, "GetTransformableViewInfoNames"));
        }

        public async Task<IEnumerable<TransformableViewInfo>> TransformableViews()
        {
            var views = await _progressiveCache.LoadAsync(async (cs) =>
            {
                if (cs.Cached)
                {
                    cs.CacheDependency = CacheHelper.GetCacheDependency([
                            $"{TransformableViewInfo.OBJECT_TYPE}|all"
                        ]);
                }
                return await _transformableViewInfoProvider.Get().GetEnumerableTypedResultAsync();
            }, new CacheSettings(86400 * 365, "GetTransformableViewInfos"));

            foreach(var view in views)
            {
                _encryptionService.DecryptView(view);
            }

            return views;
        }



        public async Task<bool> ExportViews(int id = 0)
        {
            var views = id == 0 ? await TransformableViews() : [await _transformableViewInfoProvider.GetAsync(id)];

            var groups = views.GroupBy(x => x.TransformableViewTypeEnum);

            var contentPath = _webHostEnvironment.ContentRootPath;

            foreach (var group in groups)
            {
                var folderPath = CMS.IO.Path.Combine(contentPath, "Views", "TransformableViews", group.Key.ToString());

                Directory.CreateDirectory(folderPath);

                foreach (var view in group)
                {
                    var filePath = CMS.IO.Path.Combine(folderPath, view.TransformableViewName + ".cshtml");
                    var file = new FileInfo(filePath);
                    using var writer = file.CreateText();
                    writer.Write(_encryptionService.DecryptString(view.TransformableViewContent));
                }
            }

            var importsFile = new FileInfo(CMS.IO.Path.Combine(contentPath, "Views", "TransformableViews", "_ViewImports.cshtml"));
            using var importWriter = importsFile.CreateText();
            importWriter.Write("@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers\r\n@using Kentico.PageBuilder.Web.Mvc\r\n@using Kentico.Web.Mvc");

            return true;
        }

        public async Task<bool> ImportViews(int id = 0)
        {
            return id == 0 ? await ImportViewInternal() : await ImportSingleViewInternal(id);
        }

        public async Task<bool> ImportViewInternal()
        {
            var contentPath = _webHostEnvironment.ContentRootPath;

            var folderPath = CMS.IO.Path.Combine(contentPath, "Views", "TransformableViews");

            var files = Directory.EnumerateFiles(folderPath, "*.cshtml", SearchOption.AllDirectories);

            var views = await TransformableViews();

            var viewsToUpdate = new List<TransformableViewInfo>();

            using var tr = new CMSTransactionScope();
            foreach (var filePath in files)
            {
                var file = new FileInfo(filePath);
                var viewName = file.Name.Replace(file.Extension, "");
                var delete = false;
                using (var reader = file.OpenText())
                {
                    var viewText = await reader.ReadToEndAsync();
                    var view = views.Where(x => x.TransformableViewName == viewName).FirstOrDefault();
                    if (view != null)
                    {
                        view.TransformableViewContent = viewText;
                        _transformableViewInfoProvider.Set(view);
                        delete = true;
                    }
                }
                if (delete && DeleteViewsOnImport)
                {
                    file.Delete();
                }
            }
            tr.Commit();

            return true;
        }

        public async Task<bool> ImportSingleViewInternal(int id)
        {
            var view = await _transformableViewInfoProvider.GetAsync(id);

            var contentPath = _webHostEnvironment.ContentRootPath;

            var filePath = CMS.IO.Path.Combine(contentPath, "Views", "TransformableViews", view.TransformableViewTypeEnum.ToString(), view.TransformableViewName + ".cshtml");

            if (File.Exists(filePath))
            {
                using var tr = new CMSTransactionScope();
                var file = new FileInfo(filePath);
                var viewName = file.Name.Replace(file.Extension, "");
                var delete = false;
                using (var reader = file.OpenText())
                {
                    var viewText = await reader.ReadToEndAsync();
                    if (view != null)
                    {
                        view.TransformableViewContent = viewText;
                        _transformableViewInfoProvider.Set(view);
                        delete = true;
                    }
                }
                if (delete && DeleteViewsOnImport)
                {
                    file.Delete();
                }
                tr.Commit();
                return true;
            }

            return false;
        }


    }
}
