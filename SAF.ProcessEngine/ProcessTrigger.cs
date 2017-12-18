namespace SAF.ProcessEngine
{
    using System;

    /// <summary>
    /// Represents an event and corresponding state change for process events.
    /// </summary>
    /// <typeparam name="TEvent">The type of event the trigger represents.</typeparam>
    public class ProcessTrigger<TEvent> : StateTriggerBase<TEvent, ProcessState>
        where TEvent : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessTrigger&lt;TEvent&gt;"/> class with the specified new 
        /// <see cref="ProcessState"/> and event condition.
        /// </summary>
        /// <param name="newState">The state to trigger when the condition is fulfilled.</param>
        /// <param name="condition">The event that will trigger the state change.</param>
        public ProcessTrigger(ProcessState newState, Func<object, TEvent, bool> condition)
            : base(newState, condition)
        {
        }
    }
}
