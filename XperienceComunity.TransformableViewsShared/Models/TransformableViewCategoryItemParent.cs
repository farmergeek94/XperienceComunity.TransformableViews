using HBS.TransformableViews;

namespace HBS.Xperience.TransformableViewsShared.Models
{
    public class TransformableViewCategoryItemParent : TransformableViewCategoryItem
    {
        public IEnumerable<TransformableViewCategoryItemParent> Children { get; set; } = [];

        public IEnumerable<ITransformableViewItem> Views { get; set; } = [];

    }
}