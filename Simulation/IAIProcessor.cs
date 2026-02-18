using WarSim.Domain;
namespace WarSim.Simulation
{
    public interface IAIProcessor
    {
        /// <summary>
        /// Run AI logic for the given unit using the provided world snapshot.
        /// Returns a set of commands (move, fire, etc.) that the engine will execute.
        /// </summary>
        System.Collections.Generic.IEnumerable<Commands.ICommand> ProcessUnit(Unit unit, WorldState snapshot);
    }
}
