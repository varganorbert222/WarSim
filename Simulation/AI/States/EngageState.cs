namespace WarSim.Simulation.AI.States
{
    /// <summary>
    /// Engage state: actively pursuing and attacking target
    /// </summary>
    public class EngageState : AIState
    {
        public override string Name => "Engage";

        public override void OnEnter(AIContext context)
        {
            context.Unit.Status = Domain.UnitStatus.InCombat;
            context.SetStateData("lastFireTime", 0.0);
        }

        public override void OnUpdate(AIContext context)
        {
            context.TimeInState += 0.1;

            if (context.CurrentTarget == null || context.CurrentTarget.Status == Domain.UnitStatus.Destroyed)
            {
                return;
            }

            var distance = DistanceMeters(context.Unit, context.CurrentTarget);
            var headingToTarget = HeadingTo(context.Unit, context.CurrentTarget);

            // Turn towards target
            context.PendingCommands.Add(new Commands.MoveCommand(
                context.Unit.Id,
                null,
                null,
                headingToTarget,
                null
            ));

            // Check if we can fire
            var lastFireTime = (double)(context.GetStateData<object>("lastFireTime") ?? 0.0);
            var timeSinceLastFire = context.TimeInState - lastFireTime;

            if (distance <= context.Behavior.EngageRange && timeSinceLastFire >= 1.0) // 1 second cooldown
            {
                // Try to fire (weapon system will check ammo)
                context.PendingCommands.Add(new Commands.FireCommand(
                    context.Unit.Id,
                    Domain.ProjectileType.Bullet, // TODO: get from weapon config
                    headingToTarget,
                    400.0,
                    10.0,
                    context.CurrentTarget.Latitude,
                    context.CurrentTarget.Longitude
                ));

                context.SetStateData("lastFireTime", context.TimeInState);
            }
        }

        public override string? CheckTransitions(AIContext context)
        {
            // Check health -> Retreat
            if (context.Unit.Health / 100.0 <= context.Behavior.RetreatHealthThreshold)
            {
                return "Retreat";
            }

            // Check if target lost or destroyed
            if (context.CurrentTarget == null || context.CurrentTarget.Status == Domain.UnitStatus.Destroyed)
            {
                context.CurrentTarget = null;
                return "Patrol";
            }

            // Check if target out of range (lost contact)
            var distance = DistanceMeters(context.Unit, context.CurrentTarget);
            if (distance > context.Unit.VisionRangeMeters * 1.5)
            {
                context.CurrentTarget = null;
                return "Patrol";
            }

            // TODO: Check ammo -> Rearm
            // if (context.Unit.GetAmmoPercent() < context.Behavior.RearmAmmoThreshold)
            //     return "Rearm";

            return null;
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

        private double HeadingTo(Domain.Unit from, Domain.Unit to)
        {
            var dy = to.Latitude - from.Latitude;
            var dx = to.Longitude - from.Longitude;
            var angle = Math.Atan2(dx, dy) * 180.0 / Math.PI;
            if (angle < 0) angle += 360.0;
            return angle;
        }
    }
}
