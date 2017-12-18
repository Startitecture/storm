namespace SAF.ProcessEngine
{
    using System;

    /// <summary>
    /// Contains data associated with a state changing event.
    /// </summary>
    /// <typeparam name="TState">The type that represents the possible states of the state machine.</typeparam>
    public class StateChangingEventArgs<TState> : StateChangeEventArgs<TState>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateChangingEventArgs&lt;TState&gt;"/> class.
        /// </summary>
        /// <param name="stateChange">The <see cref="StateChange&lt;T&gt;"/> associated with the state change 
        /// event.</param>
        /// <param name="error">The error, if any, associated with the state change.</param>
        [System.Diagnostics.DebuggerHidden]
        public StateChangingEventArgs(StateChange<TState> stateChange, Exception error)
            : base(stateChange, error)
        {
        }
    }
}
