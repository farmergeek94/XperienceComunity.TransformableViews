using CMS.Core;
using CMS.DataEngine;
using HBS.Xperience.TransformableViewsShared.Services;

namespace HBS.TransformableViews_Experience
{
    public partial class TransformableViewInfoProvider
    {
        public override void Set(TransformableViewInfo info)
        {
            var service = Service.Resolve<IEncryptionService>();
            base.Set(service.EncryptView(info));
        }
    }
}