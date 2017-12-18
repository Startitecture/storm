// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProducerConsumer.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for components that produce and consume items.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ProcessEngine
{
    using System;

    using SAF.Core;

    /// <summary>
    /// Provides an interface for components that produce and consume items.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of item that is produced and consumed.
    /// </typeparam>
    public interface IProducerConsumer<TItem>
    {
        /// <summary>
        /// Occurs when items are ready to retrieve.
        /// </summary>
        event EventHandler<ItemsProducedEventArgs> ItemsProduced;

        /// <summary>
        /// Gets a queue consumer for the items produced by this producer.
        /// </summary>
        QueueConsumer<TItem> ItemQueueConsumer { get; }

        /// <summary>
        /// Produces an item into the queue.
        /// </summary>
        /// <param name="item">The item to produce.</param>
        void ProduceItem(TItem item);

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
        /// <c>true</c> if the queue emptied prior to the timeout; otherwise <c>false</c>.
        /// </returns>
        bool WaitForCompletion(TimeSpan timeout);
    }
}