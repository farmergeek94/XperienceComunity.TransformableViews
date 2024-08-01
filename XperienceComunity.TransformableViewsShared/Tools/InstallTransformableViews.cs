using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine.Internal;
using CMS.Modules;
using CommandLine;
using HBS.TransformableViews_Experience;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HBS.Xperience.TransformableViewsShared.TransformableViewServices;

namespace XperienceComunity.TransformableViewsTool
{
    internal class InstallTransformableViews(IInfoProvider<ResourceInfo> resourceInfoProvider) : IInstallTransformableViews
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
    }
}
