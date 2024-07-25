using HBS.Xperience.TransformableViewsAdmin.Admin.UIPages;

namespace HBS.Xperience.TransformableViewsAdmin.Admin.Models
{
    public class TransformableViewItem
    {
        public string TransformableViewContent { get; set; }
        public string TransformableViewDisplayName { get; set; }
        public Guid? TransformableViewGuid { get; set; }
        public int? TransformableViewID { get; set; }
        public DateTime? TransformableViewLastModified { get; set; }
        public DateTime? TransformableViewLastRequested { get; set; }
        public string? TransformableViewName { get; set; }
        public int TransformableViewTransformableViewCategoryID { get; set; }
        public int TransformableViewType { get; set; } = 0;
        public string TransformableViewClassName { get; set; }
    }
}
