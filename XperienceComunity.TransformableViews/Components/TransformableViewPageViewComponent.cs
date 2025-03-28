using CMS.ContentEngine;
using HBS.Xperience.TransformableViews.Repositories;
using HBS.Xperience.TransformableViewsShared.Repositories;
using HBS.Xperience.TransformableViewsShared.Services;
using Kentico.Content.Web.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;

namespace HBS.Xperience.TransformableViews.Components
{
    // Page template component that allows us return a dynamic view.
    public class TransformableViewPageViewComponent : ViewComponent
    {
        private readonly IContentItemRetriever _webPageRetriever;
        private readonly ITransformableViewRepository _transformableViewRepository;
        private readonly ICacheService _cacheService;

        public TransformableViewPageViewComponent (IContentItemRetriever webPageRetriever, ITransformableViewRepository transformableViewRepository, ICacheService cacheService)
        {
            _webPageRetriever = webPageRetriever;
            _transformableViewRepository = transformableViewRepository;
            _cacheService = cacheService;
        }

        public async Task<IViewComponentResult> InvokeAsync(IEnumerable<ContentItemReference> view)
        {
            if (!view.Any())
            {
                return Content("Please select a view");
            }
            var viewItem = await _transformableViewRepository.GetTransformableViews(view.FirstOrDefault()?.Identifier ?? Guid.Empty, _cacheService.GetCachedLanguage());
            var model = await _webPageRetriever.GetWebPage(User.Identity?.IsAuthenticated ?? false);
            if (model == null)
            {
                return View($"TransformableView/{viewItem?.TransformableDatabaseViewCodeName}");
            }
            return View($"TransformableView/{viewItem?.TransformableDatabaseViewCodeName}", model);
        }
    }
}
