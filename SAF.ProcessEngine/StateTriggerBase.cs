namespace SAF.ProcessEngine
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Represents a condition that causes a state change.
    /// </summary>
    /// <typeparam name="TEvent">The type of event that causes the state change.</typeparam>
    /// <typeparam name="TState">The type of state that is changed.</typeparam>
    public class StateTriggerBase<TEvent, TState> : EventTrigger<TEvent>, IStateTrigger<TState>
        where TEvent : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateTriggerBase&lt;TEvent, TState&gt;"/> class with the specified 
        /// state and condition.
        /// </summary>
        /// <param name="newState">The new state of the state machine.</param>
        /// <param name="condition">The condition required to satisfy the trigger.</param>
        [DebuggerHidden]
        protected StateTriggerBase(TState newState, Func<object, TEvent, bool> condition)
            : base(condition)
        {
            this.NewState = newState;
        }

        /// <summary>
        /// Gets the new state of the process.
        /// </summary>
        public TState NewState { get; private set; }
    }
}
