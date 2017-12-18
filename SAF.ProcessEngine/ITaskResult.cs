namespace SAF.ProcessEngine
{
    using System;

    /// <summary>
    /// Provides an interface to the results of a task.
    /// </summary>
    public interface ITaskResult
    {
        /// <summary>
        /// Gets a value indicating the state of the result.
        /// </summary>
        ResultState ResultState { get; }

        /// <summary>
        /// Gets the exception, if any, associated with the task.
        /// </summary>
        Exception ItemError { get; }
    }
}
