// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IObservablePoolState.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for observing the state of a queue pool.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides an interface for observing the state of a queue pool.
    /// </summary>
    public interface IObservablePoolState : IObservableQueueState
    {
        /// <summary>
        /// Gets the number of active queues.
        /// </summary>
        int QueueCount { get; }

        /// <summary>
        /// Gets the queue states for the current pool.
        /// </summary>
        IEnumerable<IPriorityQueueState> PoolStates { get; }

        /// <summary>
        /// Gets the highest queue concurrency for the pool since it has been in use.
        /// </summary>
        int HighestConcurrency { get; }
    }
}
