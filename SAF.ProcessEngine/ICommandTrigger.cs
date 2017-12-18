namespace SAF.ProcessEngine
{
    /// <summary>
    /// Provides an interface to a command trigger.
    /// </summary>
    public interface ICommandTrigger : IEventTrigger
    {
        /// <summary>
        /// Gets the command associated with this trigger.
        /// </summary>
        IExecutable Command { get; }
    }
}
