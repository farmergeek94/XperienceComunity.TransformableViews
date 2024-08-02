using CMS.Helpers;
using HBS.TransformableViews;
using HBS.Xperience.TransformableViews.Models;
using HBS.Xperience.TransformableViewsShared.Models;
using HBS.Xperience.TransformableViewsShared.Repositories;
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
    internal class TransformableViewFileProvider : IFileProvider
    {
        private readonly ITransformableViewRepository _repository;

        public TransformableViewFileProvider(ITransformableViewRepository serviceProvider) {
            _repository = serviceProvider;
        }
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
            return new TransformableViewFile(_repository, subpath);
        }

        /// <summary>
        /// Check for 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IChangeToken Watch(string filter)
        {
            return new TransformableViewChangeToken(_repository, filter);
        }
    }
}
