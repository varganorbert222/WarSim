using System.Text.Json;
using WarSim.Domain;

namespace WarSim.Simulation.AI
{
    /// <summary>
    /// State machine-based AI processor. Replaces SimpleAIProcessor.
    /// </summary>
    public class StateMachineAIProcessor : IAIProcessor
    {
        private readonly AIStateMachine _stateMachine;
        private readonly ILogger<StateMachineAIProcessor> _logger;
        private readonly Dictionary<string, AIBehaviorConfig> _behaviors = new();

        public StateMachineAIProcessor(ILogger<StateMachineAIProcessor> logger)
        {
            _logger = logger;
            _stateMachine = new AIStateMachine(_logger as ILogger<AIStateMachine> ??
                Microsoft.Extensions.Logging.Abstractions.NullLogger<AIStateMachine>.Instance);
            LoadBehaviors();
        }

        private void LoadBehaviors()
        {
            try
            {
                var path = Path.Combine("Data", "Configs", "ai-behaviors.json");
                var json = File.ReadAllText(path);
                var config = JsonSerializer.Deserialize<AIBehaviorConfigFile>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (config?.Behaviors != null)
                {
                    foreach (var behavior in config.Behaviors)
                    {
                        _behaviors[behavior.UnitType] = behavior;
                    }
                    _logger.LogInformation($"Loaded {_behaviors.Count} AI behavior configs");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load AI behavior configs");
            }
        }

        public IEnumerable<Commands.ICommand> ProcessUnit(Unit unit, WorldState snapshot)
        {
            // Get behavior config for this unit type
            var behavior = GetBehaviorForUnit(unit);

            // Process through state machine
            return _stateMachine.ProcessUnit(unit, snapshot, behavior);
        }

        private AIBehaviorConfig GetBehaviorForUnit(Unit unit)
        {
            // Try subcategory first
            if (_behaviors.TryGetValue(unit.Subcategory, out var behavior))
            {
                return behavior;
            }

            // Fallback to category
            if (_behaviors.TryGetValue(unit.UnitCategory.ToString(), out behavior))
            {
                return behavior;
            }

            // Default behavior
            return new AIBehaviorConfig
            {
                UnitType = "Default",
                InitialState = "Idle",
                AggressionLevel = 0.5,
                EngageRange = 2000.0,
                RetreatHealthThreshold = 0.3,
                PatrolRadius = 5000.0,
                RearmAmmoThreshold = 0.2
            };
        }

        private class AIBehaviorConfigFile
        {
            public List<AIBehaviorConfig> Behaviors { get; set; } = new();
        }
    }
}
