using HBS.TransformableViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XperienceComunity.TransformableViewsShared.Models
{
    public class TransformableView : ITransformableViewItem
    {
        public string TransformableViewContent { get; set; }
        public string TransformableViewDisplayName { get; set; }
        public Guid TransformableViewGuid { get; set; }
        public int TransformableViewID { get; set; }
        public DateTime TransformableViewLastModified { get; set; }
        public string TransformableViewName { get; set; }
        public int TransformableViewTransformableViewTagID { get; set; }
        public int TransformableViewType { get; set; }
        public string TransformableViewClassName { get; set; }
    }
}
