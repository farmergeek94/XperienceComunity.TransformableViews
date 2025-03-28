using CMS.ContentEngine;
using HBS.Xperience.TransformableViewsShared.Repositories;
using HBS.Xperience.TransformableViewsShared.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Path = System.IO.Path;

namespace HBS.Xperience.TransformableViews.Models
{
    /// <summary>
    /// Class allowing us to check and see if a views timestamp has changed
    /// </summary>
    internal class TransformableViewChangeToken : IChangeToken
    {
        private readonly ITransformableViewRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _filter;

        public TransformableViewChangeToken(ITransformableViewRepository repository, IHttpContextAccessor httpContextAccessor, string filter)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _filter = filter;
        }

        public bool ActiveChangeCallbacks => false;

        public bool HasChanged
        {
            get
            {
                try
                {
                    // check if a TransformableView
                    if (_filter.IndexOf("TransformableView") > -1 && _filter.IndexOf("_ViewImports") < 0)
                    {
                        // get the repo

                        var viewName = Path.GetFileName(_filter).Replace(".cshtml", "");

                        var cacheService = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<ICacheService>();

                        // get the view (cached)
                        var view = _repository.GetTransformableViews(viewName, cacheService.GetCachedLanguage()).ConfigureAwait(false).GetAwaiter().GetResult();
                        // run the logic to check and see if the view needs updating
                        if (view != null)
                        {
                            var wasRequested = _repository.LastViewedDates.ContainsKey(viewName);

                            if (!wasRequested)
                            {
                                return false;
                            }
                            return _repository.LastModified[viewName] > _repository.LastViewedDates[viewName];
                        }
                    }
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public IDisposable RegisterChangeCallback(Action<object?> callback, object? state) => EmptyDisposable.Instance;

        internal class EmptyDisposable : IDisposable
        {
            public static EmptyDisposable Instance { get; } = new EmptyDisposable();
            private EmptyDisposable() { }
            public void Dispose() { }
        }
    }
}
