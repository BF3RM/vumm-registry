using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VUModManagerRegistry.Common.Interfaces;

namespace VUModManagerRegistry.Services.S3
{
    [ExcludeFromCodeCoverage]
    public static class S3ServiceCollectionExtensions
    {
        public static IServiceCollection AddS3Storage(
            [NotNull] this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            serviceCollection.Configure<S3ModStorageOptions>(configuration);
            serviceCollection.AddSingleton<IModStorage, S3ModStorage>();

            return serviceCollection;
        }
    }
}