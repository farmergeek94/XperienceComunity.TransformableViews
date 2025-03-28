using CMS.Core;
using HBS.Xperience.TransformableViews.Repositories;
using HBS.Xperience.TransformableViewsShared.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
            // Add the transformable view file provider
            builder.Services.AddTransient<IFileProvider, TransformableViewFileProvider>();

            //builder.Services.AddTransient<IFileProvider>(sp =>
            //{
            //    var host = ActivatorUtilities.GetServiceOrCreateInstance<IWebHostEnvironment>(sp);
            //    return host.WebRootFileProvider;
            //});

            // Add the file provider to the MVC Razor Runtime Compilation
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
