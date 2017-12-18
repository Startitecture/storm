namespace SAF.ProcessEngine
{
    using System;

    /// <summary>
    /// Event data related to a command failure event.
    /// </summary>
    public class CommandFailedEventArgs : EventArgs, IErrorEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandFailedEventArgs"/> class with the associated
        /// command execution result.
        /// </summary>
        /// <param name="command">The command that was executed.</param>
        /// <param name="error">The error, if any, associated with the command's execution.</param>
        [System.Diagnostics.DebuggerHidden]
        public CommandFailedEventArgs(IExecutable command, Exception error)
        {
            this.Command = command;
            this.EventError = error;
        }

        /// <summary>
        /// Gets the command that was executed.
        /// </summary>
        public IExecutable Command { get; private set; }

        /// <summary>
        /// Gets the error, if any, assoicated with the command execution.
        /// </summary>
        public Exception EventError { get; private set; }
    }
}
