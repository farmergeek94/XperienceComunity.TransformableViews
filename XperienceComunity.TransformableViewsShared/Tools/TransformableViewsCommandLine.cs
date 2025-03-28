using CMS.ContentEngine;
using CMS.ContentEngine.Internal;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Websites;
using HBS;
using HBS.Xperience.TransformableViewsShared.Repositories;
using Kentico.Forms.Web.Mvc;
using System.Collections;
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
        IViewSettingsService viewSettingsService,
        IReusableFieldSchemaManager reusableFieldSchemaManager,
        IValidationRuleConfigurationsXmlSerializer validationRuleConfigurationsXmlSerializer) : ITransformableViewsCommandLine
    {
        public async Task Install()
        {
            // Create the reusable schema for the transformable views
            var reusableScheme = reusableFieldSchemaManager.Get(IHBSTransformableDatabaseView.REUSABLE_FIELD_SCHEMA_NAME);
            if (reusableScheme == null)
            {
                var newSchema = new CreateReusableFieldSchemaParameters(IHBSTransformableDatabaseView.REUSABLE_FIELD_SCHEMA_NAME, "TransformableDatabaseView", "Reusable schema for loading a view");
                var guid = reusableFieldSchemaManager.CreateSchema(newSchema);
                reusableScheme = reusableFieldSchemaManager.Get(guid);
            }

            // Create the fields for the reusable schema
            var reusableSchemaFields = reusableFieldSchemaManager.GetSchemaFields(IHBSTransformableDatabaseView.REUSABLE_FIELD_SCHEMA_NAME);
            foreach (var field in GetReusableFormFields(reusableScheme.Guid))
            {
                var reusableField = reusableSchemaFields.FirstOrDefault(x => x.Name == field.Name);
                if (reusableField == null)
                {
                    reusableFieldSchemaManager.AddField(IHBSTransformableDatabaseView.REUSABLE_FIELD_SCHEMA_NAME, field);
                }
                else
                {
                    reusableFieldSchemaManager.UpdateField(IHBSTransformableDatabaseView.REUSABLE_FIELD_SCHEMA_NAME, field.Name, field);
                }
            }



            // Create the data class for the transformable layout views
            var info = DataClassInfoProvider.GetDataClassInfo(TransformableDatabaseLayoutView.CONTENT_TYPE_NAME) ?? DataClassInfo.New(TransformableDatabaseLayoutView.CONTENT_TYPE_NAME);
            var formInfo = AddResusableSchema(GetBaseFormInfo(), reusableScheme.Guid);
            info.ClassDisplayName = "Transformable Layout View";
            info.ClassName = TransformableDatabaseLayoutView.CONTENT_TYPE_NAME;
            info.ClassFormDefinition = formInfo.GetXmlDefinition();
            info.ClassTableName = "HBS_TransformableDatabaseLayoutView";
            info.ClassGUID = new Guid("F34B2905-AE64-4B18-B5A5-DCEBA740400E");
            info.ClassIconClass = "xp-layouts"; // Assuming null
            info.ClassHasUnmanagedDbSchema = false; // Assuming default value is false
            info.ClassType = "Content";
            info.ClassContentTypeType = "Reusable"; // Assuming null
            info.ClassWebPageHasUrl = false; // Assuming null
            info.ClassShortName = "HBSTransformableDatabaseLayoutView"; // Assuming null

            SetFormDefinition(info, formInfo);

            if (info.HasChanged)
            {
                DataClassInfoProvider.SetDataClassInfo(info);
            }

            // Create the data class for the transformable page views
            info = DataClassInfoProvider.GetDataClassInfo(TransformableDatabasePageView.CONTENT_TYPE_NAME) ?? DataClassInfo.New(TransformableDatabasePageView.CONTENT_TYPE_NAME);
            formInfo = GetPageFormInfo(reusableScheme.Guid);
            info.ClassDisplayName = "Transformable Page View";
            info.ClassName = TransformableDatabasePageView.CONTENT_TYPE_NAME;
            info.ClassFormDefinition = formInfo.GetXmlDefinition();
            info.ClassTableName = "HBS_TransformableDatabasePageView";
            info.ClassGUID = new Guid("79D05C6D-5B99-4C3E-8208-423BB5B09CBC");
            info.ClassIconClass = "xp-doc-plus"; // Assuming null
            info.ClassHasUnmanagedDbSchema = false; // Assuming default value is false
            info.ClassType = "Content";
            info.ClassContentTypeType = "Reusable"; // Assuming null
            info.ClassWebPageHasUrl = false; // Assuming null
            info.ClassShortName = "HBSTransformableDatabaseLayoutView"; // Assuming null

            SetFormDefinition(info, formInfo);

            if (info.HasChanged)
            {
                DataClassInfoProvider.SetDataClassInfo(info);
            }

            // Create the data class for the transformable class views
            info = DataClassInfoProvider.GetDataClassInfo(TransformableDatabaseClassView.CONTENT_TYPE_NAME) ?? DataClassInfo.New(TransformableDatabaseClassView.CONTENT_TYPE_NAME);
            formInfo = GetClassFormInfo(reusableScheme.Guid);
            info.ClassDisplayName = "Transformable Class View";
            info.ClassName = TransformableDatabaseClassView.CONTENT_TYPE_NAME;
            info.ClassFormDefinition = formInfo.GetXmlDefinition();
            info.ClassTableName = "HBS_TransformableDatabaseClassView";
            info.ClassGUID = new Guid("6A5EFCFA-0EAB-40CD-A84B-173A2C277048");
            info.ClassIconClass = "xp-table"; // Assuming null
            info.ClassHasUnmanagedDbSchema = false; // Assuming default value is false
            info.ClassType = "Content";
            info.ClassContentTypeType = "Reusable"; // Assuming null
            info.ClassWebPageHasUrl = false; // Assuming null
            info.ClassShortName = "HBSTransformableDatabaseClassView"; // Assuming null

            SetFormDefinition(info, formInfo);

            if (info.HasChanged)
            {
                DataClassInfoProvider.SetDataClassInfo(info);
            }

            // Create the data class for the transformable content views
            info = DataClassInfoProvider.GetDataClassInfo(TransformableDatabaseContentView.CONTENT_TYPE_NAME) ?? DataClassInfo.New(TransformableDatabaseContentView.CONTENT_TYPE_NAME);
            formInfo = GetContentFormInfo(reusableScheme.Guid);
            info.ClassDisplayName = "Transformable Content View";
            info.ClassName = TransformableDatabaseContentView.CONTENT_TYPE_NAME;
            info.ClassFormDefinition = formInfo.GetXmlDefinition();
            info.ClassTableName = "HBS_TransformableDatabaseContentView";
            info.ClassGUID = new Guid("0465ED65-87E8-42F5-9466-48402491E929");
            info.ClassIconClass = "xp-list-bullets"; // Assuming null
            info.ClassHasUnmanagedDbSchema = false; // Assuming default value is false
            info.ClassType = "Content";
            info.ClassContentTypeType = "Reusable"; // Assuming null
            info.ClassWebPageHasUrl = false; // Assuming null
            info.ClassShortName = "HBSTransformableDatabaseContentView"; // Assuming null

            SetFormDefinition(info, formInfo);

            if (info.HasChanged)
            {
                DataClassInfoProvider.SetDataClassInfo(info);
            }

            Environment.Exit(0);
        }

        /// <summary>
        /// Get the form info for the content views
        /// </summary>
        /// <param name="reusableSchem"></param>
        /// <returns></returns>
        private FormInfo GetContentFormInfo(Guid reusableSchem)
        {
            var formInfo = GetBaseFormInfo();
            var contentTypeField = new FormFieldInfo
            {
                Name = "TransformableDatabaseViewContentType",
                DataType = "objectcodenames",
                AllowEmpty = false,
                Enabled = true,
                Visible = true,
                Properties = new Hashtable
                {
                    { "explanationtextashtml", false },
                    { "fieldcaption", "Content Types" },
                    { "fielddescriptionashtml", false }
                },
                Settings = new Hashtable
                {
                    { "controlname", "HBS.UIFormComponents.ContentTypeSelector" },
                    { "MaximumItems", 0 },
                    { "ObjectType", "cms.class" }
                }
            };
            formInfo.AddFormItem(contentTypeField);
            return AddResusableSchema(formInfo, reusableSchem);
        }

        /// <summary>
        /// Get the form info for the class views
        /// </summary>
        /// <param name="reusableSchem"></param>
        /// <returns></returns>
        private FormInfo GetClassFormInfo(Guid reusableSchem)
        {
            var formInfo = GetBaseFormInfo();
            var classTypeField = new FormFieldInfo
            {
                Name = "TransformableDatabaseViewClasses",
                DataType = "objectcodenames",
                AllowEmpty = false,
                Enabled = true,
                Visible = true,
                Properties = new Hashtable
                {
                    { "explanationtextashtml", false },
                    { "fieldcaption", "Classes" },
                    { "fielddescriptionashtml", false }
                },
                Settings = new Hashtable
                {
                    { "controlname", "HBS.UIFormComponents.ClassSelector" },
                    { "MaximumItems", 0 },
                    { "ObjectType", "cms.class" },
                }
            };
            formInfo.AddFormItem(classTypeField);
            return AddResusableSchema(formInfo, reusableSchem);
        }

        /// <summary>
        /// Get the form info for the page views
        /// </summary>
        /// <param name="reusableSchem"></param>
        /// <returns></returns>
        private FormInfo GetPageFormInfo(Guid reusableSchem)
        {
            var formInfo = GetBaseFormInfo();
            var pageTypeField = new FormFieldInfo
            {
                Name = "TransformableDatabaseViewPageType",
                DataType = "objectcodenames",
                AllowEmpty = false,
                Enabled = true,
                Visible = true,
                Properties = new Hashtable
                {
                    { "explanationtextashtml", false },
                    { "fieldcaption", "Page Types" },
                    { "fielddescriptionashtml", false }
                },
                Settings = new Hashtable
                {
                    { "controlname", "HBS.UIFormComponents.PageTypeSelector" },
                    { "MaximumItems", 0 },
                    { "ObjectType", "cms.class" },
                }
            };
            formInfo.AddFormItem(pageTypeField);
            return AddResusableSchema(formInfo, reusableSchem);
        }

        /// <summary>
        /// Get the base form info for all of the views
        /// </summary>
        /// <returns></returns>
        private FormInfo GetBaseFormInfo()
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
        /// Add the reusable schema to the form info
        /// </summary>
        /// <param name="formInfo"></param>
        /// <param name="reusableSchema"></param>
        /// <returns></returns>
        private FormInfo AddResusableSchema(FormInfo formInfo, Guid reusableSchema)
        {
            var formReusableSchema = new FormSchemaInfo()
            {
                Name = reusableSchema.ToString(),
                Guid = reusableSchema
            };

            formInfo.AddFormItem(formReusableSchema);
            return formInfo;
        }

        /// <summary>
        /// Get the reusable fields for the transformable views
        /// </summary>
        /// <param name="kxp_schema_identifier"></param>
        /// <returns></returns>
        private IEnumerable<FormFieldInfo> GetReusableFormFields(Guid kxp_schema_identifier)
        {
            var fields = new List<FormFieldInfo>();

            var displayNameField = new FormFieldInfo
            {
                Name = "TransformableDatabaseViewDisplayName",
                DataType = "text",
                Size = 200,
                AllowEmpty = true,
                Enabled = true,
                Visible = true,
                Properties = new Hashtable()
                {
                    { "explanationtextashtml", false },
                    { "fieldcaption", "Display Name" },
                    { "fielddescriptionashtml", false },
                    { "kxp_schema_identifier", kxp_schema_identifier }
                },
                Settings = new Hashtable()
                {
                    { "controlname", "Kentico.Administration.TextInput" }
                }
            };
            fields.Add(displayNameField);

            var codeNameField = new FormFieldInfo
            {
                Name = "TransformableDatabaseViewCodeName",
                DataType = "text",
                Size = 200,
                AllowEmpty = true,
                Enabled = true,
                Visible = true,
                Properties = new Hashtable()
                {
                    { "explanationtextashtml", false },
                    { "fieldcaption", "Code Name" },
                    { "fielddescriptionashtml", false },
                    { "kxp_schema_identifier", kxp_schema_identifier }
                },
                Settings = new Hashtable()
                {
                    { "controlname", "Kentico.Administration.CodeName" },
                    { "HasAutomaticCodeNameGenerationOption", false },
                    { "IsCollapsed", false }
                },
                ValidationRuleConfigurationsXmlData = validationRuleConfigurationsXmlSerializer.Serialize([
                    new() {
                        Identifier = "Kentico.Administration.RequiredValue",
                    }
                ])
            };
            fields.Add(codeNameField);

            var viewEditorField = new FormFieldInfo
            {
                Name = "TransformableDatabaseViewEditor",
                DataType = "longtext",
                AllowEmpty = true,
                Enabled = true,
                Visible = true,
                Properties = new Hashtable
                {
                    { "explanationtext", "<div><span style=\"color: #af00db;\">@addTagHelper</span> <span style=\"color: #a31515;\">*, Microsoft.AspNetCore.Mvc.TagHelpers</span></div><div><span style=\"color: #af00db;\">@model</span> <span style=\"color: #267f99;\">HBS</span><span style=\"color: #000000;\">.</span><span style=\"color: #267f99;\">Xperience</span><span style=\"color: #000000;\">.</span><span style=\"color: #267f99;\">TransformableViews</span><span style=\"color: #000000;\">.</span><span style=\"color: #267f99;\">Models</span><span style=\"color: #000000;\">.</span><span style=\"color: #267f99;\">TransformableViewModel</span></div> <div>TransformableViewModel contains the following properties:  <span style=\"color: #0000ff;\">string</span> <span style=\"color: #001080;\">ViewTitle</span>,  <span style=\"color: #0000ff;\">string</span> <span style=\"color: #001080;\">ViewClassNames</span>,  <span style=\"color: #0000ff;\">string</span> <span style=\"color: #001080;\">ViewCustomContent</span>,  <span style=\"color: #800000;\">dynamic</span><span style=\"color: #001080;\">[] Items</span>.<br>  You can reference them like a normal view using <span style=\"color: #af00db;\">@</span><span style=\"color: #001080;\">Model</span><span style=\"color: #000000;\">.</span><span style=\"color: #001080;\">Items</span><br>To reference other transformable views, append <b>TransformableView/</b> to the view's code name e.g. <b>TransformableView/TestPartialView</b>.</div>" },
                    { "explanationtextashtml", true },
                    { "fieldcaption", "View Editor" },
                    { "fielddescriptionashtml", false },
                    { "kxp_schema_identifier", kxp_schema_identifier }
                },
                Settings = new Hashtable
                {
                    { "controlname", "Kentico.Administration.CodeEditor" },
                    { "Language", "html" }
                }
            };
            fields.Add(viewEditorField);

            return fields;
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
                if (language == null) continue;
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

        /// <summary>
        /// Read the json file and load the views into the system
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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

                    if (templates != null)
                    {
                        foreach (var fileName in templates)
                        {
                            var fileText = await File.ReadAllTextAsync(fileName);
                            if (!string.IsNullOrWhiteSpace(fileText))
                            {
                                var item = JsonSerializer.Deserialize<PageTemplateConfigurationItem>(fileText);
                                if (item != null)
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
            foreach (var item in views)
            {
                var viewItem = item.Item1;
                var language = item.Item2;
                var view = await transformableViewRepository.GetTransformableViews(viewItem.TransformableDatabaseViewCodeName, language);

                if (view == null)
                {
                    await transformableViewService.InsertTransformableView(viewItem.TransformableDatabaseViewDisplayName, viewSettingsService.WorkSpaceName, viewItem, language);
                }
                else
                {
                    if (view is IContentItemFieldsSource contentFields)

                        await transformableViewService.UpdateTransformableView(viewItem, language);
                }
            }
        }

        private async Task UpdateInsertPageTemplates(IEnumerable<PageTemplateConfigurationItem> templates)
        {
            foreach (var templateItem in templates)
            {
                var template = (await pageTemplateProvider.Get().WhereEquals(nameof(PageTemplateConfigurationInfo.PageTemplateConfigurationName), templateItem.PageTemplateConfigurationName).TopN(1).GetEnumerableTypedResultAsync()).FirstOrDefault();
                if (template == null)
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