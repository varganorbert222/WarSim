namespace WarSim.Domain
{
    public class Faction
    {
        public int Id
        {
            get; set;
        }
        public string Name { get; set; } = string.Empty;
        // Hex color (e.g. "#FF0000") for UI/visualization
        public string Color { get; set; } = "#FFFFFF";

        // List of allied faction ids (includes self id if desired)
        public List<int> Allies { get; set; } = new();

        public bool IsAlliedWith(int otherFactionId)
        {
            if (Allies == null)
            {
                return false;
            }

            return Allies.Contains(otherFactionId);
        }
    }
}
