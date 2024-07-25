using CMS.Core;
using CMS.Helpers;
using HBS.TransformableViews;
using HBS.Xperience.TransformableViewsShared.Repositories;
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
        private readonly IServiceProvider _serviceProvider;
        private string _viewPath = "";

        private byte[] _viewContent = Array.Empty<byte>();
        private DateTimeOffset _lastModified = DateTime.MinValue;
        private bool _exists = false;

        public TransformableViewFile(IServiceProvider serviceProvider, string viewName)
        {
            _serviceProvider = serviceProvider;
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
                    // Get the view
                    var serviceRepo = _serviceProvider.GetRequiredService<ITransformableViewRepository>();
                    var view = serviceRepo.GetTransformableView(viewName, true);
                    if (view != null)
                    {
                        // se the properties that will be used later.
                        _viewContent = Encoding.UTF8.GetBytes(usings + view.TransformableViewContent);
                        _lastModified = view.TransformableViewLastModified;
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