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
        private readonly IServiceProvider _serviceProvider;
        private readonly string _filter;

        public TransformableViewChangeToken(IServiceProvider serviceProvider, string filter)
        {
            _serviceProvider = serviceProvider;
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
                        var _repository = _serviceProvider.GetRequiredService<ITransformableViewRepository>();

                        var viewName = Path.GetFileName(_filter).Replace(".cshtml", "");

                        // get the view (cached)
                        var view = _repository.GetTransformableView(viewName);

                        // run the logic to check and see if the view needs updating
                        if (view != null)
                        {
                            var wasRequested = _repository.LastViewedDates.ContainsKey(viewName);

                            if (!wasRequested)
                            {
                                return false;
                            }
                            return view.TransformableViewLastModified > _repository.LastViewedDates[viewName];
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
