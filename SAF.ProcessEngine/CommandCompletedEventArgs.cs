namespace SAF.ProcessEngine
{
    using System;

    /// <summary>
    /// Event data for an executed command.
    /// </summary>
    public class CommandCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandCompletedEventArgs"/> class with the associated
        /// command execution result.
        /// </summary>
        /// <param name="command">The command that was executed.</param>
        [System.Diagnostics.DebuggerHidden]
        public CommandCompletedEventArgs(IExecutable command)
        {
            this.Command = command;
        }

        /// <summary>
        /// Gets the command that was executed.
        /// </summary>
        public IExecutable Command { get; private set; }
    }
}
