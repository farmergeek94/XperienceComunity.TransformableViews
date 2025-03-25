using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using Kentico.Xperience.Admin.Base.Components;
using Kentico.Xperience.Admin.Base.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using XperienceComunity.TransformableViewsAdmin.Admin.FormComponents;
using XperienceComunity.TransformableViewsShared.Library;

[assembly: RegisterFormComponent("HBS.UIFormComponents.ClassSelector",
                                 typeof(ClassSelectorFormComponent),
                                 "Class Selector")]
namespace XperienceComunity.TransformableViewsAdmin.Admin.FormComponents
{
    public class ClassSelectorFormComponent : MultipleObjectSelectorBase<ClassSelectorFormComponentProperties, string>
    {
        public ClassSelectorFormComponent(IObjectsRetriever objectsRetriever, IObjectSelectorWhereConditionProviderActivator whereConditionProviderActivator, ILocalizationService localizationService) : base(objectsRetriever, whereConditionProviderActivator, localizationService)
        {
        }

        protected override void ConfigureComponent()
        {
            Properties.WhereCondition = new WhereCondition().WhereNotEquals(nameof(DataClassInfo.ClassType), "Content");
            Properties.ObjectType = DataClassInfo.OBJECT_TYPE;
            base.ConfigureComponent();
        }

        protected override IEnumerable<string> ConvertFormComponentValue(IEnumerable<string> value)
        {
            return value ?? [];
        }

        protected override IEnumerable<string> ConvertFormComponentSingleItemValue(IEnumerable<string> value)
        {
            return base.ConvertFormComponentSingleItemValue(value);
        }

        protected override string ExtractValue(IDataContainer dataContainer, ObjectTypeInfo typeInfo)
        {
            return dataContainer[typeInfo.CodeNameColumn].ToString(null);
        }
    }



    // Client properties class
    public class ClassSelectorFormComponentProperties : ObjectSelectorProperties
    {
    }
}
