using AngleSharp.Dom;
using CMS.ContentEngine;
using CMS.ContentEngine.Internal;
using CMS.DataEngine;
using CMS.Helpers;
using HBS.TransformableViews;
using HBS.TransformableViews_Experience;
using HBS.Xperience.TransformableViewsAdmin.Admin.Models;
using HBS.Xperience.TransformableViewsAdmin.Admin.UIPages;
using HBS.Xperience.TransformableViewsShared.Library;
using HBS.Xperience.TransformableViewsShared.Models;
using HBS.Xperience.TransformableViewsShared.Repositories;
using HBS.Xperience.TransformableViewsShared.Services;
using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.UIPages;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

[assembly: UIPage(typeof(TransformableViewApplicationPage), "hbs-transformable-view-editor", typeof(TransformableViewPage), "View Editor", TransformableViewPage.TemplateName, 0)]
namespace HBS.Xperience.TransformableViewsAdmin.Admin.UIPages
{
    internal class TransformableViewPage : Page<TransformableViewPageClientProperties>
    {
        public const string TemplateName = "@hbs/xperience-transformable-views/TransformableViewPage"; 

        private readonly ITransformableViewInfoProvider _transformableViewInfoProvider;
        private readonly IEncryptionService _encryptionService;
        private readonly ITransformableViewRepository _transformableViewRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TransformableViewPage(ITransformableViewInfoProvider transformableViewInfoProvider, IEncryptionService encryptionService, ITransformableViewRepository transformableViewRepository, IWebHostEnvironment webHostEnvironment)
        {
            _transformableViewInfoProvider = transformableViewInfoProvider;
            _encryptionService = encryptionService;
            _transformableViewRepository = transformableViewRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public override async Task<TransformableViewPageClientProperties> ConfigureTemplateProperties(TransformableViewPageClientProperties properties)
        {
            var provider = TaxonomyInfo.Provider;
            var tagProvider = TagInfo.Provider;
            var taxonomies = await provider.Get().GetEnumerableTypedResultAsync();
            var tags = await tagProvider.Get().GetEnumerableTypedResultAsync();

            properties.Tags = tags.GetCategories();
            properties.Taxonomies = taxonomies.GetCategories();
            return properties;
        }

        [PageCommand]
        public async Task<ICommandResponse> GetViews(int categoryID)
        {
            IEnumerable<ITransformableViewItem> views = await _transformableViewInfoProvider.Get().Where(x => x.WhereEquals(nameof(ITransformableViewItem.TransformableViewTransformableViewTagID), categoryID)).GetEnumerableTypedResultAsync();
            foreach(var view in views)
            {
                view.TransformableViewContent = _encryptionService.DecryptString(view.TransformableViewContent);
            }
            return ResponseFrom(views.DeSerializeForm());
        }

        [PageCommand]
        public async Task<ICommandResponse> SetView(TransformableViewItem model)
        {

            if (model.TransformableViewID != null)
            {
                TransformableViewInfo view = await _transformableViewInfoProvider.GetAsync(model.TransformableViewID.Value);
                if (view != null)
                {
                    view.TransformableViewDisplayName = model.TransformableViewDisplayName;
                    view.TransformableViewContent = model.TransformableViewContent;
                    view.TransformableViewType = model.TransformableViewType;
                    view.TransformableViewClassName = model.TransformableViewClassName;
                    using (CMSTransactionScope tr = new CMSTransactionScope())
                    {
                        _transformableViewInfoProvider.Set(view);
                        tr.Commit();
                    }
                    return ResponseFrom((ITransformableViewItem)view).AddSuccessMessage("View saved");
                }
                return Response().AddErrorMessage("View not found");
            }
            else
            {
                TransformableViewInfo view = new()
                {
                    TransformableViewDisplayName = model.TransformableViewDisplayName,
                    TransformableViewContent = model.TransformableViewContent,
                    TransformableViewName = ValidationHelper.GetCodeName(model.TransformableViewDisplayName),
                    TransformableViewTransformableViewTagID = model.TransformableViewTransformableViewCategoryID,
                    TransformableViewType = model.TransformableViewType,
                    TransformableViewClassName = model.TransformableViewClassName
                };

                using (CMSTransactionScope tr = new CMSTransactionScope())
                {
                    // Add guid if it already exists
                    var exists = await _transformableViewInfoProvider.GetAsync(view.TransformableViewName);
                    if (exists != null) {
                        view.TransformableViewName = view.TransformableViewName + "_" + Guid.NewGuid().ToString();
                    }
                    _transformableViewInfoProvider.Set(view);
                    tr.Commit();
                }
                return ResponseFrom((ITransformableViewItem)view).AddSuccessMessage("View creaeted");
            }
        }

        [PageCommand]
        public async Task<ICommandResponse> GetClassNames(string requestVal)
        {
            var type = (TransformableViewTypeEnum)int.Parse(requestVal);
            dynamic classNames = type switch
            {
                TransformableViewTypeEnum.Transformable => (await DataClassInfoProvider.GetClasses()
                                        .Columns(nameof(DataClassInfo.ClassName), nameof(DataClassInfo.ClassDisplayName))
                                        .WhereEquals(nameof(DataClassInfo.ClassType), "Content").WhereEquals(nameof(DataClassInfo.ClassContentTypeType), "Reusable")
                                        .GetEnumerableTypedResultAsync()).Select(x => new { x.ClassName, ClassDisplayName = $"{x.ClassDisplayName} ({x.ClassName})" }),
                TransformableViewTypeEnum.Page => (await DataClassInfoProvider.GetClasses()
                                        .Columns(nameof(DataClassInfo.ClassName), nameof(DataClassInfo.ClassDisplayName))
                                        .WhereEquals(nameof(DataClassInfo.ClassType), "Content").WhereNotEquals(nameof(DataClassInfo.ClassContentTypeType), "Reusable")
                                        .GetEnumerableTypedResultAsync()).Select(x => new { x.ClassName, ClassDisplayName = $"{x.ClassDisplayName} ({x.ClassName})" }),
                TransformableViewTypeEnum.Listing => (await DataClassInfoProvider.GetClasses()
                                                        .Columns(nameof(DataClassInfo.ClassName), nameof(DataClassInfo.ClassDisplayName))
                                                        .WhereNotEquals(nameof(DataClassInfo.ClassType), "Content")
                                                        .GetEnumerableTypedResultAsync()).Select(x => new { x.ClassName, ClassDisplayName = $"{x.ClassDisplayName} ({x.ClassName})" }),
                TransformableViewTypeEnum.Layout => Enumerable.Empty<string>(),
                _ => Enumerable.Empty<string>(),
            };
            return ResponseFrom(new { classNames });
        }

        [PageCommand]
        public async Task<ICommandResponse> SetViews(IEnumerable<TransformableViewItem> model)
        {
            using var connection = new CMSConnectionScope();
            var ids = model.Where(x=>x.TransformableViewID.HasValue).Select(x => x.TransformableViewID.Value);
            var viewList = new List<ITransformableViewItem>();
            if (ids.Any())
            {
                var views = await _transformableViewInfoProvider.Get().Where(w =>
                    w.WhereIn(nameof(ITransformableViewItem.TransformableViewID), ids.ToArray())
                ).GetEnumerableTypedResultAsync();
                using (CMSTransactionScope tr = new CMSTransactionScope())
                {
                    foreach (var viewModel in model)
                    {
                        var view = views.First(x => x.TransformableViewID == viewModel.TransformableViewID);
                        view.TransformableViewDisplayName = viewModel.TransformableViewDisplayName;
                        view.TransformableViewContent = viewModel.TransformableViewContent;
                        view.TransformableViewType = viewModel.TransformableViewType;
                        view.TransformableViewClassName = viewModel.TransformableViewClassName;
                        _transformableViewInfoProvider.Set(view);
                        viewList.Add(view);
                    }
                    tr.Commit();
                }
            }
            return ResponseFrom(viewList);
        }

        [PageCommand]
        public async Task<ICommandResponse> DeleteView(int viewID)
        {
            var viewInfo = await _transformableViewInfoProvider.GetAsync(viewID);
            _transformableViewInfoProvider.Delete(viewInfo);
            return ResponseFrom(viewID).AddSuccessMessage("Category Deleted Successfully");
        }

        [PageCommand]
        public async Task<ICommandResponse> ExportView(int id)
        {
            var view = await _transformableViewInfoProvider.GetAsync(id);

            var contentPath = _webHostEnvironment.ContentRootPath;

            var folderPath = CMS.IO.Path.Combine(contentPath, "TransformableViews", view.TransformableViewTypeEnum.ToString());

            Directory.CreateDirectory(folderPath);

            var filePath = CMS.IO.Path.Combine(folderPath, view.TransformableViewName + ".cshtml");
            var file = new FileInfo(filePath);
            using var writer = file.CreateText();
            writer.Write(_encryptionService.DecryptString(view.TransformableViewContent));

            var importsFile = new FileInfo(CMS.IO.Path.Combine(contentPath, "TransformableViews", "_ViewImports.cshtml"));
            using var importWriter = importsFile.CreateText();
            importWriter.Write("@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers\r\n@using Kentico.PageBuilder.Web.Mvc\r\n@using Kentico.Web.Mvc");

            return Response().AddSuccessMessage("View Exported Successfully");
        }


        [PageCommand]
        public async Task<ICommandResponse> ExportViews()
        {
            var views = await _transformableViewRepository.TransformableViews();

            var groups = views.GroupBy(x => x.TransformableViewTypeEnum);

            var contentPath = _webHostEnvironment.ContentRootPath;

            foreach (var group in groups)
            {
                var folderPath = CMS.IO.Path.Combine(contentPath, "TransformableViews", group.Key.ToString());

                Directory.CreateDirectory(folderPath);

                foreach(var view in group)
                {
                    var filePath = CMS.IO.Path.Combine(folderPath, view.TransformableViewName + ".cshtml");
                    var file = new FileInfo(filePath);
                    using var writer = file.CreateText();
                    writer.Write(_encryptionService.DecryptString(view.TransformableViewContent));
                }
            }

            var importsFile = new FileInfo(CMS.IO.Path.Combine(contentPath, "TransformableViews", "_ViewImports.cshtml"));
            using var importWriter = importsFile.CreateText();
            importWriter.Write("@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers\r\n@using Kentico.PageBuilder.Web.Mvc\r\n@using Kentico.Web.Mvc");

            return Response().AddSuccessMessage("Views Exported Successfully");
        }

        [PageCommand]
        public async Task<ICommandResponse> ImportView(int id)
        {
            var view = await _transformableViewInfoProvider.GetAsync(id);

            var contentPath = _webHostEnvironment.ContentRootPath;

            var filePath = CMS.IO.Path.Combine(contentPath, "TransformableViews", view.TransformableViewTypeEnum.ToString(), view.TransformableViewName + ".cshtml");

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
                if (delete)
                {
                    file.Delete();
                    }
                tr.Commit();
                return Response().AddSuccessMessage("View Imported Successfully");
            }
            return Response().AddErrorMessage("View Not Found on File System");
        }

