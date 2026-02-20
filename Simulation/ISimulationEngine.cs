namespace WarSim.Simulation
{
    public interface ISimulationEngine
    {
        Task TickAsync(double deltaSeconds);
    }
}
