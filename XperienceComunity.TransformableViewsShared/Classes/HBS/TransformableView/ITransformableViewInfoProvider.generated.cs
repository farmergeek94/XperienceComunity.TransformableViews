using CMS.DataEngine;

namespace HBS.TransformableViews_Experience
{
    /// <summary>
    /// Declares members for <see cref="TransformableViewInfo"/> management.
    /// </summary>
    public partial interface ITransformableViewInfoProvider : IInfoProvider<TransformableViewInfo>, IInfoByIdProvider<TransformableViewInfo>, IInfoByNameProvider<TransformableViewInfo>, IInfoByGuidProvider<TransformableViewInfo>
    {
    }
}