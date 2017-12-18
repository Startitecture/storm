namespace SAF.ProcessEngine
{
    using System;

    /// <summary>
    /// Provides an interface to classes that dispatch commands.
    /// </summary>
    public interface ICommandDispatcher
    {
        /// <summary>
        /// Occurs when the command is executed.
        /// </summary>
        event EventHandler<CommandExecutingEventArgs> CommandExecuting;

        /// <summary>
        /// Occurs when the command is executing.
        /// </summary>
        event EventHandler<CommandCompletedEventArgs> CommandCompleted;

        /// <summary>
        /// Occurs when a command has failed during execution.
        /// </summary>
        event EventHandler<CommandFailedEventArgs> CommandFailed;
    }
}
