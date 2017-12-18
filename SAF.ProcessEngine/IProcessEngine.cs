// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProcessEngine.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface to a process engine with discrete start and stop events.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ProcessEngine
{
    using System;

    using SAF.Core;

    /// <summary>
    /// Provides an interface to a process engine with discrete start and stop events.
    /// </summary>
    public interface IProcessEngine : INamedComponent
    {
        /// <summary>
        /// Occurs when the task engine starts.
        /// </summary>
        event EventHandler<ProcessStartedEventArgs> ProcessStarted;

        /// <summary>
        /// Occurs when the task engine stops.
        /// </summary>
        event EventHandler<ProcessStoppedEventArgs> ProcessStopped;

        /// <summary>
        /// Gets a value indicating whether the task engine is busy.
        /// </summary>
        bool IsBusy { get; }

        /// <summary>
        /// Gets the amount of time that the task engine has spent processing tasks.
        /// </summary>
        TimeSpan ProcessTime { get; }

        /// <summary>
        /// Gets a value indicating whether the task engine has been canceled.
        /// </summary>
        bool Canceled { get; }

        /// <summary>
        /// Cancels the current process.
        /// </summary>
        /// <exception cref="OperationException">
        /// The process is not running and therefore cannot be canceled.
        /// </exception>
        void Cancel();

        /// <summary>
        /// Cancels the current process.
        /// </summary>
        /// <exception cref="OperationException">
        /// The process is not running and therefore cannot be canceled.
        /// </exception>
        /// <param name="timeout">
        /// The amount of time to wait for completion of the request.
        /// </param>
        /// <returns>
        /// <c>true</c> if the process stopped prior to the timeout; otherwise <c>false</c>.
        /// </returns>
        bool Cancel(TimeSpan timeout);

        /// <summary>
        /// Blocks the current thread until the queue is emptied.
        /// </summary>
        void WaitForCompletion();

        /// <summary>
        /// Blocks the current thread until the queue is emptied.
        /// </summary>
        /// <param name="timeout">
        /// The amount of time to wait for completion of the request.
        /// </param>
        /// <returns>
        /// <c>true</c> if the process stopped prior to the timeout; otherwise <c>false</c>.
        /// </returns>
        bool WaitForCompletion(TimeSpan timeout);
    }
}
