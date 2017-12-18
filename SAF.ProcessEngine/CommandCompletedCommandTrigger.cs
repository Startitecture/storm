namespace SAF.ProcessEngine
{
    /// <summary>
    /// Encapsulates a command executed state condition that should trigger execution of another command.
    /// </summary>
    public class CommandCompletedCommandTrigger : CommandTrigger<CommandCompletedEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandCompletedCommandTrigger"/> class with the specified 
        /// command and sender.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="sender">The sender of the event.</param>
        public CommandCompletedCommandTrigger(IExecutable command, IExecutable sender)
            : base(command, (s, e) => ReferenceEquals(sender, s))
        {
        }
    }
}
