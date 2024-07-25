using HBS.Xperience.TransformableViewsShared.Repositories;
using HBS.Xperience.TransformableViewsShared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HBS.Xperience.TransformableViewsShared
{
    public static class TransformableViewServices
    {
        public static IMvcBuilder WithTransformableViews(this IMvcBuilder builder, string aesKey)
        {
            builder.Services.AddSingleton<IEncryptionService>(new EncryptionService(aesKey));
            builder.Services.AddScoped<ICacheService, CacheService>();
            builder.Services.AddSingleton<ITransformableViewRepository, TransformableViewRepository>();

            return builder;
        }
    }
}
