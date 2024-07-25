using HBS.TransformableViews;
using HBS.Xperience.TransformableViewsShared.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBS.TransformableViews_Experience
{
    public partial class TransformableViewInfo : ITransformableViewItem
    {
        public TransformableViewTypeEnum TransformableViewTypeEnum
        {
            get => (TransformableViewTypeEnum)TransformableViewType; 
            set => TransformableViewType = (int)value;
        }
    }
}
