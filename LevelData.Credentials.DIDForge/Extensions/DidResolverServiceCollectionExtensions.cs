using LevelData.Credentials.DIDForge.Abstractions;
using LevelData.Credentials.DIDForge.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LevelData.Credentials.DIDForge.Extensions
{
    public static class DidResolverServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the DID resolvers and the DidResolver class to the DI container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddDidResolvers(this IServiceCollection services)
        {
            // Register individual IDidResolver implementations
            services.AddScoped<DidKeyResolver>();
            services.AddScoped<DidWebResolver>();

            // Register the DidResolver class
            services.AddScoped<DidResolver>();

            return services;
        }
    }
}
