﻿namespace HBS.Xperience.TransformableViewsShared.Models
{
    public class TransformableViewCategoryItem
    {
        public int TransformableViewCategoryID { get; set; }
        public string TransformableViewCategoryName { get; set; } = string.Empty;
        public int? TransformableViewCategoryOrder { get; set; }
        public int? TransformableViewCategoryParentID { get; set; }
        public int? TransformableViewCategoryRootID { get; set; }
        public string TransformableViewCategoryTitle { get; set; } = string.Empty;
    }
}