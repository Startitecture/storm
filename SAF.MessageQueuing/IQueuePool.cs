// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQueuePool.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface to a queue pool that
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;

    using SAF.ProcessEngine;

    /// <summary>
    /// Provides an interface to a queue pool that 
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of message handled by the pool.
    /// </typeparam>
    public interface IQueuePool<TMessage> : IServiceRoute<TMessage>, IObservablePoolState, IProcessEngine
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// Gets the messages that caused a queue to abort.
        /// </summary>
        MessageRoutingRequestCollection<TMessage> AbortedMessages { get; }
    }
}