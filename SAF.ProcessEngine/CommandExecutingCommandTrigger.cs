namespace SAF.ProcessEngine
{
    /// <summary>
    /// Encapsulates a command executing state condition that should trigger execution of another command.
    /// </summary>
    public class CommandExecutingCommandTrigger : CommandTrigger<CommandExecutingEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecutingCommandTrigger"/> class with the specified
        /// command and sender.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="sender">The sender of the event.</param>
        public CommandExecutingCommandTrigger(IExecutable command, IExecutable sender)
            : base(command, (s, e) => ReferenceEquals(sender, s))
        {
        }
    }
}
