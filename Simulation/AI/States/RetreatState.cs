namespace WarSim.Simulation.AI.States
{
    /// <summary>
    /// Retreat state: unit is damaged and moving away from danger
    /// </summary>
    public class RetreatState : AIState
    {
        public override string Name => "Retreat";

        public override void OnEnter(AIContext context)
        {
            context.Unit.Status = Domain.UnitStatus.Retreating;

            // Calculate retreat direction (away from nearest threat)
            var threats = GetNearestThreats(context);
            if (threats.Any())
            {
                var nearest = threats.First();
                var headingAway = HeadingTo(nearest, context.Unit);

                var maxSpeed = context.Unit switch
                {
                    Domain.Units.AirUnit => 300.0,
                    Domain.Units.LandUnit => 20.0,
                    Domain.Units.SeaUnit => 25.0,
                    _ => 10.0
                };

                context.PendingCommands.Add(new Commands.MoveCommand(
                    context.Unit.Id,
                    null,
                    null,
                    headingAway,
                    maxSpeed
                ));
            }
        }

        public override void OnUpdate(AIContext context)
        {
            context.TimeInState += 0.1;
        }

        public override string? CheckTransitions(AIContext context)
        {
            // If health recovered somewhat -> Patrol
            if (context.Unit.Health / 100.0 > context.Behavior.RetreatHealthThreshold + 0.2)
            {
                return "Patrol";
            }

            // If no threats nearby and retreated long enough -> Idle
            var threats = GetNearestThreats(context);
            if (!threats.Any() && context.TimeInState > 20.0)
            {
                return "Idle";
            }

            return null;
        }

        private List<Domain.Unit> GetNearestThreats(AIContext context)
        {
            return context.WorldSnapshot.Units
                .Where(u => u.FactionId != context.Unit.FactionId &&
                           u.Status != Domain.UnitStatus.Destroyed &&
                           DistanceMeters(context.Unit, u) <= context.Unit.VisionRangeMeters * 2)
                .OrderBy(u => DistanceMeters(context.Unit, u))
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

        private double HeadingTo(Domain.Unit from, Domain.Unit to)
        {
            var dy = to.Latitude - from.Latitude;
            var dx = to.Longitude - from.Longitude;
            var angle = Math.Atan2(dx, dy) * 180.0 / Math.PI;
            if (angle < 0)
            {
                angle += 360.0;
            }

            return angle;
        }
    }
}
