namespace SAF.ProcessEngine
{
    using System;
    using System.Linq;

    /// <summary>
    /// Represents a <see cref="ProcessStoppedEventArgs"/> event that triggers a state change.
    /// </summary>
    /// <typeparam name="T">The type that represents the possible states to trigger.</typeparam>
    public class ProcessStopStateTrigger<T> : StateTriggerBase<ProcessStoppedEventArgs, T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessStopStateTrigger&lt;T&gt;"/> class with the specified
        /// process, new state and additional conditions.
        /// </summary>
        /// <param name="newState">The state to trigger.</param>
        /// <param name="condition">The condition required to satisfy the trigger.</param>
        public ProcessStopStateTrigger(T newState, Func<object, ProcessStoppedEventArgs, bool> condition)
            : base(newState, condition)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessStopStateTrigger&lt;T&gt;"/> class with the specified
        /// process, new state and additional conditions.
        /// </summary>
        /// <param name="newState">The state to trigger.</param>
        /// <param name="processes">A list of valid <see cref="IProcessEngine"/> senders.</param>
        public ProcessStopStateTrigger(T newState, params IProcessEngine[] processes)
            : this(newState, (s, e) => processes.Any(x => ReferenceEquals(x, s)))
        {
        }
    }
}
