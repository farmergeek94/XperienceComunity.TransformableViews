using CMS.Core;
using CMS.Helpers;
using HBS.Xperience.TransformableViews.Models;
using HBS.Xperience.TransformableViewsShared.Models;
using HBS.Xperience.TransformableViewsShared.Repositories;
using HBS.Xperience.TransformableViewsShared.Services;
using Microsoft.AspNetCore.Http;
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
    /// <summary>
    /// The file provider giving us access to the views. 
    /// </summary>
    internal class TransformableViewFileProvider(ITransformableViewRepository repository, IHttpContextAccessor httpContextAccessor) : IFileProvider
    {
        /// <summary>
        /// Just return null here as there are no directory contents and they are not needed
        /// </summary>
        /// <param name="subpath"></param>
        /// <returns></returns>
        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return null;
        }

        /// <summary>
        /// get the file info from the transformable view.
        /// </summary>
        /// <param name="subpath"></param>
        /// <returns></returns>
        public IFileInfo GetFileInfo(string subpath)
        {
            return new TransformableViewFile(repository, httpContextAccessor, subpath);
        }

        /// <summary>
        /// Check for 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IChangeToken Watch(string filter)
        {
            return new TransformableViewChangeToken(repository, httpContextAccessor, filter);
        }
    }
}
