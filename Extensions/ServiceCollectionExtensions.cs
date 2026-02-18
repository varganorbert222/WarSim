using Microsoft.Extensions.DependencyInjection;
using WarSim.Services;

namespace WarSim.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register application services here
            services.AddSingleton<WorldStateService>();

            return services;
        }
    }
}