        [PageCommand]
        public async Task<ICommandResponse> ImportViews()
        {
            var contentPath = _webHostEnvironment.ContentRootPath;

            var folderPath = CMS.IO.Path.Combine(contentPath, "TransformableViews");

            var files = Directory.EnumerateFiles(folderPath, "*.cshtml", SearchOption.AllDirectories);

            var views = await _transformableViewRepository.TransformableViews();

            var viewsToUpdate = new List<TransformableViewInfo>();

            using var tr = new CMSTransactionScope();
            foreach(var filePath in files)
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
                if (delete)
                {
                    file.Delete();
                }
            }
            tr.Commit();

            return Response().AddSuccessMessage("Views Imported Successfully");
        }
    }

    // Contains properties that match the properties of the client template
    // Specify such classes as the generic parameter of Page<TClientProperties>
    public class TransformableViewPageClientProperties : TemplateClientProperties
    {
        // For example
        public IEnumerable<TransformableViewCategoryItem> Tags { get; set; } = Enumerable.Empty<TransformableViewCategoryItem>();
        // For example
        public IEnumerable<TransformableViewCategoryItem> Taxonomies { get; set; } = Enumerable.Empty<TransformableViewCategoryItem>();
    }

    public static class TransformableViewPageHelper {
        public static IEnumerable<TransformableViewItem> DeSerializeForm(this IEnumerable<ITransformableViewItem> items)
        {
            foreach (var item in items)
            {
                TransformableViewItem nItem = new()
                {
                    TransformableViewContent = item.TransformableViewContent,
                    TransformableViewDisplayName = item.TransformableViewDisplayName,
                    TransformableViewGuid = item.TransformableViewGuid,
                    TransformableViewID = item.TransformableViewID,
                    TransformableViewLastModified = item.TransformableViewLastModified,
                    TransformableViewName = item.TransformableViewName,
                    TransformableViewTransformableViewCategoryID = item.TransformableViewTransformableViewTagID,
                    TransformableViewType = item.TransformableViewType,
                    TransformableViewClassName = item.TransformableViewClassName
                };
                yield return nItem;
            }
        }
        public static IEnumerable<TransformableViewCategoryItem> GetCategories(this IEnumerable<TaxonomyInfo> infos)
        {
            foreach (var item in infos)
            {
                TransformableViewCategoryItem nItem = new()
                {
                    TransformableViewCategoryID = item.TaxonomyID,
                    TransformableViewCategoryName = item.TaxonomyName,
                    TransformableViewCategoryTitle = item.TaxonomyTitle
                };
                yield return nItem;
            }
        }
        public static IEnumerable<TransformableViewCategoryItem> GetCategories(this IEnumerable<TagInfo> infos)
        {
            foreach (var item in infos)
            {
                TransformableViewCategoryItem nItem = new()
                {
                    TransformableViewCategoryID = item.TagID,
                    TransformableViewCategoryName = item.TagName,
                    TransformableViewCategoryTitle = item.TagTitle,
                    TransformableViewCategoryOrder = item.TagOrder,
                    TransformableViewCategoryParentID = item.TagParentID,
                    TransformableViewCategoryRootID = item.TagTaxonomyID
                };
                yield return nItem;
            }
        }
    }
}
