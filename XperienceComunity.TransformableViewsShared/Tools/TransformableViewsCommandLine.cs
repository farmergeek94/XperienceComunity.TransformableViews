using CMS.Base;
using CMS.ContentEngine;
using CMS.DataEngine;
using CMS.Modules;
using HBS.TransformableViews;
using HBS.TransformableViews_Experience;
using HBS.Xperience.TransformableViewsShared.Models;
using HBS.Xperience.TransformableViewsShared.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using XperienceComunity.TransformableViewsShared.Library;
using XperienceComunity.TransformableViewsShared.Models;

namespace XperienceComunity.TransformableViewsTool
{
    internal class TransformableViewsCommandLine(IInfoProvider<ResourceInfo> resourceInfoProvider, ITransformableViewInfoProvider transformableViewInfoProvider, IEncryptionService encryptionService, IInfoProvider<TagInfo> tagInfoProvider, IInfoProvider<TaxonomyInfo> taxonomyInfoProvider) : ITransformableViewsCommandLine
    {
        public void Install()
        {
            var resource = resourceInfoProvider.Get("HBS.TransformableViews_Experience");
            if (resource == null)
            {
                DataContainer dataContainer = new DataContainer();

                // Set the values for each field
                dataContainer.SetValue("ResourceDisplayName", "Transformable Views");
                dataContainer.SetValue("ResourceName", "HBS.TransformableViews_Experience");
                dataContainer.SetValue("ResourceDescription", null); // Assuming NULL
                dataContainer.SetValue("ResourceGUID", new Guid("6792116b-6b06-4284-bd27-c7249c8ad79f"));
                dataContainer.SetValue("ResourceIsInDevelopment", false); // 0 in SQL is equivalent to false

                resource = ResourceInfo.New(dataContainer);
                ResourceInfoProvider.ProviderObject.Set(resource);
                Console.WriteLine("Installed the Transformable Views Module");
            }

            var classInfo = DataClassInfoProvider.ProviderObject.Get("HBS.TransformableView");
            if (classInfo == null)
            {
                // Create a new DataContainer
                DataContainer dataContainer = new DataContainer();

                // Set the values for each field
                dataContainer.SetValue("ClassDisplayName", "Transformable View");
                dataContainer.SetValue("ClassName", "HBS.TransformableView");
                dataContainer.SetValue("ClassXmlSchema", @"<?xml version=""1.0"" encoding=""utf-8""?>
<xs:schema id=""NewDataSet"" xmlns="""" xmlns:xs=""http://www.w3.org/2001/XMLSchema"" xmlns:msdata=""urn:schemas-microsoft-com:xml-msdata"">
  <xs:element name=""NewDataSet"" msdata:IsDataSet=""true"" msdata:UseCurrentLocale=""true"">
    <xs:complexType>
      <xs:choice minOccurs=""0"" maxOccurs=""unbounded"">
        <xs:element name=""HBS_TransformableView"">
          <xs:complexType>
            <xs:sequence>
              <xs:element name=""TransformableViewID"" msdata:ReadOnly=""true"" msdata:AutoIncrement=""true"" type=""xs:int"" />
              <xs:element name=""TransformableViewName"">
                <xs:simpleType>
                  <xs:restriction base=""xs:string"">
                    <xs:maxLength value=""200"" />
                  </xs:restriction>
                </xs:simpleType>
              </xs:element>
              <xs:element name=""TransformableViewDisplayName"">
                <xs:simpleType>
                  <xs:restriction base=""xs:string"">
                    <xs:maxLength value=""200"" />
                  </xs:restriction>
                </xs:simpleType>
              </xs:element>
              <xs:element name=""TransformableViewContent"">
                <xs:simpleType>
                  <xs:restriction base=""xs:string"">
                    <xs:maxLength value=""2147483647"" />
                  </xs:restriction>
                </xs:simpleType>
              </xs:element>
              <xs:element name=""TransformableViewGuid"" msdata:DataType=""System.Guid, System.Private.CoreLib, Version=8.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"" type=""xs:string"" />
              <xs:element name=""TransformableViewLastModified"" type=""xs:dateTime"" />
              <xs:element name=""TransformableViewType"" type=""xs:int"" />
              <xs:element name=""TransformableViewTransformableViewTagID"" type=""xs:int"" />
              <xs:element name=""TransformableViewClassName"">
                <xs:simpleType>
                  <xs:restriction base=""xs:string"">
                    <xs:maxLength value=""200"" />
                  </xs:restriction>
                </xs:simpleType>
              </xs:element>
            </xs:sequence>
          </xs:complexType>
        </xs:element>
      </xs:choice>
    </xs:complexType>
    <xs:unique name=""Constraint1"" msdata:PrimaryKey=""true"">
      <xs:selector xpath="".//HBS_TransformableView"" />
      <xs:field xpath=""TransformableViewID"" />
    </xs:unique>
  </xs:element>
</xs:schema>");
                dataContainer.SetValue("ClassFormDefinition", @"<form><field column=""TransformableViewID"" columntype=""integer"" enabled=""true"" guid=""f9259ef8-39c7-4ab9-b88b-4ffbd9968c78"" isPK=""true"" /><field column=""TransformableViewName"" columnprecision=""0"" columnsize=""200"" columntype=""text"" enabled=""true"" guid=""4d039700-f045-44b7-a465-66e6d660b73b"" /><field column=""TransformableViewDisplayName"" columnprecision=""0"" columnsize=""200"" columntype=""text"" enabled=""true"" guid=""d4edadcf-786b-4322-8b76-592b2ca08fe6"" /><field column=""TransformableViewContent"" columnprecision=""0"" columntype=""longtext"" enabled=""true"" guid=""8347b140-f704-45ae-a2c9-42149d23ccb8"" /><field column=""TransformableViewType"" columnprecision=""0"" columntype=""integer"" enabled=""true"" guid=""20f1c702-03d7-46ae-8596-577d6f162a58""><properties><defaultvalue>0</defaultvalue></properties></field><field column=""TransformableViewTransformableViewTagID"" columnprecision=""0"" columntype=""integer"" enabled=""true"" guid=""50332ef8-ac1e-40b5-b3b3-9ddc924af5f8"" refobjtype=""cms.tag"" reftype=""Required"" /><field column=""TransformableViewClassName"" columnprecision=""0"" columnsize=""200"" columntype=""text"" enabled=""true"" guid=""4be9962f-1c2c-4570-b8b8-ee215eacfa24"" /><field column=""TransformableViewGuid"" columnprecision=""0"" columntype=""guid"" enabled=""true"" guid=""cc6ab941-4735-4641-a1c1-00178d004116"" /><field column=""TransformableViewLastModified"" columnprecision=""7"" columntype=""datetime"" enabled=""true"" guid=""17abc21d-cc4d-4ddb-9b68-236391638abe"" /></form>");
                dataContainer.SetValue("ClassTableName", "HBS_TransformableView");
                dataContainer.SetValue("ClassShowTemplateSelection", null); // Assuming null
                dataContainer.SetValue("ClassGUID", new Guid("021b6fde-8d0c-4770-bff8-bea84f53ede7"));
                dataContainer.SetValue("ClassContactMapping", null); // Assuming null
                dataContainer.SetValue("ClassContactOverwriteEnabled", null); // Assuming null
                dataContainer.SetValue("ClassConnectionString", null); // Assuming null
                dataContainer.SetValue("ClassDefaultObjectType", null); // Assuming null
                dataContainer.SetValue("ClassResourceID", resource.ResourceID);
                dataContainer.SetValue("ClassCodeGenerationSettings", @"<Data><CodeNameColumn>TransformableViewName</CodeNameColumn><DisplayNameColumn>TransformableViewDisplayName</DisplayNameColumn><GUIDColumn>TransformableViewGuid</GUIDColumn><LastModifiedColumn>TransformableViewLastModified</LastModifiedColumn><ObjectType>HBS.TransformableView</ObjectType><UseGuidHashtable>True</UseGuidHashtable><UseIdHashtable>True</UseIdHashtable><UseNameHashtable>True</UseNameHashtable></Data>");
                dataContainer.SetValue("ClassIconClass", null); // Assuming null
                dataContainer.SetValue("ClassHasUnmanagedDbSchema", false); // Assuming default value is false
                dataContainer.SetValue("ClassType", "Other");
                dataContainer.SetValue("ClassContentTypeType", null); // Assuming null
                dataContainer.SetValue("ClassWebPageHasUrl", null); // Assuming null
                dataContainer.SetValue("ClassShortName", null); // Assuming null

                classInfo = DataClassInfo.New(dataContainer);

                DataClassInfoProvider.SetDataClassInfo(classInfo);

                Console.WriteLine($"Added the {TransformableViewInfo.OBJECT_TYPE} Class");

                ConnectionHelper.ExecuteNonQuery(@"SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HBS_TransformableView](
	[TransformableViewID] [int] IDENTITY(1,1) NOT NULL,
	[TransformableViewName] [nvarchar](200) NOT NULL,
	[TransformableViewDisplayName] [nvarchar](200) NOT NULL,
	[TransformableViewContent] [nvarchar](max) NOT NULL,
	[TransformableViewGuid] [uniqueidentifier] NOT NULL,
	[TransformableViewLastModified] [datetime2](7) NOT NULL,
	[TransformableViewType] [int] NOT NULL,
	[TransformableViewTransformableViewTagID] [int] NOT NULL,
	[TransformableViewClassName] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_HBS_TransformableView] PRIMARY KEY CLUSTERED 
(
	[TransformableViewID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[HBS_TransformableView] ADD  CONSTRAINT [DEFAULT_HBS_TransformableView_TransformableViewName]  DEFAULT (N'') FOR [TransformableViewName]
GO
ALTER TABLE [dbo].[HBS_TransformableView] ADD  CONSTRAINT [DEFAULT_HBS_TransformableView_TransformableViewDisplayName]  DEFAULT (N'') FOR [TransformableViewDisplayName]
GO
ALTER TABLE [dbo].[HBS_TransformableView] ADD  CONSTRAINT [DEFAULT_HBS_TransformableView_TransformableViewContent]  DEFAULT (N'') FOR [TransformableViewContent]
GO
ALTER TABLE [dbo].[HBS_TransformableView] ADD  CONSTRAINT [DEFAULT_HBS_TransformableView_TransformableViewGuid]  DEFAULT ('00000000-0000-0000-0000-000000000000') FOR [TransformableViewGuid]
GO
ALTER TABLE [dbo].[HBS_TransformableView] ADD  CONSTRAINT [DEFAULT_HBS_TransformableView_TransformableViewLastModified]  DEFAULT ('1/1/0001 12:00:00 AM') FOR [TransformableViewLastModified]
GO
ALTER TABLE [dbo].[HBS_TransformableView] ADD  CONSTRAINT [DEFAULT_HBS_TransformableView_TransformableViewType]  DEFAULT ((0)) FOR [TransformableViewType]
GO
ALTER TABLE [dbo].[HBS_TransformableView] ADD  CONSTRAINT [DEFAULT_HBS_TransformableView_TransformableViewTransformableViewTagID]  DEFAULT ((0)) FOR [TransformableViewTransformableViewTagID]
GO
ALTER TABLE [dbo].[HBS_TransformableView] ADD  CONSTRAINT [DEFAULT_HBS_TransformableView_TransformableViewClassName]  DEFAULT (N'') FOR [TransformableViewClassName]
GO", null, queryType: QueryTypeEnum.SQLQuery);

                Console.WriteLine($"Created the HBS_TransformableView Table");

                Console.WriteLine($"Module installation completed");
            } else
            {
                Console.WriteLine($"Module already installed");
            }

            Environment.Exit(0);
        }

        public void CreateJsonLoadFile(string extractPath)
        {
            Console.WriteLine("Beginning export...");
            var tempPath = CMS.IO.Path.Combine(System.IO.Path.GetTempPath(), "TransformableViews_Export");
            var viewsPath = CMS.IO.Path.Combine(tempPath, "Views");


            var taxonomies = (taxonomyInfoProvider.Get().GetEnumerableTypedResult()).GetCategories();
            var tags = (tagInfoProvider.Get().GetEnumerableTypedResult()).GetCategories();

            IEnumerable<ITransformableViewItem> views = transformableViewInfoProvider.Get().GetEnumerableTypedResult();

            var tagIds = views.Select(x => x.TransformableViewTransformableViewTagID).Distinct();

            var filteredTags = tags.Where(x => tagIds.Contains(x.TransformableViewCategoryID));
            filteredTags = GetAllParents(filteredTags, tags);

            var taxIds = filteredTags.Select(x => x.TransformableViewCategoryRootID);

            var taxonomyJson = JsonSerializer.Serialize(taxonomies.Where(x => taxIds.Contains(x.TransformableViewCategoryID)));
            var tagJson = JsonSerializer.Serialize(filteredTags);
            var viewJson = new Dictionary<string, string>();
            foreach (var view in views)
            {
                view.TransformableViewContent = encryptionService.DecryptString(view.TransformableViewContent);
                viewJson.Add(view.TransformableViewName, JsonSerializer.Serialize(view));
            }

            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }

            Directory.CreateDirectory(viewsPath);

            File.WriteAllText(CMS.IO.Path.Combine(tempPath, "taxonomies.json"), taxonomyJson);
            File.WriteAllText(CMS.IO.Path.Combine(tempPath, "tags.json"), tagJson);

            foreach (var item in viewJson)
            {
                File.WriteAllText(CMS.IO.Path.Combine(viewsPath, $"{item.Key}.json"), item.Value);
            }


            if (File.Exists(extractPath))
            {
                File.Delete(extractPath);
            }

            System.IO.Compression.ZipFile.CreateFromDirectory(tempPath, extractPath);

            Console.WriteLine($"Exported {viewJson.Count} views to {extractPath}");

            Environment.Exit(0);
        }

        public void ReadJsonLoadFile(string importPath)
        {
            Console.WriteLine("Beginning import...");

            if (!File.Exists(importPath))
            {
                Console.WriteLine($"Import package not found at {importPath}.  Please provide correct path.");
                Environment.Exit(1);
                return;
            }

            var tempPath = CMS.IO.Path.Combine(System.IO.Path.GetTempPath(), "TransformableViews_Export");

            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }

            Directory.CreateDirectory(tempPath);

            System.IO.Compression.ZipFile.ExtractToDirectory(importPath, tempPath);

            var taxonomies = JsonSerializer.Deserialize<TransformableViewCategoryItemParent[]>(File.ReadAllText(CMS.IO.Path.Combine(tempPath, "taxonomies.json")))?.ToList();
            var tags = JsonSerializer.Deserialize<TransformableViewCategoryItemParent[]>(File.ReadAllText(CMS.IO.Path.Combine(tempPath, "tags.json")))?.ToList();

            if(tags == null || taxonomies == null || tags.Count == 0 || taxonomies.Count == 0)
            {
                Console.WriteLine("You must have tags and taxonomies to import");
                Environment.Exit(1);
                return;
            }

            var files = Directory.EnumerateFiles(CMS.IO.Path.Combine(tempPath, "Views"));

            var transformableViews = files?.Select(x=> JsonSerializer.Deserialize<TransformableView>(File.ReadAllText(x))).Where(x=>x != null) as IEnumerable<TransformableView>;

            if(transformableViews == null || !transformableViews.Any())
            {
                Console.WriteLine("No views found to import");
                Environment.Exit(1);
                return;
            }

            // Add Views to tags
            foreach(var tag in tags)
            {
                tag.Views = transformableViews.Where(x => x.TransformableViewTransformableViewTagID == tag.TransformableViewCategoryID);
            }

            var lookup = tags.ToLookup(l => l.TransformableViewCategoryParentID);

            // Create the Hirarchal layout
            foreach(var tag in tags)
            {
                tag.Children = lookup[tag.TransformableViewCategoryID];
            }

            // Select only the upper most levels.  
            tags = tags.Where(x => x.TransformableViewCategoryParentID == 0).ToList();

            foreach(var taxonomy in taxonomies)
            {
                taxonomy.Children = tags.Where(x => x.TransformableViewCategoryRootID == taxonomy.TransformableViewCategoryID);
            }

            UpdateInsertTaxonomies(taxonomies);

            Console.WriteLine($"{transformableViews.Count()} view(s), {tags.Count} tag(s) and {taxonomies.Count} taxonomy(s) were imported from {importPath}.");

            Environment.Exit(0);
        }

        private void UpdateInsertTaxonomies(IEnumerable<TransformableViewCategoryItemParent> taxonomies)
        {
            // using nested loops in order to keep track of the updated ids without causing any issues. 
            foreach(var taxonomy in taxonomies)
            {
                var tax = taxonomyInfoProvider.Get(taxonomy.TransformableViewCategoryName);

                if(tax == null)
                {
                    tax = TaxonomyInfo.New();
                    tax.TaxonomyTitle = taxonomy.TransformableViewCategoryTitle;
                    tax.TaxonomyName = taxonomy.TransformableViewCategoryName;
                    taxonomyInfoProvider.Set(tax);
                }

                foreach(var tag in taxonomy.Children)
                {
                    UpdateInsertTags(tag, tax.TaxonomyID, null);
                }
            }
        }

        private void UpdateInsertTags(TransformableViewCategoryItemParent tagItem, int taxonomyID, int? parentID)
        {
            var tagUpdate = tagInfoProvider.Get().WhereEquals(nameof(TagInfo.TagName), tagItem.TransformableViewCategoryName).WhereEquals(nameof(TagInfo.TagTaxonomyID), taxonomyID).FirstOrDefault();

            if(tagUpdate == null)
            {
                tagUpdate = TagInfo.New();
                tagUpdate.TagTitle = tagItem.TransformableViewCategoryTitle;
                tagUpdate.TagName = tagItem.TransformableViewCategoryName;
                if (parentID != null)
                {
                    tagUpdate.TagParentID = parentID ?? 0;
                }
                tagUpdate.TagTaxonomyID = taxonomyID;
                if (tagItem.TransformableViewCategoryOrder != 0)
                {
                    tagUpdate.TagOrder = tagItem.TransformableViewCategoryOrder ?? 0;
                }
                tagInfoProvider.Set(tagUpdate);
            }

            foreach(var tag in tagItem.Children)
            {
                UpdateInsertTags(tag, taxonomyID, tagUpdate.TagID);
            }
            UpdateInsertViews(tagItem.Views, tagUpdate.TagID);
        }

        private void UpdateInsertViews(IEnumerable<ITransformableViewItem> views, int tagID)
        {
            foreach(var viewItem in views)
            {
                var view = transformableViewInfoProvider.Get(viewItem.TransformableViewName);
                var viewName = viewItem.TransformableViewName;
                var displayName = viewItem.TransformableViewDisplayName;

                if (view != null && (view.TransformableViewType != viewItem.TransformableViewType || (view.TransformableViewClassName ?? "") != (viewItem.TransformableViewClassName ?? "")))
                {
                    var guid = Guid.NewGuid().ToString();
                    viewName = viewItem.TransformableViewName + "_" + guid;
                    viewName = viewName[..199];
                    displayName = $"{viewItem.TransformableViewDisplayName} (guid)"[..199];
                    Console.WriteLine($"View {viewItem.TransformableViewDisplayName} already exists in the system under a different type or classname.  Using {displayName} instead to avoid conflicts");
                }

                if (view == null)
                {
                    view = TransformableViewInfo.New();
                    view.TransformableViewDisplayName = viewItem.TransformableViewDisplayName;
                }

                view.TransformableViewTransformableViewTagID = tagID;
                view.TransformableViewType = viewItem.TransformableViewType;
                view.TransformableViewClassName = viewItem.TransformableViewClassName;
                view.TransformableViewName = viewName;
                view.TransformableViewContent = viewItem.TransformableViewContent;
                transformableViewInfoProvider.Set(view);
            }
        }

        private List<TransformableViewCategoryItem> GetAllParents(IEnumerable<TransformableViewCategoryItem> items, IEnumerable<TransformableViewCategoryItem> originalList)
        {
            var parentIds = items.Select(x => x.TransformableViewCategoryParentID);
            var parents = originalList.Where(x => parentIds.Contains(x.TransformableViewCategoryID));

            var itemList = items.ToList();
            if (parents.Any())
            {
                var parentList = GetAllParents(parents, originalList);
                if (parentList.Any())
                {
                    foreach(var item in parentList)
                    {
                        if(!itemList.Where(x=>x.TransformableViewCategoryID == item.TransformableViewCategoryID).Any())
                        {
                            itemList.Add(item);
                        }
                    }
                }
            }
            return itemList;
        }
    }
}
