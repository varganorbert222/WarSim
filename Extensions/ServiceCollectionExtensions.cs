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
            // Simulation engine and hosted background service
            services.AddSingleton<WarSim.Simulation.ISimulationEngine, WarSim.Simulation.SimulationEngine>();
            services.AddSingleton<WarSim.Simulation.IEntityProcessor, WarSim.Simulation.EntityProcessor>();
            services.AddSingleton<WarSim.Simulation.IAIProcessor, WarSim.Simulation.AI.SimpleAIProcessor>();
            services.AddSingleton<WarSim.Simulation.IWeaponSystem, WarSim.Simulation.Weapons.SimpleWeaponSystem>();
            services.AddSingleton<WarSim.Simulation.Weapons.WeaponFactory>();
            services.AddSingleton<WarSim.Services.FactionService>();
            services.AddHostedService<WarSim.Services.SimulationHostedService>();

            return services;
        }
    }
}
