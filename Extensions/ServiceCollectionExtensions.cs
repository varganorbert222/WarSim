using WarSim.Services;

namespace WarSim.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register application services here
            _ = services.AddSingleton<WorldStateService>();
            // Simulation engine and hosted background service
            _ = services.AddSingleton<WarSim.Simulation.ISimulationEngine, WarSim.Simulation.SimulationEngine>();
            _ = services.AddSingleton<WarSim.Simulation.IEntityProcessor, WarSim.Simulation.EntityProcessor>();
            _ = services.AddSingleton<WarSim.Simulation.IAIProcessor, WarSim.Simulation.AI.SimpleAIProcessor>();
            _ = services.AddSingleton<WarSim.Simulation.IWeaponSystem, WarSim.Simulation.Weapons.SimpleWeaponSystem>();
            _ = services.AddSingleton<WarSim.Simulation.Weapons.WeaponFactory>();
            _ = services.AddSingleton<WarSim.Services.FactionService>();
            _ = services.AddHostedService<WarSim.Services.SimulationHostedService>();

            return services;
        }
    }
}
