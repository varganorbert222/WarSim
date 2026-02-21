namespace WarSim.DTOs
{
    /// <summary>
    /// Simplified snapshot for client-side interpolation and prediction.
    /// Contains only the essential movement data.
    /// </summary>
    public class MovementSnapshotDto
    {
        public long Tick
        {
            get; set;
        }
        public DateTime Timestamp
        {
            get; set;
        }
        public List<UnitMovementDto> Units { get; set; } = new();
        public List<ProjectileMovementDto> Projectiles { get; set; } = new();
    }

    public class UnitMovementDto
    {
        public Guid Id
        {
            get; set;
        }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Subcategory { get; set; } = string.Empty;
        public double Latitude
        {
            get; set;
        }
        public double Longitude
        {
            get; set;
        }
        public double? Altitude
        {
            get; set;
        }
        public double Heading
        {
            get; set;
        }
        /// <summary>
        /// Speed in meters per second (normalized for all unit types)
        /// </summary>
        public double SpeedMps
        {
            get; set;
        }
        public string Status { get; set; } = string.Empty;
        public int FactionId
        {
            get; set;
        }
        public double Health
        {
            get; set;
        }
        public double VisionRange
        {
            get; set;
        }
        public double DirectionX
        {
            get; set;
        }
        public double DirectionY
        {
            get; set;
        }
        public double AmmoPercentage
        {
            get; set;
        }
    }

    public class ProjectileMovementDto
    {
        public Guid Id
        {
            get; set;
        }
        public double Latitude
        {
            get; set;
        }
        public double Longitude
        {
            get; set;
        }
        public double Heading
        {
            get; set;
        }
        /// <summary>
        /// Speed in meters per second
        /// </summary>
        public double SpeedMps
        {
            get; set;
        }
        public string Type { get; set; } = string.Empty;
        public Guid OwnerUnitId
        {
            get; set;
        }
    }
}
