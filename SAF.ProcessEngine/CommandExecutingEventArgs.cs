namespace SAF.ProcessEngine
{
    using System;

    /// <summary>
    /// Event data for an executing command.
    /// </summary>
    public class CommandExecutingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecutingEventArgs"/> class with the associated 
        /// executing command.
        /// </summary>
        /// <param name="command">The command that is executing.</param>
        [System.Diagnostics.DebuggerHidden]
        public CommandExecutingEventArgs(IExecutable command)
        {
            this.Command = command;
        }

        /// <summary>
        /// Gets the command that is executing.
        /// </summary>
        public IExecutable Command
        {
            get;
            private set;
        }
    }
}
