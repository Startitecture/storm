namespace SAF.ProcessEngine
{
    /// <summary>
    /// Provides an interface to a <see cref="StateTriggerBase{TEvent,TState}"/>.
    /// </summary>
    /// <typeparam name="TState">The type that represents the possible states that can be triggered.</typeparam>
    public interface IStateTrigger<TState> : IEventTrigger
    {
        /// <summary>
        /// Gets the state triggered by the associated event.
        /// </summary>
        TState NewState { get; }
    }
}
