using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using HBS.Xperience.TransformableViewsShared.Repositories;
using HBS.Xperience.TransformableViewsShared.Services;
using Kentico.Content.Web.Mvc.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Path = System.IO.Path;

namespace Xperience.Community.TransformableViews.Models
{
    /// <summary>
    /// Get a view from the database as a file.
    /// </summary>
    internal class TransformableViewFile : IFileInfo
    {
        private readonly ITransformableViewRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string _viewPath = "";
        private byte[] _viewContent = Array.Empty<byte>();
        private DateTimeOffset _lastModified = DateTime.MinValue;
        private bool _exists = false;

        public TransformableViewFile(ITransformableViewRepository repository, IHttpContextAccessor httpContextAccessor, string viewName)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _viewPath = viewName;
            GetView(viewName);
        }
        public bool Exists => _exists;

        public bool IsDirectory => false;

        public DateTimeOffset LastModified => _lastModified;

        public long Length
        {
            get
            {
                using (var stream = new MemoryStream(_viewContent))
                {
                    return stream.Length;
                }
            }
        }

        public string Name => Path.GetFileName(_viewPath);

        public string PhysicalPath => null;

        public Stream CreateReadStream()
        {
            return new MemoryStream(_viewContent);
        }

        private void GetView(string viewPath)
        {
            // Add some usings on top of the views
            var usings = @"@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
@using Kentico.PageBuilder.Web.Mvc
@using Kentico.Web.Mvc
";
            // Check if TransformableView
            if (viewPath.IndexOf("TransformableView") > -1 && viewPath.IndexOf("_ViewImports") < 0)
            {
                var viewName = Path.GetFileName(viewPath).Replace(".cshtml","");

                try
                {
                    var cacheService = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<ICacheService>();
                    // Get the view
                    var view = _repository.GetTransformableViews(viewName, cacheService.GetCachedLanguage(), true).ConfigureAwait(false).GetAwaiter().GetResult();
                    if (view != null)
                    {
                        // se the properties that will be used later.
                        _viewContent = Encoding.UTF8.GetBytes(usings + view.TransformableDatabaseViewEditor);
                        _lastModified = _repository.LastModified[viewName];
                        _exists = true;
                    }
                }
                catch (Exception ex)
                {
                    // if something went wrong, Exists will be false
                }
            }
        }
    }
}