using CMS.ContentEngine;
using HBS.Xperience.TransformableViewsShared.Repositories;
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
        private readonly string _filter;
        private readonly string? _language;

        public TransformableViewChangeToken(ITransformableViewRepository repository, string filter, string? language)
        {
            _repository = repository;
            _filter = filter;
            _language = language;
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

                        // get the view (cached)
                        var view = _repository.GetTransformableViews(viewName, _language).ConfigureAwait(false).GetAwaiter().GetResult();
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
