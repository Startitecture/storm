namespace SAF.ProcessEngine
{
    using System;

    /// <summary>
    /// Contains data associated with a state change event.
    /// </summary>
    /// <typeparam name="T">The type that represents the possible states of the state machine.</typeparam>
    public class StateChangeEventArgs<T> : EventArgs, IErrorEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateChangeEventArgs&lt;T&gt;"/> class, with the specified
        /// state change.
        /// </summary>
        /// <param name="stateChange">The state change associated with the state change event.</param>
        /// <param name="error">The error, if any, associated with the state change.</param>
        [System.Diagnostics.DebuggerHidden]
        protected StateChangeEventArgs(StateChange<T> stateChange, Exception error)
        {
            this.StateChange = stateChange;
            this.EventError = error;
        }

        /// <summary>
        /// Gets the <see cref="T:SAF.ProcessEngine.StateChange`1"/> associated with state change event.
        /// </summary>
        public StateChange<T> StateChange
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the exception, if any, associated with the state change event.
        /// </summary>
        public Exception EventError
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of the <see cref="StateChangeEventArgs&lt;T&gt;"/> 
        /// object.
        /// </summary>
        /// <returns>A <see cref="System.String"/> representation of the <see cref="StateChangeEventArgs&lt;T&gt;"/>
        /// object.</returns>
        public override string ToString()
        {
            return String.Format(
                "{0}{1}", this.StateChange, this.EventError == null ? String.Empty : this.EventError.Message);
        }
    }
}
