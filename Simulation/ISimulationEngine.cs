using System.Threading.Tasks;

namespace WarSim.Simulation
{
    public interface ISimulationEngine
    {
        Task TickAsync(double deltaSeconds);
    }
}
