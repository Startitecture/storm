// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IObservableQueueState.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for observing the state of a queue.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using SAF.Core;

    /// <summary>
    /// Provides an interface for observing the state of a queue.
    /// </summary>
    public interface IObservableQueueState : INamedComponent
    {
        /// <summary>
        /// Gets the state of the current queue.
        /// </summary>
        IPriorityQueueState QueueState { get; }
    }
}
