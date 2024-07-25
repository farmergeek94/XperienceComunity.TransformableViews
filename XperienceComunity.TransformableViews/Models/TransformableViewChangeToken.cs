using CMS.Core;
using CMS.Helpers;
using HBS.TransformableViews;
using HBS.Xperience.TransformableViewsShared.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Path = System.IO.Path;

namespace HBS.Xperience.TransformableViews.Models
{
    internal class TransformableViewChangeToken : IChangeToken
    {
        private readonly string _filter;

        private ITransformableViewRepository _repository => Service.Resolve<ITransformableViewRepository>();

        public TransformableViewChangeToken(string filter)
        {
            _filter = filter;
        }

        public bool ActiveChangeCallbacks => false;

        public bool HasChanged
        {
            get
            {
                try
                {
                    if (_filter.IndexOf("TransformableView") > -1 && _filter.IndexOf("_ViewImports") < 0)
                    {
                        var viewName = Path.GetFileName(_filter).Replace(".cshtml", "");
                        var view = _repository.GetTransformableView(viewName);
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
