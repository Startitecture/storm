// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPriorityQueueState.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface to objects that monitor queue state.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;

    /// <summary>
    /// Provides an interface to objects that monitor queue state.
    /// </summary>
    public interface IPriorityQueueState
    {
        /// <summary>
        /// Gets a value indicating whether the queue is busy.
        /// </summary>
        bool IsBusy { get; }

        /// <summary>
        /// Gets a value indicating whether the queue has been aborted.
        /// </summary>
        bool QueueAborted { get; }

        /// <summary>
        /// Gets the number of elements in the queue.
        /// </summary>
        long QueueLength { get; }

        /// <summary>
        /// Gets the average time between the last several requests.
        /// </summary>
        TimeSpan AverageRequestLatency { get; }

        /// <summary>
        /// Gets the average time between the last several responses to requests.
        /// </summary>
        TimeSpan AverageResponseLatency { get; }

        /// <summary>
        /// Gets the failure rate over the last several requests.
        /// </summary>
        double FailureRate { get; }

        /// <summary>
        /// Gets the number of messages processed by the queue route.
        /// </summary>
        long MessagesProcessed { get; }

        /// <summary>
        /// Gets the number of message requests.
        /// </summary>
        long MessageRequests { get; }
    }
}