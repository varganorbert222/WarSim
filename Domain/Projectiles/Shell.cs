using WarSim.Domain.Projectiles;

namespace WarSim.Domain.Projectiles
{
    public class Shell : Projectile
    {
        public double ShellMassKg { get; set; }

        public override string Type => "Shell";
    }
}
