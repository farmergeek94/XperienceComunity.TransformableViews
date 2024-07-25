using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBS.Xperience.TransformableViewsShared.Models
{
    public class TransformableViewModelBase
    {
        public string ViewTitle { get; set; } = "";
        public string ViewClassNames { get; set; } = "";
        public string ViewCustomContent { get; set; } = "";
        public string View { get; set; } = string.Empty;
    }
}
