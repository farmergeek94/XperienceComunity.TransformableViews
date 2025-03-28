using CMS.ContentEngine;
using CMS.ContentEngine.Internal;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Websites;
using HBS;
using HBS.Xperience.TransformableViewsShared.Repositories;
using System.Text.Json;
using XperienceComunity.TransformableViewsShared.Library;
using XperienceComunity.TransformableViewsShared.Models;
using XperienceComunity.TransformableViewsShared.Services;
using static HBS.Xperience.TransformableViewsShared.TransformableViewServices;

namespace XperienceComunity.TransformableViewsTool
{
    internal class TransformableViewsCommandLine(
        IInfoProvider<PageTemplateConfigurationInfo> pageTemplateProvider,
        ITransformableViewRepository transformableViewRepository,
        ITransformableViewService transformableViewService,
        IViewSettingsService viewSettingsService) : ITransformableViewsCommandLine
    {
        public async Task Install()
        {
            var info = DataClassInfoProvider.GetDataClassInfo(TransformableDatabaseLayoutView.CONTENT_TYPE_NAME) ?? DataClassInfo.New(TransformableDatabaseLayoutView.CONTENT_TYPE_NAME);

            info.ClassDisplayName = "Transformable Database View";
            info.ClassName = "HBS.TransformableDatabaseView";
            info.ClassFormDefinition = GetFormInfo().GetXmlDefinition();
            info.ClassTableName = "HBS_TransformableDatabaseView";
            info.ClassGUID = new Guid("79D05C6D-5B99-4C3E-8208-423BB5B09CBC");
            info.ClassIconClass = "xp-layouts"; // Assuming null
            info.ClassHasUnmanagedDbSchema = false; // Assuming default value is false
            info.ClassType = "Content";
            info.ClassContentTypeType = "Reusable"; // Assuming null
            info.ClassWebPageHasUrl = false; // Assuming null
            info.ClassShortName = "HBSTransformableDatabaseView"; // Assuming null

            SetFormDefinition(info, GetFormInfo());

            if (info.HasChanged)
            {
                //await AbstractInfoProvider<DataClassInfo, DataClassInfoProvider, ObjectQuery<DataClassInfo>>.ProviderObject.SetAsync(info);
            }

            Environment.Exit(0);
        }

        private FormInfo GetFormInfo()
        {
            var formInfo = FormHelper.GetBasicFormDefinition("ContentItemDataID");

            var formItem = new FormFieldInfo
            {
                Name = nameof(ContentItemDataInfo.ContentItemDataCommonDataID),
                ReferenceToObjectType = ContentItemCommonDataInfo.OBJECT_TYPE,
                ReferenceType = ObjectDependencyEnum.Required,
                System = true,
                DataType = "integer",
                Enabled = true,
                Visible = false
            };
            formInfo.AddFormItem(formItem);

            formItem = new FormFieldInfo
            {
                Name = nameof(ContentItemDataInfo.ContentItemDataGUID),
                IsUnique = true,
                System = true,
                DataType = "guid",
                Enabled = true,
                Visible = false
            };
            formInfo.AddFormItem(formItem);


            return formInfo;
        }

        /// <summary>
        /// Ensure that the form is upserted with any existing form
        /// </summary>
        /// <param name="info"></param>
        /// <param name="form"></param>
        private static void SetFormDefinition(DataClassInfo info, FormInfo form)
        {
            if (info.ClassID > 0)
            {
                var existingForm = new FormInfo(info.ClassFormDefinition);
                existingForm.CombineWithForm(form, new());
                info.ClassFormDefinition = existingForm.GetXmlDefinition();
            }
            else
            {
                info.ClassFormDefinition = form.GetXmlDefinition();
            }
        }

        public async Task CreateJsonLoadFile(TransformableExport model)
        {
            Console.WriteLine("Beginning export...");
            var tempPath = CMS.IO.Path.Combine(System.IO.Path.GetTempPath(), "TransformableViews_Export");
            var viewsPath = CMS.IO.Path.Combine(tempPath, "Views");

            // Get all views irrespective of language
            var views = await transformableViewRepository.GetTransformableViews(null);
            var languages = await ContentLanguageInfoProvider.ProviderObject.Get().GetEnumerableTypedResultAsync();

            var viewJson = new Dictionary<string, string>();
            foreach (var view in views)
            {
                var type = transformableViewService.GetViewTypeString(view);
                var languageId = view is IContentItemFieldsSource contentFields ? contentFields.SystemFields.ContentItemCommonDataContentLanguageID : 0;
                var language = languages.FirstOrDefault(x => x.ContentLanguageID == languageId);
                if(language == null) continue;
                var filePath = CMS.IO.Path.Combine(viewsPath, type, $"{view.TransformableDatabaseViewCodeName}_lang_{language.ContentLanguageName}.json");
                viewJson.Add(filePath, JsonSerializer.Serialize(view));
            }

            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }

            Directory.CreateDirectory(viewsPath);

            foreach (var item in viewJson)
            {
                await File.WriteAllTextAsync(item.Key, item.Value);
            }

            if (model.PageTemplates)
            {
                var templates = (await pageTemplateProvider.Get().WhereLike(nameof(PageTemplateConfigurationInfo.PageTemplateConfigurationTemplate), "%HBS.TransformableViewPageTemplate%").GetEnumerableTypedResultAsync()).GetTemplateItems();
                if (templates.Any())
                {
                    var templatesPath = CMS.IO.Path.Combine(tempPath, "PageTemplates");

                    Directory.CreateDirectory(templatesPath);

                    foreach (var template in templates)
                    {
                        await File.WriteAllTextAsync(CMS.IO.Path.Combine(templatesPath, $"{template.PageTemplateConfigurationName}.json"), JsonSerializer.Serialize(template));
                    }
                }
            }


            if (File.Exists(model.Export))
            {
                File.Delete(model.Export);
            }

            System.IO.Compression.ZipFile.CreateFromDirectory(tempPath, model.Export);

            Console.WriteLine($"Exported {viewJson.Count} views to {model.Export}");

            Environment.Exit(0);
        }

