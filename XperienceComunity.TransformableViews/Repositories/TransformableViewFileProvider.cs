using CMS.Helpers;
using HBS.TransformableViews;
using HBS.Xperience.TransformableViews.Models;
using HBS.Xperience.TransformableViewsShared.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xperience.Community.TransformableViews.Models;

namespace HBS.Xperience.TransformableViews.Repositories
{
    internal class TransformableViewFileProvider : IFileProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public TransformableViewFileProvider(IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
        }
        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return null;
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            return new TransformableViewFile(_serviceProvider, subpath);
        }

        public IChangeToken Watch(string filter)
        {
            return new TransformableViewChangeToken(filter);
        }
    }
}
