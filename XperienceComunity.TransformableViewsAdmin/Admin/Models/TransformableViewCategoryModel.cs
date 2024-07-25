namespace HBS.Xperience.TransformableViewsAdmin.Admin.Models
{
    public class TransformableViewCategoryModel
    {
        public string TransformableViewCategoryDisplayName { get; set; } = string.Empty;
        public Guid? TransformableViewCategoryGuid { get; set; }
        public int? TransformableViewCategoryID { get; set; }
        public DateTime? TransformableViewCategoryLastModified { get; set; }
        public string? TransformableViewCategoryName { get; set; }
        public int? TransformableViewCategoryParentID { get; set; }
        public int TransformableViewCategoryOrder { get; set; }
    }
}
