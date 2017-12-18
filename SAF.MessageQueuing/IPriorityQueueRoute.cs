// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPriorityQueueRoute.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface to a priority queue.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;

    using SAF.ProcessEngine;

    /// <summary>
    /// Provides an interface to a priority queue.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of <see cref="IPriorityMessage"/> that the queue handles.
    /// </typeparam>
    public interface IPriorityQueueRoute<TMessage> : IServiceRoute<TMessage>, IProcessEngine, IObservableQueueState
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// Gets the messages waiting in the queue.
        /// </summary>
        MessageRoutingRequestCollection<TMessage> QueuedMessages { get; }

        /// <summary>
        /// Subscribes a queue monitor with the current route.
        /// </summary>
        /// <param name="monitor">
        /// The queue monitor to subscribe.
        /// </param>
        void StartSubscription(QueueMonitor<TMessage> monitor);

        /// <summary>
        /// Ends the subscription of a queue monitor to the current route.
        /// </summary>
        /// <param name="monitor">
        /// The queue monitor to unsubscribe.
        /// </param>
        void EndSubscription(QueueMonitor<TMessage> monitor);
    }
}