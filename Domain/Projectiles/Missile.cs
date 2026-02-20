namespace WarSim.Domain.Projectiles
{
    public class Missile : Projectile
    {
        public double GuidanceRangeMeters
        {
            get; set;
        }

        public override string Type => "Missile";
    }
}
