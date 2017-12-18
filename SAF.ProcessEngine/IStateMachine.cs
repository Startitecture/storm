namespace SAF.ProcessEngine
{
    using System;

    /// <summary>
    /// Provides an interface to a <see cref="StateMachineBase&lt;TState&gt;"/>.
    /// </summary>
    /// <typeparam name="TState">The type that represents the possible states of the state machine.</typeparam>
    public interface IStateMachine<TState>
    {
        /// <summary>
        /// Occurs when the state is changing.
        /// </summary>
        event EventHandler<StateChangingEventArgs<TState>> StateChanging;

        /// <summary>
        /// Occurs when the state changes.
        /// </summary>
        event EventHandler<StateChangedEventArgs<TState>> StateChanged;

        /// <summary>
        /// Gets the current state of the state machine.
        /// </summary>
        TState State { get; }
    }
}
