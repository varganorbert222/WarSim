namespace WarSim.Domain.Projectiles
{
    public class Bullet : Projectile
    {
        public double CaliberMm
        {
            get; set;
        }

        public override string Type => "Bullet";
    }
}
