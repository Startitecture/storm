namespace SAF.ProcessEngine
{
    using System;

    /// <summary>
    /// Encapsulates a state changing event condition that should trigger execution of a command.
    /// </summary>
    /// <typeparam name="T">The type that represents the possible states of the trigger.</typeparam>
    public class StateChangingCommandTrigger<T> : CommandTrigger<StateChangingEventArgs<T>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateChangingCommandTrigger&lt;T&gt;"/> class with the
        /// specified command, sender, and state.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="state">The state to trigger on.</param>
        public StateChangingCommandTrigger(IExecutable command, IStateMachine<T> sender, T state)
            : base(command, (s, e) => CheckBaseCondition(s, e, sender, state))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateChangingCommandTrigger&lt;T&gt;"/> class with the
        /// specified command, sender, state and additional trigger conditions.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="state">The state to trigger on.</param>
        /// <param name="additionalCondition">Any additional conditions required for execution.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", 
            "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "The caller can use lambda expressions for simplified use of the parameter.")]
        public StateChangingCommandTrigger(
            IExecutable command, 
            IStateMachine<T> sender, 
            T state,
            Predicate<StateChangingEventArgs<T>> additionalCondition)
            : base(command, (s, e) => CheckBaseCondition(s, e, sender, state) && additionalCondition(e))
        {
        }

        /// <summary>
        /// Checks the base condition of this trigger.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">Event data associated with the event.</param>
        /// <param name="sender">The sender required by the trigger.</param>
        /// <param name="state">The state required by the trigger.</param>
        /// <returns>True if the event has the same sender and represents the required state change, otherwise 
        /// false.</returns>
        private static bool CheckBaseCondition(
            object source, StateChangingEventArgs<T> e, IStateMachine<T> sender, T state)
        {
            return ReferenceEquals(source, sender) && e.StateChange.NewState.Equals(state);
        }
    }
}
