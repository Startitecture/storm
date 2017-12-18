namespace SAF.ProcessEngine
{
    /// <summary>
    /// Represents a <see cref="CommandExecutingEventArgs"/> event that causes a state change.
    /// </summary>
    /// <typeparam name="T">The type that represents the possible states to trigger.</typeparam>
    public class CommandExecutingStateTrigger<T> : StateTriggerBase<CommandExecutingEventArgs, T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecutingStateTrigger&lt;T&gt;"/> class with the 
        /// specified command and state.
        /// </summary>
        /// <param name="newState">The state that should be triggered.</param>
        /// <param name="command">The command that should trigger the state change upon execution.</param>
        public CommandExecutingStateTrigger(T newState, IExecutable command)
            : base(newState, (s, e) => ReferenceEquals(command, s))
        {
        }
    }
}
