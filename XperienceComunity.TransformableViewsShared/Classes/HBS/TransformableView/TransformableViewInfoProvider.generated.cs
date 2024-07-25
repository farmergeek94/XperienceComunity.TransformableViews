using CMS.DataEngine;

namespace HBS.TransformableViews_Experience
{
    /// <summary>
    /// Class providing <see cref="TransformableViewInfo"/> management.
    /// </summary>
    [ProviderInterface(typeof(ITransformableViewInfoProvider))]
    public partial class TransformableViewInfoProvider : AbstractInfoProvider<TransformableViewInfo, TransformableViewInfoProvider>, ITransformableViewInfoProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransformableViewInfoProvider"/> class.
        /// </summary>
        public TransformableViewInfoProvider()
            : base(TransformableViewInfo.TYPEINFO)
        {
        }
    }
}