        public async Task ReadJsonLoadFile(TransformableImport model)
        {
            Console.WriteLine("Beginning import...");

            if (!File.Exists(model.Import))
            {
                Console.WriteLine($"Import package not found at {model.Import}.  Please provide correct path.");
                Environment.Exit(1);
                return;
            }

            var tempPath = CMS.IO.Path.Combine(System.IO.Path.GetTempPath(), "TransformableViews_Export");

            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }

            Directory.CreateDirectory(tempPath);

            System.IO.Compression.ZipFile.ExtractToDirectory(model.Import, tempPath);

            List<Tuple<IHBSTransformableDatabaseView, string>> transformableViews = [];

            transformableViews.AddRange(await ReadJsonFiles(tempPath, "Layouts"));

            transformableViews.AddRange(await ReadJsonFiles(tempPath, "Pages"));

            transformableViews.AddRange(await ReadJsonFiles(tempPath, "Classes"));

            transformableViews.AddRange(await ReadJsonFiles(tempPath, "Content"));

            if (transformableViews == null || transformableViews.Count == 0)
            {
                Console.WriteLine("No views found to import");
                Environment.Exit(1);
                return;
            }

            await UpdateInsertViews(transformableViews);

            var pageTemplateCount = 0;
            if (model.PageTemplates)
            {
                var templatesPath = CMS.IO.Path.Combine(tempPath, "PageTemplates");

                if (Directory.Exists(templatesPath))
                {
                    var templates = Directory.EnumerateFiles(templatesPath);

                    var templateList = new List<PageTemplateConfigurationItem>();

                    if(templates != null)
                    {
                        foreach (var fileName in templates)
                        {
                            var fileText = await File.ReadAllTextAsync(fileName);
                            if (!string.IsNullOrWhiteSpace(fileText))
                            {
                                var item = JsonSerializer.Deserialize<PageTemplateConfigurationItem>(fileText);
                                if(item != null)
                                {
                                    templateList.Add(item);
                                }
                            }
                        }
                    }
                    if (templateList.Count != 0)
                    {
                        pageTemplateCount = templateList.Count;
                        await UpdateInsertPageTemplates(templateList);
                    }
                }
            }
            

