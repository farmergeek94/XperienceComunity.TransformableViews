using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBS.Xperience.TransformableViewsShared.Models
{
    public class TransformableViewObjectsFormComponentModel : TransformableViewModelBase
    {
        public string ClassName { get; set; } = string.Empty;
        public string Columns { get; set; } = string.Empty;
        public string WhereCondition { get; set; } = string.Empty;
        public string OrderBy { get; set; } = string.Empty;
        public int? TopN { get; set; } = null;
    }
}
