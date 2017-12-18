// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITaskEngine.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface to a task engine.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ProcessEngine
{
    /// <summary>
    /// Provides an interface to a task engine.
    /// </summary>
    public interface ITaskEngine : IProcessEngine
    {
        /// <summary>
        /// Gets the number of tasks assigned to the process.
        /// </summary>
        long TotalTasks { get; }

        /// <summary>
        /// Gets the number of tasks waiting in the queue.
        /// </summary>
        long WaitingTasks { get; }

        /// <summary>
        /// Gets the number of tasks that the task engine has consumed.
        /// </summary>
        long CompletedTasks { get; }

        /// <summary>
        /// Gets the number of successful task results.
        /// </summary>
        long SuccessfulResults { get; }

        /// <summary>
        /// Gets the number of empty task results.
        /// </summary>
        long EmptyResults { get; }

        /// <summary>
        /// Gets the number of failed task results.
        /// </summary>
        long FailedResults { get; }

        /// <summary>
        /// Gets the progress of the process.
        /// </summary>
        double Progress { get; }

        /// <summary>
        /// Gets the health of the current process in terms of whether tasks or commands have failed.
        /// </summary>
        UserResultState ProcessHealth { get; }

        /// <summary>
        /// Gets the average time taken per task.
        /// </summary>
        double TasksPerSecond { get; }
    }
}
