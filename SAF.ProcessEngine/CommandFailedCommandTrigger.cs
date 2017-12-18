namespace SAF.ProcessEngine
{
    /// <summary>
    /// Encapsulates a command failed state condition that should trigger execution of another command.
    /// </summary>
    public class CommandFailedCommandTrigger : CommandTrigger<CommandFailedEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandFailedCommandTrigger"/> class with the specified
        /// command and sender.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="sender">The sender of the event.</param>
        public CommandFailedCommandTrigger(IExecutable command, IExecutable sender)
            : base(command, (s, e) => ReferenceEquals(sender, s))
        {
        }
    }
}
