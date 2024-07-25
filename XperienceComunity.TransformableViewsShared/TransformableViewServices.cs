using CMS.Core;
using HBS.Xperience.TransformableViewsShared.Repositories;
using HBS.Xperience.TransformableViewsShared.Services;
using Microsoft.Extensions.DependencyInjection;
using XperienceComunity.TransformableViewsShared.Services;

namespace HBS.Xperience.TransformableViewsShared
{
    public static class TransformableViewServices
    {
        /// <summary>
        /// Add the services required for the transformable views. 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="aesKey">Key to encrypt the views with.  Must be a valid aes key.</param>
        /// <param name="deleteViewsOnImport">Delete views on import.</param>
        /// <returns></returns>
        public static IMvcBuilder WithTransformableViews(this IMvcBuilder builder, string aesKey, bool deleteViewsOnImport = false)
        {
            builder.Services.AddSingleton<IEncryptionService>(new EncryptionService(aesKey));
            builder.Services.AddScoped<ICacheService, CacheService>();
            builder.Services.AddSingleton<ITransformableViewRepository, TransformableViewRepository>();
            builder.Services.AddSingleton<IViewSettingsService>(new ViewSettingsService(deleteViewsOnImport));

            return builder;
        }
    }
}
