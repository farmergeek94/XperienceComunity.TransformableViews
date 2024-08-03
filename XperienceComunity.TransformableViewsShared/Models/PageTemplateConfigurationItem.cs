using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XperienceComunity.TransformableViewsShared.Models
{
    public class PageTemplateConfigurationItem
    {
        public int PageTemplateConfigurationID { get; set; }
        public Guid PageTemplateConfigurationGUID { get; set; }
        public DateTime PageTemplateConfigurationLastModified { get; set; }
        public string PageTemplateConfigurationName { get; set; }
        public string PageTemplateConfigurationDescription { get; set; }
        public string PageTemplateConfigurationIcon { get; set; }
        public string PageTemplateConfigurationTemplate { get; set; }
        public string PageTemplateConfigurationWidgets { get; set; }
    }
}
