﻿using CMS.ContentEngine;
using CMS.Websites;
using HBS.TransformableViews;
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
