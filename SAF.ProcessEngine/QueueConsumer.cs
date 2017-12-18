// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueueConsumer.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Consumes items from a queue using synchronized lock.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ProcessEngine
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;

    /// <summary>
    /// Consumes items from a queue using synchronized lock.
    /// </summary>
    /// <typeparam name="T">The type of item to consume.</typeparam>
    public sealed class QueueConsumer<T>
    {
        /// <summary>
        /// The queue to retrieve items from.
        /// </summary>
        private readonly IProducerConsumerCollection<T> queue;

        /// <summary>
        /// The object that synchronizes queue operations.
        /// </summary>
        private readonly object queueSync;

        /// <summary>
        /// The object that synchronizes queue limiting.
        /// </summary>
        private readonly object queueLimitSync;

        /// <summary>
        /// The current item.
        /// </summary>
        private T current;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueConsumer&lt;T&gt;"/> class with the specified queue and
        /// synchronization object.
        /// </summary>
        /// <param name="queue">
        /// The queue to consume.
        /// </param>
        /// <param name="queueSync">
        /// The synchronization object to use for thread-safe access to the queue. This object should be used to synchronize queue 
        /// operations.
        /// </param>
        /// <param name="queueLimitSync">
        /// The synchronization object to use for queue limiting functionality.
        /// </param>
        public QueueConsumer(
            IProducerConsumerCollection<T> queue, 
            object queueSync, 
            object queueLimitSync)
        {
            if (queue == null)
            {
                throw new ArgumentNullException("queue");
            }

            if (queueSync == null)
            {
                throw new ArgumentNullException("queueSync");
            }

            if (queueLimitSync == null)
            {
                throw new ArgumentNullException("queueLimitSync");
            }

            this.queue = queue;
            this.queueSync = queueSync;
            this.queueLimitSync = queueLimitSync;
        }

        /// <summary>
        /// Occurs when all queue items have been consumed.
        /// </summary>
        public event EventHandler<ItemsConsumedEventArgs> AllItemsConsumed;

        /// <summary>
        /// Gets the most recent item retrieved from the queue.
        /// </summary>
        public T Current
        {
            get
            {
                return this.current;
            }
        }

        /// <summary>
        /// Consumes the next item in the queue, if one exists.
        /// </summary>
        /// <returns>
        /// True if the queue had an item remaining to de-queue, otherwise false.
        /// </returns>
        public bool ConsumeNext()
        {
            lock (this.queueSync)
            {
                T item;

                if (!this.queue.TryTake(out item))
                {
                    this.OnAllItemsConsumed(ItemsConsumedEventArgs.Empty);
                    return false;
                }

                this.current = item;

                lock (this.queueLimitSync)
                {
                    Monitor.Pulse(this.queueLimitSync);
                }
            }

            return true;
        }

        /// <summary>
        /// Asynchronously triggers the <see cref="AllItemsConsumed"/> event.
        /// </summary>
        /// <param name="e"><see cref="ItemsConsumedEventArgs"/> event data associated with the event.</param>
        private void OnAllItemsConsumed(ItemsConsumedEventArgs e)
        {
            EventHandler<ItemsConsumedEventArgs> temp = this.AllItemsConsumed;

            if (temp != null)
            {
                temp(this, e);
            }
        }
    }
}
