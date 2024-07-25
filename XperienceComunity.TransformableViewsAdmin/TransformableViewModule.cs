using CMS.Core;
using CMS;
using CMS.DataEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kentico.Xperience.Admin.Base;
using HBS.Xperience.TransformableViewsAdmin;

[assembly: RegisterModule(typeof(TransformableViewModule))]

namespace HBS.Xperience.TransformableViewsAdmin
{
    internal class TransformableViewModule : AdminModule
    {
        public TransformableViewModule() : base("HBS.Xperience.TransformableViews")
        {
        }

        protected override void OnInit()
        {
            base.OnInit();
            RegisterClientModule("hbs", "xperience-transformable-views");
        }
    }
}
