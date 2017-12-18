namespace SAF.ProcessEngine
{
    /// <summary>
    /// Represents a <see cref="CommandCompletedEventArgs"/> event that causes a state change.
    /// </summary>
    /// <typeparam name="T">The type that represents the possible states to trigger.</typeparam>
    public class CommandFailedStateTrigger<T> : StateTriggerBase<CommandFailedEventArgs, T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandFailedStateTrigger&lt;T&gt;"/> class with the 
        /// specified command and state.
        /// </summary>
        /// <param name="newState">The state that should be triggered.</param>
        /// <param name="command">The command that should trigger the state change once executed.</param>
        public CommandFailedStateTrigger(T newState, IExecutable command)
            : base(newState, (s, e) => ReferenceEquals(command, s))
        {
        }
    }
}
