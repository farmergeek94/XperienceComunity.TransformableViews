using CMS.ContentEngine;
using CMS.Helpers;
using CMS.Websites.Routing;
using HBS.Xperience.TransformableViews.Components;
using HBS.Xperience.TransformableViewsShared.Library;
using HBS.Xperience.TransformableViewsShared.Models;
using HBS.Xperience.TransformableViewsShared.Repositories;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kentico.Xperience.Admin.Base.Forms;
using HBS.Xperience.TransformableViews.Repositories;

namespace HBS.Xperience.TransformableViews.Components
{
    public class TransformableViewPageViewComponent : ViewComponent
    {
        private readonly IContentItemRetriever _webPageRetriever;

        public TransformableViewPageViewComponent (IContentItemRetriever webPageRetriever)
        {
            _webPageRetriever = webPageRetriever;
        }

        public async Task<IViewComponentResult> InvokeAsync(IEnumerable<ObjectRelatedItem> view)
        {
            if (!view.Any())
            {
                return Content("Please select a view");
            }
            var model = await _webPageRetriever.GetWebPage(User.Identity?.IsAuthenticated ?? false);
            if (model == null)
            {
                return View(view.FirstOrDefault()?.ObjectCodeName);
            }
            return View(view.FirstOrDefault()?.ObjectCodeName, model);
        }
    }
}
