namespace WarSim.Domain
{
    public class Unit
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
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
        public UnitStatus Status { get; set; } = UnitStatus.Idle;
    }
}
