using WarSim.Domain;

namespace WarSim.Simulation.AI
{
    /// <summary>
    /// Context object passed to AI states containing unit and world information
    /// </summary>
    public class AIContext
    {
        public Unit Unit { get; set; }
        public WorldState WorldSnapshot { get; set; }
        public AIBehaviorConfig Behavior { get; set; }
        public List<Commands.ICommand> PendingCommands { get; set; } = new();
        
        // State machine runtime data
        public Dictionary<string, object> StateData { get; set; } = new();
        public Unit? CurrentTarget { get; set; }
        public double TimeInState { get; set; }
        public string? LastStateName { get; set; }

        public AIContext(Unit unit, WorldState snapshot, AIBehaviorConfig behavior)
        {
            Unit = unit;
            WorldSnapshot = snapshot;
            Behavior = behavior;
        }

        public T? GetStateData<T>(string key) where T : class
        {
            return StateData.TryGetValue(key, out var value) ? value as T : null;
        }

        public void SetStateData(string key, object value)
        {
            StateData[key] = value;
        }
    }

    /// <summary>
    /// AI behavior configuration loaded from JSON
    /// </summary>
    public class AIBehaviorConfig
    {
        public string UnitType { get; set; } = string.Empty;
        public string InitialState { get; set; } = "Idle";
        public double AggressionLevel { get; set; } = 0.5;
        public double EngageRange { get; set; } = 2000.0;
        public double RetreatHealthThreshold { get; set; } = 0.3;
        public double PatrolRadius { get; set; } = 5000.0;
        public double RearmAmmoThreshold { get; set; } = 0.2;
        public Dictionary<string, StateTransitionConfig> Transitions { get; set; } = new();
    }

    public class StateTransitionConfig
    {
        public string FromState { get; set; } = string.Empty;
        public string ToState { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public double Priority { get; set; } = 1.0;
    }
}
