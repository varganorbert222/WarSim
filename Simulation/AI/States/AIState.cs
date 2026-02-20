namespace WarSim.Simulation.AI
{
    /// <summary>
    /// Base class for all AI states. Uses classic State pattern.
    /// </summary>
    public abstract class AIState
    {
        public abstract string Name { get; }

        /// <summary>
        /// Called when entering this state
        /// </summary>
        public virtual void OnEnter(AIContext context) { }

        /// <summary>
        /// Called every tick while in this state
        /// </summary>
        public virtual void OnUpdate(AIContext context) { }

        /// <summary>
        /// Called when exiting this state
        /// </summary>
        public virtual void OnExit(AIContext context) { }

        /// <summary>
        /// Check if we should transition to another state
        /// </summary>
        public abstract string? CheckTransitions(AIContext context);
    }
}