            Console.WriteLine($"{transformableViews.Count()} view(s), {pageTemplateCount} page template(s) were imported from {model.Import}.");

            Environment.Exit(0);
        }

        private async Task<IEnumerable<Tuple<IHBSTransformableDatabaseView, string>>> ReadJsonFiles(string tempPath, string type)
        {
            var files = Directory.EnumerateFiles(CMS.IO.Path.Combine(tempPath, "Views", type));

            List<Tuple<IHBSTransformableDatabaseView, string>> transformableViews = [];
            foreach (var file in files)
            {
                var fileText = await File.ReadAllTextAsync(file);
                if (!string.IsNullOrWhiteSpace(fileText))
                {
                    IHBSTransformableDatabaseView? item = type switch
                    {
                        "Layouts" => JsonSerializer.Deserialize<TransformableDatabaseLayoutView>(fileText),
                        "Pages" => JsonSerializer.Deserialize<TransformableDatabasePageView>(fileText),
                        "Classes" => JsonSerializer.Deserialize<TransformableDatabaseClassView>(fileText),
                        "Content" => JsonSerializer.Deserialize<TransformableDatabaseContentView>(fileText),
                        _ => null
                    };
                    if (item != null)
                    {
                        var language = file.Split("_lang_")[1].Split(".json")[0];
                        transformableViews.Add(Tuple.Create(item, language));
                    }
                }
            }
            return transformableViews;
        }

        private async Task UpdateInsertViews(IEnumerable<Tuple<IHBSTransformableDatabaseView, string>> views)
        {
            foreach(var item in views)
            { 
                var viewItem = item.Item1;
                 var view = await transformableViewRepository.GetTransformableViews(viewItem.TransformableDatabaseViewCodeName);

                if (view == null)
                {
                    await transformableViewService.InsertTransformableView(viewItem.TransformableDatabaseViewDisplayName, viewSettingsService.WorkSpaceName, viewItem, item.Item2);
                }
                else
                {
                    if(view is IContentItemFieldsSource contentFields)
                        await transformableViewService.UpdateTransformableView(contentFields.SystemFields.ContentItemID, viewItem.TransformableDatabaseViewEditor, item.Item2);
                }
            }
        }

        private async Task UpdateInsertPageTemplates(IEnumerable<PageTemplateConfigurationItem> templates)
        {
            foreach (var templateItem in templates)
            {
                var template = (await pageTemplateProvider.Get().WhereEquals(nameof(PageTemplateConfigurationInfo.PageTemplateConfigurationName), templateItem.PageTemplateConfigurationName).TopN(1).GetEnumerableTypedResultAsync()).FirstOrDefault();
                if(template == null)
                {
                    template = PageTemplateConfigurationInfo.New();
                    template.PageTemplateConfigurationName = templateItem.PageTemplateConfigurationName;
                }
                template.PageTemplateConfigurationDescription = templateItem.PageTemplateConfigurationDescription;
                template.PageTemplateConfigurationIcon = templateItem.PageTemplateConfigurationIcon;
                template.PageTemplateConfigurationTemplate = templateItem.PageTemplateConfigurationTemplate;
                template.PageTemplateConfigurationWidgets = templateItem.PageTemplateConfigurationWidgets;

                await pageTemplateProvider.SetAsync(template);
            }
        }
    }
}
