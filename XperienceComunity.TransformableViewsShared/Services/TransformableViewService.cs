using CMS.ContentEngine;
using CMS.ContentEngine.Internal;
using CMS.DataEngine;
using CMS.Membership;
using HBS;
using HBS.Xperience.TransformableViewsShared.Repositories;
using Kentico.Content.Web.Mvc.Routing;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XperienceComunity.TransformableViewsShared.Services
{
    internal class TransformableViewService(
        ITransformableViewRepository transformableViewRepository,
        IWebHostEnvironment webHostEnvironment,
        IContentItemManagerFactory contentItemManagerFactory,
        IUserInfoProvider userInfoProvider,
        IViewSettingsService viewSettingsService
        ) : ITransformableViewService
    {

        private readonly IContentItemManager _contentItemManager = contentItemManagerFactory.Create(userInfoProvider.Get("administrator").UserID);

        public bool DeleteViewsOnImport => viewSettingsService.DeleteViewsOnImport;

        public async Task<bool> ExportViews(string language, int id = 0)
        {
            var views = id == 0 ? await transformableViewRepository.GetTransformableViews(language) : [await transformableViewRepository.GetTransformableViews(id, language)];

            var contentPath = webHostEnvironment.ContentRootPath;

            foreach (var view in views)
            {
                if (view == null)
                    continue;

                var folderName = GetViewTypeString(view);

                var folderPath = CMS.IO.Path.Combine(contentPath, "Views", "TransformableViews", folderName);

                Directory.CreateDirectory(folderPath);

                var filePath = CMS.IO.Path.Combine(folderPath, $"{view.TransformableDatabaseViewCodeName}_lang_{language}.cshtml");
                var file = new FileInfo(filePath);
                using var writer = file.CreateText();
                writer.Write(view.TransformableDatabaseViewEditor);
            }

            var importsFile = new FileInfo(CMS.IO.Path.Combine(contentPath, "Views", "TransformableViews", "_ViewImports.cshtml"));
            using var importWriter = importsFile.CreateText();
            importWriter.Write("@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers\r\n@using Kentico.PageBuilder.Web.Mvc\r\n@using Kentico.Web.Mvc");

            return true;
        }

        public async Task<bool> ImportViews(string language, int id = 0)
        {
            return id == 0 ? await ImportViewInternal(language) : await ImportSingleViewInternal(id, language);
        }

        public async Task<bool> ImportViewInternal(string language)
        {
            var contentPath = webHostEnvironment.ContentRootPath;

            var folderPath = CMS.IO.Path.Combine(contentPath, "Views", "TransformableViews");

            var files = Directory.EnumerateFiles(folderPath, "*.cshtml", SearchOption.AllDirectories);

            var views = await transformableViewRepository.GetTransformableViews(language);

            foreach (var filePath in files)
            {
                if (!filePath.Contains("_lang_" + language))
                    continue;
                var file = new FileInfo(filePath);
                var viewName = file.Name.Replace(file.Extension, "").Split("_lang_")[0];
                var delete = false;
                using (var reader = file.OpenText())
                {
                    var viewText = await reader.ReadToEndAsync();
                    var view = views.Where(x => x.TransformableDatabaseViewCodeName == viewName).FirstOrDefault();
                    view.TransformableDatabaseViewEditor = viewText;
                    await UpdateTransformableView(view, language);
                    delete = true;
                }
                if (delete && DeleteViewsOnImport)
                {
                    file.Delete();
                }
            }
            return true;
        }

        public async Task<bool> ImportSingleViewInternal(int id, string language)
        {
            var view = await transformableViewRepository.GetTransformableViews(id, language);

            if (view == null)
            {
                return false;
            }

            var contentPath = webHostEnvironment.ContentRootPath;

            var folderName = GetViewTypeString(view);

            var filePath = CMS.IO.Path.Combine(contentPath, "Views", "TransformableViews", folderName, $"{view.TransformableDatabaseViewCodeName}_lang_{language}.cshtml");

            if (File.Exists(filePath))
            {
                using var tr = new CMSTransactionScope();
                var file = new FileInfo(filePath);
                var viewName = file.Name.Replace(file.Extension, "");
                var delete = false;
                using (var reader = file.OpenText())
                {
                    var viewText = await reader.ReadToEndAsync();
                    view.TransformableDatabaseViewEditor = viewText;
                    if(!await UpdateTransformableView(view, language))
                    {
                        return false;
                    }

                    delete = true;
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

        public async Task<bool> InsertTransformableView(string displayName, string workspaceName, IHBSTransformableDatabaseView view, string language)
        {
            var languageName = language;

            var viewData = new Dictionary<string, object>
                    {
                        { "TransformableDatabaseViewCodeName", view.TransformableDatabaseViewCodeName },
                        { "TransformableDatabaseViewDisplayName", view.TransformableDatabaseViewDisplayName },
                        { "TransformableDatabaseViewEditor", view.TransformableDatabaseViewEditor },
                    };
            var extraData = GetViewTypeData(view);
            if (extraData != null)
            {
                viewData.Add(extraData.Item1, extraData.Item2);
            }

            // Creates a content item metadata object
            CreateContentItemParameters createParams = new(
                GetViewContentTypeString(GetViewTypeString(view)),
                null,
                displayName,
                languageName,
                workspaceName);

            var id = await _contentItemManager.Create(createParams, new ContentItemData(viewData));

           return await _contentItemManager.TryPublish(id, languageName);
        }

        public async Task<bool> UpdateTransformableView(IHBSTransformableDatabaseView view, string language)
        {
            var languageName = language;

            var viewData = new Dictionary<string, object>
                    {
                        { "TransformableDatabaseViewCodeName", view.TransformableDatabaseViewCodeName },
                        { "TransformableDatabaseViewDisplayName", view.TransformableDatabaseViewDisplayName },
                        { "TransformableDatabaseViewEditor", view.TransformableDatabaseViewEditor },
                    };
            var extraData = GetViewTypeData(view);
            if (extraData != null)
            {
                viewData.Add(extraData.Item1, extraData.Item2);
            }


            if (view is IContentItemFieldsSource contentFields)
            {
                var result = await _contentItemManager.TryCreateDraft(contentFields.SystemFields.ContentItemID,
                                        languageName);
                if (!result) return result;

                result = await _contentItemManager.TryUpdateDraft(contentFields.SystemFields.ContentItemID,
                                        languageName,
                                        new ContentItemData(viewData));
                if (!result) return result;

                 result = await _contentItemManager.TryPublish(contentFields.SystemFields.ContentItemID, languageName);
                return result;
            }
            return false;
        }

        public string GetViewTypeString(IHBSTransformableDatabaseView view)
        {
            var folderName = "Shared";
            if (view is TransformableDatabaseLayoutView)
                folderName = "Layouts";
            else if (view is TransformableDatabaseClassView)
                folderName = "Classes";
            else if (view is TransformableDatabaseContentView)
                folderName = "Content";
            else if (view is TransformableDatabasePageView)
                folderName = "Pages";
            return folderName;
        }

        private Tuple<string, object>? GetViewTypeData(IHBSTransformableDatabaseView view)
        {
            if (view is TransformableDatabaseClassView item)
                return new("TransformableDatabaseViewClasses", item.TransformableDatabaseViewClasses);
            else if (view is TransformableDatabaseContentView item2)
                return new("TransformableDatabaseViewContentType", item2.TransformableDatabaseViewContentType);
            else if (view is TransformableDatabasePageView item3)
                return new("TransformableDatabaseViewPageType", item3.TransformableDatabaseViewPageType);
            return null;
        }

        private string GetViewContentTypeString(string folderName)
        {
            return folderName switch
            {
                "Layouts" => TransformableDatabaseLayoutView.CONTENT_TYPE_NAME,
                "Classes" => TransformableDatabaseClassView.CONTENT_TYPE_NAME,
                "Content" => TransformableDatabaseContentView.CONTENT_TYPE_NAME,
                "Pages" => TransformableDatabasePageView.CONTENT_TYPE_NAME,
                _ => "Shared"
            };
        }
    }
}
