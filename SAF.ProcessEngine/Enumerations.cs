namespace SAF.ProcessEngine
{
    /// <summary>
    /// Represents the types of processes in this namespace.
    /// </summary>
    public enum ProcessType
    {
        /// <summary>
        /// The process type is not set.
        /// </summary>
        None = 0,

        /// <summary>
        /// The process is a producer.
        /// </summary>
        Producer = 1,

        /// <summary>
        /// The process is a consumer.
        /// </summary>
        Consumer = 2,

        /// <summary>
        /// The process is auxiliary.
        /// </summary>
        Auxiliary = 3,

        /// <summary>
        /// The process is a process controller.
        /// </summary>
        Controller = 4
    }

    /// <summary>
    /// Represents the states of a generic process.
    /// </summary>
    public enum ProcessState
    {
        /// <summary>
        /// The process has not yet started.
        /// </summary>
        NotStarted = 0,

        /// <summary>
        /// The process has started.
        /// </summary>
        Started = 1,

        /// <summary>
        /// The process has stopped.
        /// </summary>
        Stopped = 2
    }

    /// <summary>
    /// Represents the states of a command execution.
    /// </summary>
    public enum CommandState
    {
        /// <summary>
        /// The command has not yet been executed.
        /// </summary>
        NotExecuted = 0,

        /// <summary>
        /// The command is executing.
        /// </summary>
        Executing = 1,

        /// <summary>
        /// The command has been executed.
        /// </summary>
        Executed = 2
    }

    /// <summary>
    /// Represents the result states of a generic process.
    /// </summary>
    public enum UserResultState
    {
        /// <summary>
        /// The process is running without error.
        /// </summary>
        Nominal = 0,
        
        /// <summary>
        /// A task has returned an error.
        /// </summary>
        TaskError = 1,

        /// <summary>
        /// Part of the process has returned an error.
        /// </summary>
        ProcessError = 2
    }

    /// <summary>
    /// Possible states of an item-related task result.
    /// </summary>
    public enum ResultState
    {
        /// <summary>
        /// The result is successful.
        /// </summary>
        Success = 0,

        /// <summary>
        /// The result is empty.
        /// </summary>
        Empty = 1,

        /// <summary>
        /// The result is an error.
        /// </summary>
        Error = 2
    }
}
