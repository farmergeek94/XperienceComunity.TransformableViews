namespace HBS.TransformableViews
{
    public interface ITransformableViewItem
    {
        string TransformableViewContent { get; set; }
        string TransformableViewDisplayName { get; set; }
        Guid TransformableViewGuid { get; set; }
        int TransformableViewID { get; set; }
        DateTime TransformableViewLastModified { get; set; }
        string TransformableViewName { get; set; }
        int TransformableViewTransformableViewTagID { get; set; }
        int TransformableViewType { get; set; }
        string TransformableViewClassName { get; set; }
    }
}