using CMS.ContentEngine;
using CMS.Websites;
using HBS.Xperience.TransformableViewsShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XperienceComunity.TransformableViewsShared.Models;

namespace XperienceComunity.TransformableViewsShared.Library
{
    public static class TransformableViewPageHelper
    {
        public static IEnumerable<PageTemplateConfigurationItem> GetTemplateItems(this IEnumerable<PageTemplateConfigurationInfo> infos)
        {
            foreach (var item in infos)
            {
                PageTemplateConfigurationItem nItem = new()
                {
                    PageTemplateConfigurationID = item.PageTemplateConfigurationID,
                    PageTemplateConfigurationName = item.PageTemplateConfigurationName,
                    PageTemplateConfigurationDescription = item.PageTemplateConfigurationDescription,
                    PageTemplateConfigurationIcon = item.PageTemplateConfigurationIcon,
                    PageTemplateConfigurationTemplate = item.PageTemplateConfigurationTemplate,
                    PageTemplateConfigurationWidgets = item.PageTemplateConfigurationWidgets,
                    PageTemplateConfigurationLastModified = item.PageTemplateConfigurationLastModified,
                    PageTemplateConfigurationGUID = item.PageTemplateConfigurationGUID,
                };
                yield return nItem;
            }
        }
    }
}
