using WarSim.Domain;

namespace WarSim.Domain.Units
{
    public class Helicopter : AirUnit
    {
        public string Role { get; set; } = string.Empty; // e.g., transport, attack, recon

        public Helicopter()
        {
        }
    }
}
