using System;
using WarSim.Domain;

namespace WarSim.Simulation.AI
{
    public class SimpleAIProcessor : IAIProcessor
    {
        private readonly Random _rand = new();

        public System.Collections.Generic.IEnumerable<Commands.ICommand> ProcessUnit(Unit unit, WorldState snapshot)
        {
            var list = new System.Collections.Generic.List<Commands.ICommand>();

            // If idle, issue a move command with random heading and default speed
            if (unit.Status == UnitStatus.Idle)
            {
                var heading = _rand.NextDouble() * 360.0;
                // request to move with small speed
                list.Add(new Commands.MoveCommand(unit.Id, null, null, heading, 5.0));
            }
            else if (unit.Status == UnitStatus.Moving)
            {
                // small random turn
                var newHeading = (unit.Heading + (_rand.NextDouble() - 0.5) * 5.0) % 360.0;
                if (newHeading < 0) newHeading += 360.0;
                list.Add(new Commands.MoveCommand(unit.Id, null, null, newHeading, null));
            }

            // Target selection: choose nearest enemy within vision range
            var enemies = snapshot.Units.Where(u => u.FactionId != unit.FactionId && u.Status != UnitStatus.Destroyed).ToList();
            Unit? chosen = null;
            double bestDist = double.MaxValue;
            foreach (var e in enemies)
            {
                var d = DistanceMeters(unit.Latitude, unit.Longitude, e.Latitude, e.Longitude);
                if (d <= unit.VisionRangeMeters && d < bestDist)
                {
                    bestDist = d;
                    chosen = e;
                }
            }

            if (chosen != null)
            {
                // Point heading towards target
                var headingTo = HeadingTo(unit.Latitude, unit.Longitude, chosen.Latitude, chosen.Longitude);
                list.Add(new Commands.MoveCommand(unit.Id, null, null, headingTo, null));

                // Fire if within a convenient range
                if (bestDist <= 2000.0) // arbitrary firing range
                {
                    list.Add(new Commands.FireCommand(unit.Id, ProjectileType.Bullet, headingTo, 400.0, 10.0, chosen.Latitude, chosen.Longitude));
                }
            }

            return list;
        }

        private static double DistanceMeters(double lat1, double lon1, double lat2, double lon2)
        {
            const double metersPerDegLat = 111320.0;
            var dy = (lat2 - lat1) * metersPerDegLat;
            var avgLat = (lat1 + lat2) / 2.0 * Math.PI / 180.0;
            var metersPerDegLon = metersPerDegLat * Math.Cos(avgLat);
            var dx = (lon2 - lon1) * metersPerDegLon;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private static double HeadingTo(double lat1, double lon1, double lat2, double lon2)
        {
            var dy = lat2 - lat1;
            var dx = lon2 - lon1;
            var angle = Math.Atan2(dx, dy) * 180.0 / Math.PI; // note: swapped to match previous conventions
            if (angle < 0) angle += 360.0;
            return angle;
        }
    }
}
