namespace SAF.ProcessEngine
{
    /// <summary>
    /// Represents a <see cref="CommandCompletedEventArgs"/> event that causes a state change.
    /// </summary>
    /// <typeparam name="T">The type that represents the possible states to trigger.</typeparam>
    public class CommandCompletedStateTrigger<T> : StateTriggerBase<CommandCompletedEventArgs, T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandCompletedStateTrigger&lt;T&gt;"/> class with the 
        /// specified command and state.
        /// </summary>
        /// <param name="newState">The state that should be triggered.</param>
        /// <param name="command">The command that should trigger the state change once executed.</param>
        public CommandCompletedStateTrigger(T newState, IExecutable command)
            : base(newState, (s, e) => ReferenceEquals(command, s))
        {
            this.Command = command;
        }

        /// <summary>
        /// Gets the command associated with the trigger.
        /// </summary>
        public IExecutable Command { get; private set; }
    }
}
