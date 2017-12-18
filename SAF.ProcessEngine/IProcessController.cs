namespace SAF.ProcessEngine
{
    /// <summary>
    /// Provides an interface to a process controller.
    /// </summary>
    /// <typeparam name="TState">The type that represents the possible states of the process.</typeparam>
    public interface IProcessController<TState> : IStateMachine<TState>, ITaskEngine, ICommandDispatcher
    {
    }
}
