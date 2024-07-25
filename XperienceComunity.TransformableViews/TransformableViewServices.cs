using CMS.Core;
using HBS.Xperience.TransformableViews.Repositories;
using HBS.Xperience.TransformableViewsShared.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace HBS.Xperience.TransformableViews
{
    public static class TransformableViewServices
    {
        public static IMvcBuilder UseTransformableViewsProvider(this IMvcBuilder builder)
        {
            builder.Services.AddTransient<IFileProvider, TransformableViewFileProvider>(sp => {
                var provider = ActivatorUtilities.CreateInstance<TransformableViewFileProvider>(sp);
                return provider;
            });

            //builder.Services.AddTransient<IFileProvider>(sp => {
            //    var host = ActivatorUtilities.GetServiceOrCreateInstance<IWebHostEnvironment>(sp);
            //    return host.WebRootFileProvider;
            //});

            builder.Services
                .AddOptions<MvcRazorRuntimeCompilationOptions>()
                .Configure<IEnumerable<IFileProvider>>((options, providers) =>
                {
                    foreach (IFileProvider provider in providers)
                    {
                        options.FileProviders.Add(provider);
                    }
                });

            builder.AddRazorRuntimeCompilation();
            builder.Services.AddTransient<IContentItemRetriever, ContentItemRetriever>();

            return builder;
        }
    }
}
