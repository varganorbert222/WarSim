namespace WarSim.Domain.Projectiles
{
    public abstract class Projectile
    {
        protected Projectile()
        {
            Id = Guid.NewGuid();
        }

        // Allow setting Id when creating snapshot clones
        public Guid Id
        {
            get; set;
        }

        /// <summary>
        /// Latitude of the projectile.
        /// </summary>
        public double Latitude
        {
            get; set;
        }

        /// <summary>
        /// Longitude of the projectile.
        /// </summary>
        public double Longitude
        {
            get; set;
        }

        /// <summary>
        /// Speed in m/s.
        /// </summary>
        public double Speed
        {
            get; set;
        }

        /// <summary>
        /// Heading in degrees.
        /// </summary>
        public double Heading
        {
            get; set;
        }

        /// <summary>
        /// Damage potential.
        /// </summary>
        public double Damage
        {
            get; set;
        }

        /// <summary>
        /// Owner unit id (who fired it).
        /// </summary>
        public Guid OwnerUnitId
        {
            get; set;
        }

        public abstract string Type
        {
            get;
        }
    }
}
