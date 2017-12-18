namespace SAF.ProcessEngine
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Represents a state change in a state machine.
    /// </summary>
    /// <typeparam name="T">The type that represents the possible states of the state machine.</typeparam>
    public class StateChange<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateChange&lt;T&gt;"/> class with the specified previous 
        /// state, the trigger that triggered the state change and the exception (if any) associated with the state 
        /// change.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventTrigger">The event that caused the state change.</param>
        /// <param name="newState">The new state of the process.</param>
        [DebuggerHidden]
        public StateChange(object sender, EventArgs eventTrigger, T newState)
        {
            this.Sender = sender;
            this.NewState = newState;
            this.EventData = eventTrigger;
        }

        /// <summary>
        /// Gets the sender of the event trigger.
        /// </summary>
        public object Sender { get; private set; }

        /// <summary>
        /// Gets event data associated with the event trigger.
        /// </summary>
        public EventArgs EventData { get; private set; }

        /// <summary>
        /// Gets the new state of the state machine.
        /// </summary>
        public T NewState { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this <see cref="StateChange&lt;T&gt;"/>.
        /// </summary>
        /// <returns>a <see cref="System.String"/> representation of this <see cref="StateChange&lt;T&gt;"/>.</returns>
        public override string ToString()
        {
            return String.Format("{0} -> {1}", this.EventData, this.NewState);
        }
    }
}
