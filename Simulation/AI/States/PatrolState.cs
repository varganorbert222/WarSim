namespace WarSim.Simulation.AI.States
{
    /// <summary>
    /// Patrol state: unit moves around patrol area scanning for enemies
    /// </summary>
    public class PatrolState : AIState
    {
        private readonly Random _rand = new();
        public override string Name => "Patrol";

        public override void OnEnter(AIContext context)
        {
            context.Unit.Status = Domain.UnitStatus.Moving;
            SetRandomPatrolHeading(context);
        }

        public override void OnUpdate(AIContext context)
        {
            context.TimeInState += 0.1;

            // Change patrol heading every 30 seconds
            if (context.TimeInState > 30.0)
            {
                SetRandomPatrolHeading(context);
                context.TimeInState = 0;
            }
        }

        public override string? CheckTransitions(AIContext context)
        {
            // Check for enemies
            var enemies = GetEnemiesInRange(context);
            if (enemies.Any())
            {
                context.CurrentTarget = enemies.OrderBy(e => DistanceMeters(context.Unit, e)).First();
                return "Engage";
            }

            // Check health
            if (context.Unit.Health / 100.0 <= context.Behavior.RetreatHealthThreshold)
            {
                return "Retreat";
            }

            return null;
        }

        private void SetRandomPatrolHeading(AIContext context)
        {
            var newHeading = _rand.NextDouble() * 360.0;
            var speed = context.Unit switch
            {
                Domain.Units.AirUnit => 150.0,
                Domain.Units.LandUnit => 10.0,
                Domain.Units.SeaUnit => 15.0,
                _ => 5.0
            };

            context.PendingCommands.Add(new Commands.MoveCommand(
                context.Unit.Id,
                null,
                null,
                newHeading,
                speed
            ));
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
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
