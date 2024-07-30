using Kentico.Xperience.Admin.Base.FormAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBS.Xperience.TransformableViewsShared.Models
{
    public class TransformableViewModelBase
    {
        [TextInputComponent(Label = "View Title", Order = 900)]
        public string ViewTitle { get; set; } = "";
        [TextInputComponent(Label = "View Classes", Order = 901)]
        public string ViewClassNames { get; set; } = "";
        [TextAreaComponent(Label = "View Custom Content", Order = 902)]
        public string ViewCustomContent { get; set; } = "";

        public virtual string View { get; set; } = string.Empty;
    }
}
