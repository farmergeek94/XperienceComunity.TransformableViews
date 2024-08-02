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
            builder.Services.AddSingleton<ITransformableViewsCommandLine,TransformableViewsCommandLine>();
            builder.Services.AddTransient<IStartupFilter, TransformableViewsStartupFilter>();

            return builder;
        }

        [Verb("transformable-views-install", false, null, HelpText = "Installs the transformable module.")]
        public class TransformableInstall : TransformableOptionsDefault
        {
        }

        [Verb("transformable-views-export", false, null, HelpText = "Export the transformable views.")]
        public class TransformableExport : TransformableOptionsDefault
        {
            [Option("location", Required = false, HelpText = "The absolute or relative path of the target file where the export zip will be placed.")]
            public string Export { get; set; } = "./TransformableViews_Export.zip";
        }

        [Verb("transformable-views-import", false, null, HelpText = "Import the transformable views.")]
        public class TransformableImport : TransformableOptionsDefault
        {
            [Option("location", Required = false, HelpText = "The absolute or relative path of the target file where the export zip will be placed.")]
            public string Import { get; set; } = "./TransformableViews_Export.zip";
        }

        public class TransformableOptionsDefault
        {
        }

        public class TransformableViewsStartupFilter(ITransformableViewsCommandLine commandLine) : IStartupFilter
        {
            public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
            {
                return builder =>
                {
                    var args = Environment.GetCommandLineArgs().Skip(1);
                    var parser = new Parser(with => with.IgnoreUnknownArguments = true);
                    parser.ParseArguments<TransformableInstall, TransformableExport, TransformableImport>(args)
                    .WithParsed(delegate (TransformableOptionsDefault opts)
                    {
                        if (opts is TransformableInstall parsedOpts)
                        {
                            commandLine.Install();
                        } else if(opts is TransformableExport expt)
                        {
                            commandLine.CreateJsonLoadFile(expt.Export);
                        } else if(opts is TransformableImport import)
                        {
                            commandLine.ReadJsonLoadFile(import.Import);
                        }
                    });
                    next(builder);
                };
            }
        }
    }
}
