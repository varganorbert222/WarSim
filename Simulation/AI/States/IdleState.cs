namespace WarSim.Simulation.AI.States
{
    /// <summary>
    /// Idle state: unit is not doing anything, waiting for orders or targets
    /// </summary>
    public class IdleState : AIState
    {
        public override string Name => "Idle";

        public override void OnEnter(AIContext context)
        {
            context.Unit.Status = Domain.UnitStatus.Idle;
        }

        public override void OnUpdate(AIContext context)
        {
            // Scan for enemies periodically
            context.TimeInState += 0.1; // assuming 100ms tick
        }

        public override string? CheckTransitions(AIContext context)
        {
            // Check if enemies in range -> Engage
            var enemies = GetEnemiesInRange(context);
            if (enemies.Any() && context.Behavior.AggressionLevel > 0.3)
            {
                context.CurrentTarget = enemies.First();
                return "Engage";
            }

            // Check if should patrol
            if (context.TimeInState > 10.0 && context.Behavior.PatrolRadius > 0)
            {
                return "Patrol";
            }

            return null;
        }

        private List<Domain.Unit> GetEnemiesInRange(AIContext context)
        {
            return context.WorldSnapshot.Units
                .Where(u => u.FactionId != context.Unit.FactionId &&
                           u.Status != Domain.UnitStatus.Destroyed &&
                           DistanceMeters(context.Unit, u) <= context.Unit.VisionRangeMeters)
                .ToList();
        }

        private double DistanceMeters(Domain.Unit u1, Domain.Unit u2)
        {
            const double metersPerDegLat = 111320.0;
            var dy = (u2.Latitude - u1.Latitude) * metersPerDegLat;
            var avgLat = (u1.Latitude + u2.Latitude) / 2.0 * Math.PI / 180.0;
            var metersPerDegLon = metersPerDegLat * Math.Cos(avgLat);
            var dx = (u2.Longitude - u1.Longitude) * metersPerDegLon;
            return Math.Sqrt((dx * dx) + (dy * dy));
        }
    }
}
