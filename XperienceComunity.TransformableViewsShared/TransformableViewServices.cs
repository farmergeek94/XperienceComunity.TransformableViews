using CMS.Core;
using CMS.DataEngine;
using CMS.Modules;
using CommandLine;
using HBS.Xperience.TransformableViewsShared.Repositories;
using HBS.Xperience.TransformableViewsShared.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using XperienceComunity.TransformableViewsShared.Services;
using XperienceComunity.TransformableViewsTool;

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
            builder.Services.AddSingleton<IInstallTransformableViews,InstallTransformableViews>();
            builder.Services.AddTransient<IStartupFilter, TransformableViewsStartupFilter>();

            return builder;
        }

        [Verb("transformable-views-install", false, null, HelpText = "Installs the transformable module.")]
        public class TransformableOptions : TransformableOptionsDefault
        {
            public bool Install { get; set; } = true;
        }

        [Verb("transformable-views-default", false, null, HelpText = "Default Verb")]
        public class TransformableOptionsDefault
        {
        }

        public class TransformableViewsStartupFilter(IInstallTransformableViews installTransformableViews) : IStartupFilter
        {
            public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
            {
                return builder =>
                {
                    var args = Environment.GetCommandLineArgs().Skip(1);
                    var parser = new Parser(with => with.IgnoreUnknownArguments = true);
                    var result = parser.ParseArguments<TransformableOptionsDefault, TransformableOptions>(args);

                    var result2 = result.WithParsed(delegate (TransformableOptionsDefault opts)
                    {
                        if (opts is TransformableOptions parsedOpts)
                        {
                            if (parsedOpts.Install)
                            {
                                installTransformableViews.Install();
                            }
                        }
                    });
                    next(builder);
                };
            }
        }
    }
}
