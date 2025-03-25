using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XperienceComunity.TransformableViewsShared.Services
{
    internal class ViewSettingsService(bool deleteViewsOnImport) : IViewSettingsService
    {
        public bool DeleteViewsOnImport { get; } = deleteViewsOnImport;

        public string WorkSpaceName { get; set; } = "KenticoDefault";
    }
}
