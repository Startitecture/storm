// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemProducer.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Base class for item producers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ProcessEngine
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;

    using SAF.Core;
    using SAF.StringResources;

    /// <summary>
    /// Base class for item producers.
    /// </summary>
    /// <typeparam name="TItem">The type of item that is produced.</typeparam>
    public class ItemProducer<TItem> : ProcessEngineBase, IProducerConsumer<TItem>
    {
        #region Fields

        /// <summary>
        /// Name of this instance's thread.
        /// </summary>
        private const string ThreadName = "ItemProducer";

        /// <summary>
        /// The object that provides synchronized access to the task limit counter.
        /// </summary>
        private readonly object queueLimitControl = new object();

        /// <summary>
        /// The object that provides synchronized access to the producer queue.
        /// </summary>
        private readonly object itemQueueControl = new object();

        /// <summary>
        /// The object that provides synchronized access for item retrievals.
        /// </summary>
        private readonly object retrievalControl = new object();

        /// <summary>
        /// The producer queue for the items.
        /// </summary>
        private readonly IProducerConsumerCollection<TItem> itemQueue;

        /// <summary>
        /// The queue consumer to provide to consumers of this item producer.
        /// </summary>
        private readonly QueueConsumer<TItem> itemQueueConsumer;

        /// <summary>
        /// The maximum number of items that the collection can contain.
        /// </summary>
        private long maxQueueLength = Int64.MaxValue;

        /// <summary>
        /// The number of items that have been added..
        /// </summary>
        private long addedItems;

        /// <summary>
        /// The value that indicates whether a consumer is retrieving items.
        /// </summary>
        private bool retrievingItems;

        /// <summary>
        /// The subscriber exception.
        /// </summary>
        private Exception subscriberException;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SAF.ProcessEngine.ItemProducer`1"/> class.
        /// </summary>
        public ItemProducer()
            : this(ThreadName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SAF.ProcessEngine.ItemProducer`1"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the producer component.
        /// </param>
        public ItemProducer(string name)
            : this(name, new ConcurrentQueue<TItem>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SAF.ProcessEngine.ItemProducer`1"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the producer component.
        /// </param>
        /// <param name="collection">
        /// The collection.
        /// </param>
        public ItemProducer(string name, IProducerConsumerCollection<TItem> collection)
            : base(name)
        {
            this.itemQueue = collection;
            this.itemQueueConsumer = new QueueConsumer<TItem>(
                this.itemQueue,
                this.itemQueueControl,
                this.queueLimitControl);

            this.itemQueueConsumer.AllItemsConsumed += this.FinalizeItemRetrieval;
            this.StartItemThread();
        }

        #region Delegates and Events

        /// <summary>
        /// Occurs when items are ready to retrieve.
        /// </summary>
        public event EventHandler<ItemsProducedEventArgs> ItemsProduced;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a queue consumer for the items produced by this producer.
        /// </summary>
        public QueueConsumer<TItem> ItemQueueConsumer
        {
            get { return this.itemQueueConsumer; }
        }

        /// <summary>
        /// Gets or sets the limit on the number of waiting tasks. Once the limit is reached, the caller's thread 
        /// will be blocked until another task is processed.
        /// </summary>
        public long MaxQueueLength
        {
            get { return Interlocked.Read(ref this.maxQueueLength); }
            set { this.maxQueueLength = value > 0 ? value : 1; }
        }

        /// <summary>
        /// Gets the number of items added to the queue.
        /// </summary>
        public long ItemsAdded
        {
            get { return Interlocked.Read(ref this.addedItems); }
        }

        /// <summary>
        /// Gets the number of items waiting to be processed.
        /// </summary>
        public long ItemsPending 
        {
            get { return this.itemQueue.Count; } 
        }

        /// <summary>
        /// Gets the unprocessed items remaining in the queue.
        /// </summary>
        public IEnumerable<TItem> QueuedItems
        {
            get
            {
                return this.itemQueue.ToList();
            }
        }

        #endregion 

        #region Methods

        /// <summary>
        /// Produces an item into the queue.
        /// </summary>
        /// <param name="item">The item to produce.</param>
        public void ProduceItem(TItem item)
        {
            lock (this.itemQueueControl)
            {
                if (this.Canceled)
                {
                    throw new ComponentAbortedException(
                        String.Format(ErrorMessages.ComponentAbortedAndRefusingRequests, this),
                        this.subscriberException);
                }
            }

            Interlocked.Increment(ref this.addedItems);

            if (this.itemQueue.Count >= this.maxQueueLength)
            {
                lock (this.queueLimitControl)
                {
                    if (this.ItemsPending >= this.maxQueueLength)
                    {
                        Monitor.Wait(this.queueLimitControl);
                    }

                    if (this.Canceled)
                    {
                        throw new ComponentAbortedException(
                            String.Format(ErrorMessages.ComponentAbortedAndRefusingRequests, this),
                            this.subscriberException);
                    }
                }
            }

            lock (this.itemQueueControl)
            {
                bool pulseQueue = false;

                if (this.itemQueue.Count == 0)
                {
                    pulseQueue = true;
                    this.StartProcess();
                }

                if (!this.itemQueue.TryAdd(item))
                {
                    throw new OperationException(item, ErrorMessages.ItemCouldNotBeAddedToCollection);
                }

                if (pulseQueue)
                {
                    Monitor.Pulse(this.itemQueueControl);
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this <see cref="TaskEngine&lt;TDir, TResult&gt;"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> representation of this 
        /// <see cref="TaskEngine&lt;TDir, TResult&gt;"/>.</returns>
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Cancels the current process.
        /// </summary>
        protected override void CancelProcess()
        {
            // Pulse the wait operations for both threads.
            lock (this.itemQueueControl)
            {
                Monitor.Pulse(this.itemQueueControl);
            }

            lock (this.queueLimitControl)
            {
                Monitor.Pulse(this.queueLimitControl);
            }
        }

        /// <summary>
        /// Determines whether the process is stopping.
        /// </summary>
        /// <returns>True if the process is stopping, otherwise false.</returns>
        protected override bool IsStopping()
        {
            return this.ItemsPending == 0;
        }

        /// <summary>
        /// Triggers the ProcessStopped event.
        /// </summary>
        /// <param name="eventArgs">
        /// Event data associated with the event.
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Do not throw exceptions in background threads.")]
        protected override void OnProcessStopped(ProcessStoppedEventArgs eventArgs)
        {
            if (eventArgs == null)
            {
                throw new ArgumentNullException("eventArgs");
            }

            // TFS ITEM 23128 - do not allow the background thread to throw exceptions. Trace and save the exception to be observed the
            // next time the queue is used.
            try
            {
                base.OnProcessStopped(eventArgs);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ErrorMessages.EventSubscriberThrewExceptionInBackgroundThread, this, ex);
                this.subscriberException = Normalize.AggregateExceptions(ex, eventArgs.EventError);

                if (this.Canceled == false)
                {
                    this.Cancel();
                }
            }
        }

        /// <summary>
        /// Starts the item queue thread.
        /// </summary>
        private void StartItemThread()
        {
            // Create a background thread. The thread will automatically terminate when the program terminates.
            var thread = new Thread(this.InitializeQueue) { Name = this.Name, IsBackground = true };
            thread.Start();
        }

        /// <summary>
        /// Initializes the item queue.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", 
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Generic exceptions are handled in this thread and passed back to the caller through events.")]
        private void InitializeQueue()
        {
            Exception error = null;

            try
            {
                while (!this.Canceled)
                {
                    if (this.itemQueue.Count == 0)
                    {
                        lock (this.itemQueueControl)
                        {
                            // Check before and after the wait.
                            if (this.Canceled)
                            {
                                break;
                            }

                            if (this.itemQueue.Count == 0)
                            {
                                Monitor.Wait(this.itemQueueControl);
                            }

                            if (this.Canceled)
                            {
                                break;
                            }
                        }
                    }

                    // By using the retrieval control and flag, we prevent triggering an event every single time an item is produced, 
                    // while ensuring the QueueConsumer consumes all the produced items.
                    if (this.retrievingItems)
                    {
                        continue;
                    }

                    lock (this.retrievalControl)
                    {
                        if (this.retrievingItems)
                        {
                            continue;
                        }

                        this.retrievingItems = true;
                        this.OnItemsProduced(ItemsProducedEventArgs.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                var message = String.Format(ErrorMessages.ComponentExitingWithUnhandledError, this.Name, ex.Message);
                this.retrievingItems = false;

                lock (this.itemQueueControl)
                {
                    error = new QueueAbortException<TItem>(message, this.itemQueueConsumer.Current, this.itemQueue.ToList(), ex);
                    this.CancelProcess();
                }
            }

            this.StopProcessIfStopping(error);
        }

        /// <summary>
        /// Finalizes the item retrieval process.
        /// </summary>
        /// <param name="sender">The sender of the <see cref="ItemsConsumedEventArgs"/> event.</param>
        /// <param name="e">The <see cref="ItemsConsumedEventArgs"/> associated with the event.</param>
        private void FinalizeItemRetrieval(object sender, ItemsConsumedEventArgs e)
        {
            lock (this.retrievalControl)
            {
                this.retrievingItems = false;
            }

            this.StopProcessIfStopping(null);
        }

        /// <summary>
        /// Triggers the <see cref="ItemsProduced"/> event if the consumer queue has waiting items.
        /// </summary>
        /// <param name="e"><see cref="ItemsProducedEventArgs"/> associated with the event.</param>
        private void OnItemsProduced(ItemsProducedEventArgs e)
        {
            var temp = this.ItemsProduced;

            if (temp != null)
            {
                temp(this, e);
            }
        }

        #endregion 
    }
}
