using WarSim.Domain;
using WarSim.Domain.Projectiles;
using WarSim.Domain.Units;
using WarSim.Logging;

namespace WarSim.Simulation
{
    /// <summary>
    /// Simple simulation engine that performs movement updates for units and projectiles.
    /// It follows a snapshot-update-swap approach: reads a snapshot, computes a new snapshot and updates the world state.
    /// Designed to be extended with combat, AI, frontlines, and more.
    /// </summary>
    public class SimulationEngine : ISimulationEngine
    {
        private readonly Services.WorldStateService _world;
        private readonly ILogger<SimulationEngine> _logger;
        private readonly IEntityProcessor _processor;
        private readonly IAIProcessor _ai;
        private readonly IWeaponSystem _weapons;
        private readonly WarSim.Simulation.Weapons.WeaponFactory _weaponFactory;

        public SimulationEngine(Services.WorldStateService world, ILogger<SimulationEngine> logger, IEntityProcessor processor, IAIProcessor ai, IWeaponSystem weapons, WarSim.Simulation.Weapons.WeaponFactory weaponFactory)
        {
            _world = world;
            _logger = logger;
            _processor = processor;
            _ai = ai;
            _weapons = weapons;
            _weaponFactory = weaponFactory;
        }

        public Task TickAsync(double deltaSeconds)
        {
            // Get immutable snapshot
            var snapshot = _world.GetSnapshot();

            ConsoleColorLogger.Log("SimulationEngine", LogLevel.Information, $"⏱️ Starting tick {snapshot.Tick} with {snapshot.Units.Count} units and {snapshot.Projectiles.Count} projectiles");

            var units = snapshot.Units.Select(u => _processor.CloneUnit(u)).ToList();
            var projectiles = snapshot.Projectiles.Select(p => _processor.CloneProjectile(p)).ToList();


            var newProjectiles = new System.Collections.Concurrent.ConcurrentBag<Projectile>();
            var newCommands = new System.Collections.Concurrent.ConcurrentBag<Commands.ICommand>();

            // Parallel update units
            _ = Parallel.ForEach(units, unit =>
            {
                try
                {

                    // First let AI make decisions (may return commands)
                    var cmds = _ai.ProcessUnit(unit, snapshot);
                    if (cmds != null)
                    {
                        foreach (var c in cmds)
                        {
                            newCommands.Add(c);
                        }
                    }

                    // Movement update based on possibly modified unit state
                    _processor.UpdateUnitMovement(unit, deltaSeconds);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating unit {UnitId}", unit.Id);
                    ConsoleColorLogger.Log("SimulationEngine", LogLevel.Error, $"Error updating unit {unit.Id}: {ex.Message}");
                }
            });

            // Merge newly fired projectiles into the projectile list
            projectiles.AddRange(newProjectiles);

            // Collect and process commands produced by AI
            // Parallel update projectiles
            _ = Parallel.ForEach(projectiles, proj =>
            {
                try
                {
                    _processor.UpdateProjectileMovement(proj, deltaSeconds);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating projectile {ProjectileId}", proj.Id);
                    ConsoleColorLogger.Log("SimulationEngine", LogLevel.Error, $"Error updating projectile {proj.Id}: {ex.Message}");
                }
            });

            // Process commands (move/fire) after unit updates
            while (!newCommands.IsEmpty)
            {
                if (!newCommands.TryTake(out var cmd))
                {
                    break;
                }

                switch (cmd)
                {
                    case Commands.MoveCommand mc:
                    {
                        var target = units.FirstOrDefault(u => u.Id == mc.UnitId);
                        if (target != null)
                        {
                            if (mc.Heading.HasValue)
                            {
                                target.Heading = mc.Heading.Value;
                            }

                            if (mc.Latitude.HasValue)
                            {
                                target.Latitude = mc.Latitude.Value;
                            }

                            if (mc.Longitude.HasValue)
                            {
                                target.Longitude = mc.Longitude.Value;
                            }

                            if (mc.Speed.HasValue)
                            {
                                switch (target)
                                {
                                    case LandUnit lu:
                                        lu.GroundSpeed = mc.Speed;
                                        break;
                                    case AirUnit au:
                                        au.Airspeed = mc.Speed;
                                        break;
                                    case SeaUnit su:
                                        su.SpeedKnots = mc.Speed;
                                        break;
                                }
                            }
                            target.Status = UnitStatus.Moving;
                        }
                        break;
                    }
                    case Commands.FireCommand fc:
                    {
                        var shooter = units.FirstOrDefault(u => u.Id == fc.UnitId);
                        var proj = _weaponFactory.CreateProjectile(fc, shooter?.Latitude ?? 0.0, shooter?.Longitude ?? 0.0);
                        projectiles.Add(proj);
                        break;
                    }
                }
            }

            // After processing commands, check projectile collisions and apply effects
            // compute aggregates for summary
            var originalUnitPositions = snapshot.Units.ToDictionary(u => u.Id, u => (u.Latitude, u.Longitude));
            var originalProjectileIds = new HashSet<Guid>(snapshot.Projectiles.Select(p => p.Id));
            var originalDestroyed = snapshot.Units.Count(u => u.Status == UnitStatus.Destroyed);

            projectiles = _weapons.ProcessProjectiles(projectiles, units, snapshot.Factions);

            var newState = new WorldState(units, projectiles, snapshot.Factions, snapshot.Tick + 1);
            _world.UpdateState(newState);

            // summary
            var movedCount = units.Count(u => originalUnitPositions.TryGetValue(u.Id, out var pos) && (Math.Abs(pos.Latitude - u.Latitude) > 1e-6 || Math.Abs(pos.Longitude - u.Longitude) > 1e-6));
            var firedCount = projectiles.Count(p => !originalProjectileIds.Contains(p.Id));
            var destroyedNow = units.Count(u => u.Status == UnitStatus.Destroyed);
            var destroyedDelta = Math.Max(0, destroyedNow - originalDestroyed);

            ConsoleColorLogger.Log("SimulationEngine", LogLevel.Information, $"✅ Tick {snapshot.Tick} → {newState.Tick} complete: {movedCount} moved, {firedCount} fired, {destroyedDelta} destroyed");

            return Task.CompletedTask;
        }
    }
}
