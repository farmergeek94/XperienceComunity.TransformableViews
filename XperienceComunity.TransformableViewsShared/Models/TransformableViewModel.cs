using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBS.Xperience.TransformableViewsShared.Models
{
    public class TransformableViewModel : TransformableViewModelBase
    {
        public dynamic Items { get; set; } = Enumerable.Empty<dynamic>();
    }
}
