using WarSim.Domain;
using WarSim.Simulation.AI.States;

namespace WarSim.Simulation.AI
{
    /// <summary>
    /// AI State machine manager. Tracks current state per unit and processes transitions.
    /// </summary>
    public class AIStateMachine
    {
        private readonly Dictionary<string, AIState> _states = new();
        private readonly Dictionary<Guid, (AIState current, AIContext context)> _unitStates = new();
        private readonly ILogger<AIStateMachine> _logger;

        public AIStateMachine(ILogger<AIStateMachine> logger)
        {
            _logger = logger;
            RegisterStates();
        }

        private void RegisterStates()
        {
            RegisterState(new IdleState());
            RegisterState(new PatrolState());
            RegisterState(new EngageState());
            RegisterState(new RetreatState());
            RegisterState(new RearmState());
        }

        private void RegisterState(AIState state)
        {
            _states[state.Name] = state;
        }

        public void InitializeUnit(Unit unit, AIBehaviorConfig behavior, WorldState snapshot)
        {
            var initialStateName = behavior.InitialState;
            if (!_states.TryGetValue(initialStateName, out var initialState))
            {
                initialState = _states["Idle"];
            }

            var context = new AIContext(unit, snapshot, behavior);
            _unitStates[unit.Id] = (initialState, context);
            initialState.OnEnter(context);
        }

        public List<Commands.ICommand> ProcessUnit(Unit unit, WorldState snapshot, AIBehaviorConfig behavior)
        {
            if (!_unitStates.TryGetValue(unit.Id, out var stateInfo))
            {
                InitializeUnit(unit, behavior, snapshot);
                stateInfo = _unitStates[unit.Id];
            }

            var (currentState, context) = stateInfo;

            // Update context with latest data
            context.Unit = unit;
            context.WorldSnapshot = snapshot;
            context.PendingCommands.Clear();

            // Update current state
            currentState.OnUpdate(context);

            // Check for transitions
            var nextStateName = currentState.CheckTransitions(context);
            if (nextStateName != null && _states.TryGetValue(nextStateName, out var nextState))
            {
                Logging.ConsoleColorLogger.Log("AI.StateMachine", Microsoft.Extensions.Logging.LogLevel.Debug,
                    $"Unit {unit.Name} transition: {currentState.Name} → {nextStateName}");

                currentState.OnExit(context);
                context.LastStateName = currentState.Name;
                context.TimeInState = 0;
                nextState.OnEnter(context);
                _unitStates[unit.Id] = (nextState, context);
            }

            return context.PendingCommands;
        }

        public string? GetUnitStateName(Guid unitId)
        {
            return _unitStates.TryGetValue(unitId, out var stateInfo) ? stateInfo.current.Name : null;
        }
    }
}
