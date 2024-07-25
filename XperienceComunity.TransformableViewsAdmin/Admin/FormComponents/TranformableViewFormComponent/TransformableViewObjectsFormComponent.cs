using CMS.DataEngine;
using HBS.Xperience.TransformableViewsShared.Library;
using HBS.Xperience.TransformableViewsShared.Models;
using HBS.Xperience.TransformableViewsShared.Repositories;
using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Admin.Base.Forms;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBS.Xperience.TransformableViewsAdmin.Admin.FormComponents.TranformableViewFormComponent
{
    /// <summary>
    /// Form component to allow us to use a custom react component for display
    /// </summary>
    [ComponentAttribute(typeof(TransformableViewObjectsFormComponentAttribute))]
    public class TransformableViewObjectsFormComponent : FormComponent<TransformableViewObjectsFormComponentProperties, TransformableViewObjectsFormComponentClientProperties, TransformableViewObjectsFormComponentModel>
    {
        private readonly ITransformableViewRepository _transformableViewRepository;

        public override string ClientComponentName => "@hbs/xperience-transformable-views/TransformableViewObjects";

        public TransformableViewObjectsFormComponent(ITransformableViewRepository transformableViewRepository)
        {
            _transformableViewRepository = transformableViewRepository;
        }

        /// <summary>
        /// Get the object views based on classname
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        [FormComponentCommand]
        public async Task<ICommandResponse> GetViews(string className)
        {
            IEnumerable<SelectListItem> views = await _transformableViewRepository.GetTransformableViewObjectSelectItems(className);
            return ResponseFrom(views);
        }

        /// <summary>
        /// get the availabel object types.
        /// </summary>
        /// <returns></returns>
        [FormComponentCommand]
        public async Task<ICommandResponse> GetObjectTypes()
        {
            var exisitingTypes = ObjectTypeManager.ExistingObjectTypes;
            var types = ObjectTypeManager.RegisteredTypes.Where(x => exisitingTypes.Contains(x.ObjectType)).Select(x => new SelectListItem(x.ObjectClassName, x.ObjectType));
            return ResponseFrom(types);
        }
    }
    public class TransformableViewObjectsFormComponentProperties : FormComponentProperties
    {
    }

    public class TransformableViewObjectsFormComponentClientProperties : FormComponentClientProperties<TransformableViewObjectsFormComponentModel>
    {
    }
}
