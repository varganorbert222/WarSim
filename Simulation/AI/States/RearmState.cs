namespace WarSim.Simulation.AI.States
{
    /// <summary>
    /// Rearm state: unit is heading to base/supply point to restock ammunition
    /// </summary>
    public class RearmState : AIState
    {
        public override string Name => "Rearm";

        public override void OnEnter(AIContext context)
        {
            context.Unit.Status = Domain.UnitStatus.Moving;

            // TODO: Find nearest friendly base/supply point
            // For now, just slow down and simulate rearming
            var slowSpeed = context.Unit switch
            {
                Domain.Units.AirUnit => 100.0,
                Domain.Units.LandUnit => 5.0,
                Domain.Units.SeaUnit => 10.0,
                _ => 5.0
            };

            context.PendingCommands.Add(new Commands.MoveCommand(
                context.Unit.Id,
                null,
                null,
                null,
                slowSpeed
            ));

            context.SetStateData("rearmStartTime", context.TimeInState);
        }

        public override void OnUpdate(AIContext context)
        {
            context.TimeInState += 0.1;

            // Simulate rearming over time (10 seconds to fully rearm)
            var rearmDuration = 10.0;
            if (context.TimeInState >= rearmDuration)
            {
                // Rearm complete - restore all ammo
                foreach (var weapon in context.Unit.WeaponSlots)
                {
                    // Reset to full ammo (will be populated by weapon config service)
                    weapon.CurrentAmmo = weapon.CurrentAmmo > 0 ? 300 : 0; // placeholder
                    weapon.CurrentMagazine = weapon.CurrentMagazine > 0 ? 30 : 0; // placeholder
                    weapon.IsReloading = false;
                }
            }
        }

        public override string? CheckTransitions(AIContext context)
        {
            // If rearming done -> Patrol
            if (context.TimeInState >= 10.0)
            {
                return "Patrol";
            }

            // If under attack while rearming -> Retreat
            var threats = GetNearestThreats(context);
            if (threats.Any() && context.Unit.Health / 100.0 < 0.5)
            {
                return "Retreat";
            }

            return null;
        }

        private List<Domain.Unit> GetNearestThreats(AIContext context)
        {
            return context.WorldSnapshot.Units
                .Where(u => u.FactionId != context.Unit.FactionId &&
                           u.Status != Domain.UnitStatus.Destroyed &&
                           DistanceMeters(context.Unit, u) <= context.Unit.VisionRangeMeters)
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
    }
}
