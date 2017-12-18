// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PriorityQueuePool.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    using SAF.ActionTracking;
    using SAF.Core;
    using SAF.ProcessEngine;
    using SAF.StringResources;

    /// <summary>
    /// The base class for priority queue pools. The number of queues in the pool is related to CPU usage and whether or not the
    /// existing queues are currently keeping up with requests.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of <see cref="IPriorityMessage"/> handled by the queue pool.
    /// </typeparam>
    /// <typeparam name="TQueue">
    /// The type of queues in the pool.
    /// </typeparam>
    public abstract class PriorityQueuePool<TMessage, TQueue> : ProcessEngineBase, IQueuePool<TMessage>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
        where TQueue : IPriorityQueueRoute<TMessage>
    {
        #region Constants

        /// <summary>
        /// The to string format.
        /// </summary>
        private const string ToStringFormat = "{0} [pool {1}]";

        /// <summary>
        /// The producer name format.
        /// </summary>
        private const string ProducerNameFormat = "{0} {1} producer";

        /// <summary>
        /// The log policy setting.
        /// </summary>
        private const string LogPolicySetting = "LogQueuePolicyResponses";

        #endregion

        #region Static Fields

        /// <summary>
        /// The select queue state.
        /// </summary>
        private static readonly Func<TQueue, IPriorityQueueState> SelectQueueState = x => x.QueueState;

        /// <summary>
        /// The queue selector.
        /// </summary>
        private static readonly Func<KeyValuePair<Guid, TQueue>, TQueue> QueueSelector = x => x.Value;

        #endregion

        #region Fields

        /// <summary>
        /// The factory for creating new queues.
        /// </summary>
        private readonly IQueueRouteFactory<TQueue> factory;

        /// <summary>
        /// The queuing policy.
        /// </summary>
        private readonly IQueuingPolicy queuingPolicy;

        /// <summary>
        /// The queue availability comparer.
        /// </summary>
        private readonly QueueAvailabilityComparer<TQueue> queueAvailabilityComparer = QueueAvailabilityComparer<TQueue>.Availability;

        /// <summary>
        /// The routing request producer.
        /// </summary>
        private readonly ItemProducer<MessageRoutingRequest<TMessage>> requestQueue;

        /// <summary>
        /// The aborted requests.
        /// </summary>
        private readonly ConcurrentBag<MessageRoutingRequest<TMessage>> abortedMessages =
            new ConcurrentBag<MessageRoutingRequest<TMessage>>();

        /// <summary>
        /// The active processing queues.
        /// </summary>
        private readonly ConcurrentDictionary<Guid, TQueue> activeQueues = new ConcurrentDictionary<Guid, TQueue>();

        /// <summary>
        /// The idle processing queues.
        /// </summary>
        private readonly ConcurrentDictionary<Guid, TQueue> idleQueues = new ConcurrentDictionary<Guid, TQueue>(); 

        /// <summary>
        /// The queue monitor.
        /// </summary>
        private readonly QueueMonitor<TMessage> poolMonitor;

        /// <summary>
        /// The action event proxy.
        /// </summary>
        private readonly IActionEventProxy actionEventProxy;

        /// <summary>
        /// The queue lock.
        /// </summary>
        private readonly object queueLock = new object();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueuePool{TMessage,TQueue}"/> class.
        /// </summary>
        /// <param name="profileProvider">
        /// The profile provider.
        /// </param>
        /// <param name="factory">
        /// The factory.
        /// </param>
        /// <param name="queuingPolicy">
        /// The queuing policy.
        /// </param>
        /// <param name="actionEventProxy">
        /// The action event proxy.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory"/>, <paramref name="profileProvider"/>, or <paramref name="queuingPolicy"/> is null.
        /// </exception>
        protected PriorityQueuePool(
            IRoutingProfileProvider<TMessage> profileProvider, 
            IQueueRouteFactory<TQueue> factory, 
            IQueuingPolicy queuingPolicy, 
            IActionEventProxy actionEventProxy, 
            string name)
            : base(name)
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }

            if (profileProvider == null)
            {
                throw new ArgumentNullException("profileProvider");
            }

            if (queuingPolicy == null)
            {
                throw new ArgumentNullException("queuingPolicy");
            }

            if (actionEventProxy == null)
            {
                throw new ArgumentNullException("actionEventProxy");
            }

            this.InstanceIdentifier = Guid.NewGuid();
            this.factory = factory;
            this.queuingPolicy = queuingPolicy;
            this.actionEventProxy = actionEventProxy;
            this.poolMonitor = new QueueMonitor<TMessage>(this);
            this.requestQueue = CreateProducerQueue(profileProvider.PriorityComparer, name);
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueuePool{TMessage,TQueue}"/> class.
        /// </summary>
        /// <param name="factory">
        /// The queue route factory.
        /// </param>
        /// <param name="priorityComparer">
        /// The message priority comparer.
        /// </param>
        /// <param name="queuingPolicy">
        /// The queuing policy.
        /// </param>
        /// <param name="actionEventProxy">
        /// The action event proxy.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory"/>, <paramref name="priorityComparer"/>, or <paramref name="queuingPolicy"/> is null.
        /// </exception>
        protected PriorityQueuePool(
            IQueueRouteFactory<TQueue> factory, 
            IComparer<TMessage> priorityComparer, 
            IQueuingPolicy queuingPolicy, 
            IActionEventProxy actionEventProxy)
            : this(factory, priorityComparer, queuingPolicy, actionEventProxy, typeof(TQueue).ToRuntimeName())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueuePool{TMessage,TQueue}"/> class.
        /// </summary>
        /// <param name="factory">
        /// The queue route factory.
        /// </param>
        /// <param name="priorityComparer">
        /// The message priority comparer.
        /// </param>
        /// <param name="queuingPolicy">
        /// The queuing policy.
        /// </param>
        /// <param name="actionEventProxy">
        /// The action Event Proxy.
        /// </param>
        /// <param name="name">
        /// The name of the queue.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory"/>, <paramref name="priorityComparer"/>, or <paramref name="queuingPolicy"/> is null.
        /// </exception>
        protected PriorityQueuePool(
            IQueueRouteFactory<TQueue> factory, 
            IComparer<TMessage> priorityComparer, 
            IQueuingPolicy queuingPolicy, 
            IActionEventProxy actionEventProxy, 
            string name)
            : base(name)
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }

            if (priorityComparer == null)
            {
                throw new ArgumentNullException("priorityComparer");
            }

            if (queuingPolicy == null)
            {
                throw new ArgumentNullException("queuingPolicy");
            }

            this.InstanceIdentifier = Guid.NewGuid();
            this.factory = factory;
            this.queuingPolicy = queuingPolicy;
            this.actionEventProxy = actionEventProxy;
            this.poolMonitor = new QueueMonitor<TMessage>(this);
            this.requestQueue = CreateProducerQueue(priorityComparer, name);
            this.Initialize();
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Occurs when a queue processing request is completed.
        /// </summary>
        public event EventHandler<ResponseEventArgs<TMessage>> RequestCompleted;

        /// <summary>
        /// Occurs when a queue processing request is received.
        /// </summary>
        public event EventHandler<RequestEventArgs<TMessage>> RequestReceived;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the instance identifier for the current service route.
        /// </summary>
        public Guid InstanceIdentifier { get; private set; }

        /// <summary>
        /// Gets the pool state.
        /// </summary>
        public IPriorityQueueState QueueState
        {
            get
            {
                return this.poolMonitor;
            }
        }

        /// <summary>
        /// Gets the number of active queues.
        /// </summary>
        public int QueueCount
        {
            get
            {
                return this.activeQueues.Count + this.idleQueues.Count;
            }
        }

        /// <summary>
        /// Gets the queue states for the current pool.
        /// </summary>
        public IEnumerable<IPriorityQueueState> PoolStates
        {
            get
            {
                return this.activeQueues.Values.Union(this.idleQueues.Values).Select(SelectQueueState);
            }
        }

        /// <summary>
        /// Gets the highest queue concurrency for the pool since it has been in use.
        /// </summary>
        public int HighestConcurrency { get; private set; }

        /// <summary>
        /// Gets the messages that caused a queue to abort.
        /// TODO: This needs to have some kind of reconciliation so messages don't grow forever.
        /// </summary>
        [DoNotLog]
        public MessageRoutingRequestCollection<TMessage> AbortedMessages
        {
            get
            {
                return new MessageRoutingRequestCollection<TMessage>(this.abortedMessages.ToList());
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Sends a routing request to the service route.
        /// </summary>
        /// <param name="message">
        /// The routing request to send to the service route.
        /// </param>
        public void SendMessage(MessageRoutingRequest<TMessage> message)
        {
            this.poolMonitor.NotifyPending();
            this.StartProcess();
            this.requestQueue.ProduceItem(message);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current 
        /// <see cref="T:SAF.MessageQueuing.PriorityQueuePool`2" />.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents the current <see cref="T:SAF.MessageQueuing.PriorityQueuePool`2" />.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return String.Format(ToStringFormat, this.Name, this.InstanceIdentifier);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the process is stopping.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the process is stopping; otherwise <c>false</c>.
        /// </returns>
        protected override bool IsStopping()
        {
            ////Trace.TraceInformation(
            ////    "[{0}]: Requests={1};Processed={2};Aborted={3}",
            ////    this,
            ////    this.poolMonitor.MessageRequests,
            ////    this.poolMonitor.MessagesProcessed,
            ////    this.abortedMessages.Count);
            return this.poolMonitor.MessageRequests == this.poolMonitor.MessagesProcessed; // + this.abortedMessages.Count;
        }

        /// <summary>
        /// Cancels the current process.
        /// </summary>
        protected override void CancelProcess()
        {
            this.requestQueue.Cancel();

            this.requestQueue.ItemsProduced -= this.SendMessagesToNextQueue;
            this.requestQueue.ProcessStopped -= this.HandleRequestQueueAbort;

            // Uncomment this if you like deadlocks.
            ////lock (this.queueLock)
            ////{
            foreach (var queue in this.activeQueues.Union(this.idleQueues))
            {
                queue.Value.Cancel();
                this.FinalizeQueue(queue.Value);
            }
            ////}
       
            this.activeQueues.Clear();
            this.idleQueues.Clear();
        }

        /// <summary>
        /// The create producer queue.
        /// </summary>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// A <see cref="T:SAF.ProcessEngine.ItemProducer`1"/> instance with the specified name and comparer.
        /// </returns>
        private static ItemProducer<MessageRoutingRequest<TMessage>> CreateProducerQueue(IComparer<TMessage> comparer, string name)
        {
            var producerName = String.Format(ProducerNameFormat, name, typeof(TMessage).ToRuntimeName());
            var routingRequestComparer = new MessageRoutingRequestComparer<TMessage>(comparer);
            var sortedCollection = new ConcurrentSortedCollection<MessageRoutingRequest<TMessage>>(routingRequestComparer);
            return new ItemProducer<MessageRoutingRequest<TMessage>>(producerName, sortedCollection);
        }

        /// <summary>
        /// Initializes the queue pool.
        /// </summary>
        private void Initialize()
        {
            this.ProcessStopped += this.TrimIdleQueues;
            this.requestQueue.ItemsProduced += this.SendMessagesToNextQueue;
            this.requestQueue.ProcessStopped += this.HandleRequestQueueAbort;
            var newQueue = this.CreateNewQueue();
            this.idleQueues.AddOrUpdate(newQueue.InstanceIdentifier, newQueue, (guid, queue) => newQueue);
            this.HighestConcurrency = Math.Max(this.HighestConcurrency, this.activeQueues.Count + this.idleQueues.Count);
        }

        #region Queue Operations

        /// <summary>
        /// Sends all waiting messages to next queue.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="itemsProducedEventArgs">
        /// The event data associated with the event.
        /// </param>
        private void SendMessagesToNextQueue(object sender, ItemsProducedEventArgs itemsProducedEventArgs)
        {
            while (this.requestQueue.ItemQueueConsumer.ConsumeNext())
            {
                var routingRequest = this.requestQueue.ItemQueueConsumer.Current;

                try
                {
                    this.SendMessageToQueue(routingRequest);
                }
                catch (DomainException ex)
                {
                    this.actionEventProxy.RecordAction(routingRequest.Message, ex);
                }
            }
        }

        /// <summary>
        /// Sends a routing request to the next queue.
        /// </summary>
        /// <param name="routingRequest">
        /// The routing request to send.
        /// </param>
        private void SendMessageToQueue(MessageRoutingRequest<TMessage> routingRequest)
        {
            try
            {
                TQueue nextQueue;

                lock (this.queueLock)
                {
                    bool idleAvailable = this.idleQueues.Count > 0;
                    bool activeAvailable = this.activeQueues.Count > 0;
                    var logPolicyActions = ConfigurationManager.AppSettings.IsTrue(LogPolicySetting);

                    if (idleAvailable)
                    {
                        // Select an idle queue.
                        var nextQueueId = this.idleQueues.OrderBy(QueueSelector, this.queueAvailabilityComparer).First().Key;
                        this.idleQueues.TryRemove(nextQueueId, out nextQueue);
                        this.activeQueues.AddOrUpdate(nextQueueId, nextQueue, (guid, queue) => nextQueue);

                        if (logPolicyActions)
                        {
                            var description = String.Format(ActionMessages.IdleQueueReusedAction, nextQueue);
                            this.actionEventProxy.RecordAction(this.poolMonitor, ActionMessages.IdleQueueReused, description);
                        }
                    }
                    else
                    {
                        var policyDecision = this.queuingPolicy.AllowQueueCreation(this);

                        if (logPolicyActions)
                        {
                            var actionName = policyDecision.IsApproved
                                                 ? ActionMessages.AllowQueueCreation
                                                 : ActionMessages.DenyQueueCreation;

                            this.actionEventProxy.RecordAction(this, actionName, policyDecision.Reason);
                        }

                        if (policyDecision.IsApproved)
                        {
                            // Make another queue.
                            nextQueue = this.CreateNewQueue();
                            this.activeQueues.AddOrUpdate(nextQueue.InstanceIdentifier, nextQueue, (guid, queue) => nextQueue);
                            this.HighestConcurrency = Math.Max(this.HighestConcurrency, this.activeQueues.Count + this.idleQueues.Count);
                        }
                        else if (activeAvailable)
                        {
                            // Select the most available active queue.
                            nextQueue = this.activeQueues.OrderBy(QueueSelector, this.queueAvailabilityComparer).First().Value;

                            if (logPolicyActions)
                            {
                                var description = String.Format(ActionMessages.ActiveQueueReuseDescription, nextQueue);
                                this.actionEventProxy.RecordAction(this.poolMonitor, ActionMessages.ActiveQueueReuse, description);
                            }
                        }
                        else
                        {
                            // Make a new queue.
                            nextQueue = this.CreateNewQueue();
                            this.activeQueues.AddOrUpdate(nextQueue.InstanceIdentifier, nextQueue, (guid, queue) => nextQueue);
                            this.HighestConcurrency = Math.Max(this.HighestConcurrency, this.activeQueues.Count + this.idleQueues.Count);

                            if (logPolicyActions)
                            {
                                var description = String.Format(ActionMessages.NewQueueUseDescription, nextQueue);
                                this.actionEventProxy.RecordAction(this.poolMonitor, ActionMessages.NewQueueUse, description);
                            }
                        }
                    }
                }

                nextQueue.SendMessage(routingRequest);
            }
            catch (DomainException ex)
            {
                this.actionEventProxy.RecordAction(routingRequest.Message, ex);
            }
        }

        /// <summary>
        /// Creates a new queue.
        /// </summary>
        /// <returns>
        /// A new queue of the type <typeparamref name="TQueue"/>.
        /// </returns>
        private TQueue CreateNewQueue()
        {
            var queue = this.factory.Create();
            queue.StartSubscription(this.poolMonitor);
            queue.RequestReceived += this.ProcessRequestEvent;
            queue.RequestCompleted += this.ProcessResponseEvent;
            queue.ProcessStopped += this.ProcessCompletedQueue;
            return queue;
        }

        /// <summary>
        /// Finalizes the queue.
        /// </summary>
        /// <param name="queue">
        /// The queue to finalize.
        /// </param>
        private void FinalizeQueue(TQueue queue)
        {
            queue.EndSubscription(this.poolMonitor);
            queue.RequestReceived -= this.ProcessRequestEvent;
            queue.RequestCompleted -= this.ProcessResponseEvent;
            queue.ProcessStopped -= this.ProcessCompletedQueue;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Triggers the <see cref="RequestReceived"/> event and processes the original event.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// Event data associated with the event.
        /// </param>
        private void ProcessRequestEvent(object sender, RequestEventArgs<TMessage> e)
        {
            // Hijack the queue route's event so that it reports back as the queue pool instead.
            this.OnRequestReceived(new RequestEventArgs<TMessage>(new MessageEntry<TMessage>(this, e.RequestEvent)));
        }

        /// <summary>
        /// Triggers the <see cref="RequestCompleted"/> event and processes the original event.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// Event data associated with the event.
        /// </param>
        private void ProcessResponseEvent(object sender, ResponseEventArgs<TMessage> e)
        {
            // Hijack the queue route's event so that it reports back as the queue pool instead.
            this.OnRequestCompleted(new ResponseEventArgs<TMessage>(new MessageExit<TMessage>(this, e.ResponseEvent)));
        }

        /// <summary>
        /// Removes idle queues from the pool.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The event data associated with the event.
        /// </param>
        private void ProcessCompletedQueue(object sender, ProcessStoppedEventArgs e)
        {
            var queue = (TQueue)sender;

            ////Trace.TraceInformation("[{0}] Stopping for {1}", this, e.EventError == null ? "idle" : e.EventError.ToString());
            var requestAbort = e.EventError as MessageRoutingAbortException<TMessage>;

            if (requestAbort != null)
            {
                ////Trace.TraceError(
                ////    "[{0}] Process aborted queue {1} on message {2}: {3}",
                ////    this,
                ////    sender,
                ////    requestAbort.AbortedItem,
                ////    requestAbort.Message);
                lock (this.queueLock)
                {
                    ////Trace.TraceInformation(
                    ////    "[{0}] ProcessCompletedQueue (pre): Active queues {1}; Idle queues: {2}",
                    ////    this,
                    ////    this.activeQueues.Count,
                    ////    this.idleQueues.Count);
                    TQueue removedQueue;

                    if (this.activeQueues.TryRemove(queue.InstanceIdentifier, out removedQueue))
                    {
                        ////Trace.TraceInformation("[{0}] Removing aborted queue {1}.", this, queue);
                        this.FinalizeQueue(removedQueue);
                    }

                    ////else
                    ////{
                    ////    Trace.TraceWarning("[{0}] Aborted queue {1} was not found in the active queue list.", this, queue);
                    ////}

                    ////Trace.TraceInformation(
                    ////    "[{0}] ProcessCompletedQueue (post): Active queues {1}; Idle queues: {2}",
                    ////    this,
                    ////    this.activeQueues.Count,
                    ////    this.idleQueues.Count);
                }

                this.ProcessAbortException(requestAbort);
            }
            else if (e.EventError != null)
            {
                ////Trace.TraceError("[{0}] Process unhandled exception for queue {1}: {2}", this, sender, e.EventError.Message);
                lock (this.queueLock)
                {
                    ////Trace.TraceInformation(
                    ////    "[{0}] ProcessCompletedQueue (pre): Active queues {1}; Idle queues: {2}",
                    ////    this,
                    ////    this.activeQueues.Count,
                    ////    this.idleQueues.Count);
                    TQueue removedQueue;

                    if (this.activeQueues.TryRemove(queue.InstanceIdentifier, out removedQueue))
                    {
                        ////Trace.TraceInformation("[{0}] Removing failed queue {1}.", this, queue);
                        this.FinalizeQueue(removedQueue);
                    }

                    ////else
                    ////{
                    ////    Trace.TraceWarning("[{0}] Failed queue {1} was not found in the active queue list.", this, queue);
                    ////}

                    ////Trace.TraceInformation(
                    ////    "[{0}] ProcessCompletedQueue (post): Active queues {1}; Idle queues: {2}",
                    ////    this,
                    ////    this.activeQueues.Count,
                    ////    this.idleQueues.Count);
                }

                foreach (var message in queue.QueuedMessages)
                {
                    this.actionEventProxy.RecordAction(message, e.EventError);
                    this.abortedMessages.Add(message);
                }
            }
            else
            {
                // If this queue exits without error and there is another free queue then cancel and remove this one.
                lock (this.queueLock)
                {
                    ////Trace.TraceInformation(
                    ////    "[{0}] ProcessCompletedQueue (pre): Active queues {1}; Idle queues: {2}",
                    ////    this,
                    ////    this.activeQueues.Count,
                    ////    this.idleQueues.Count);
                    TQueue removedQueue;

                    if (this.activeQueues.TryRemove(queue.InstanceIdentifier, out removedQueue))
                    {
                        ////Trace.TraceInformation("[{0}] Removing active queue {1}.", this, queue);
                        this.idleQueues.AddOrUpdate(removedQueue.InstanceIdentifier, removedQueue, (guid, queue1) => removedQueue);

                        ////Trace.TraceInformation("[{0}] Adding idle queue {1}.", this, queue);
                    }

                    ////else
                    ////{
                    ////    Trace.TraceWarning("[{0}] Queue {1} was not found in the active queue list.", this, queue);
                    ////}

                    ////Trace.TraceInformation(
                    ////    "[{0}] ProcessCompletedQueue (post): Active queues {1}; Idle queues: {2}",
                    ////    this,
                    ////    this.activeQueues.Count,
                    ////    this.idleQueues.Count);
                }
            }

            this.StopProcessIfStopping(null);
        }

        /// <summary>
        /// Handles messages in the request queue if it is aborted.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="eventArgs">
        /// The event data associated with the event.
        /// </param>
        private void HandleRequestQueueAbort(object sender, ProcessStoppedEventArgs eventArgs)
        {
            var requestAbort = eventArgs.EventError as MessageRoutingAbortException<TMessage>;

            if (requestAbort != null)
            {
                this.poolMonitor.NotifyQueueAbort();
                this.ProcessAbortException(requestAbort);
            }
            else if (eventArgs.EventError != null)
            {
                this.poolMonitor.NotifyQueueAbort();
                this.actionEventProxy.RecordAction(this, eventArgs.EventError);

                foreach (var queuedItem in this.requestQueue.QueuedItems)
                {
                    this.abortedMessages.Add(queuedItem);
                }
            }
        }

        /// <summary>
        /// Processes an abort exception.
        /// </summary>
        /// <param name="requestAbort">
        /// The queue aborted exception to handle. 
        /// </param>
        private void ProcessAbortException(MessageRoutingAbortException<TMessage> requestAbort)
        {
            this.actionEventProxy.RecordAction(requestAbort.AbortedItem, requestAbort);
            this.abortedMessages.Add(requestAbort.AbortedItem);

            foreach (var pendingItem in requestAbort.PendingItems)
            {
                if (this.requestQueue.Canceled)
                {
                    this.abortedMessages.Add(pendingItem);
                }
                else
                {
                    ////Trace.TraceInformation("[{0}] Resending aborted message request {1}.", this, pendingItem);
                    this.requestQueue.ProduceItem(pendingItem);                    
                }
            }
        }

        /// <summary>
        /// Trims idle queues from the queue pool once the pool is no longer active.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="eventArgs">
        /// The process stopped event args.
        /// </param>
        private void TrimIdleQueues(object sender, ProcessStoppedEventArgs eventArgs)
        {
            // TODO: This does not work currently.
            if (!this.queuingPolicy.TrimIdleQueues)
            {
                return;
            }

            lock (this.queueLock)
            {
                if (this.idleQueues.Count < 2)
                {
                    return;
                }

                ////Trace.TraceInformation(
                ////    "[{0}] TrimIdleQueues (pre): Active queues {1}; Idle queues: {2}",
                ////    this,
                ////    this.activeQueues.Count,
                ////    this.idleQueues.Count);
                foreach (var idleQueue in this.idleQueues.Skip(1).ToList())
                {
                    TQueue removedQueue;

                    if (!this.idleQueues.TryRemove(idleQueue.Key, out removedQueue))
                    {
                        continue;
                    }

                    ////Trace.TraceInformation("[{0}] Removing idle queue {1}.", this, idleQueue);
                    this.FinalizeQueue(removedQueue);
                    removedQueue.Cancel();
                }

                ////Trace.TraceInformation(
                ////    "[{0}] TrimIdleQueues (post): Active queues {1}; Idle queues: {2}",
                ////    this,
                ////    this.activeQueues.Count,
                ////    this.idleQueues.Count);
            }
        }

        #endregion

        #region Event Invokers

        /// <summary>
        /// Triggers the <see cref="RequestCompleted"/> event.
        /// </summary>
        /// <param name="e">
        /// Event data associated with the event.
        /// </param>
        private void OnRequestCompleted(ResponseEventArgs<TMessage> e)
        {
            EventHandler<ResponseEventArgs<TMessage>> handler = this.RequestCompleted;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Triggers the <see cref="RequestReceived"/> event.
        /// </summary>
        /// <param name="e">
        /// Event data associated with the event.
        /// </param>
        private void OnRequestReceived(RequestEventArgs<TMessage> e)
        {
            EventHandler<RequestEventArgs<TMessage>> handler = this.RequestReceived;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #endregion
    }
}