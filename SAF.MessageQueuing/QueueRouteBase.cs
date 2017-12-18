// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueueRouteBase.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.MessageQueuing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using SAF.ActionTracking;
    using SAF.Core;
    using SAF.ProcessEngine;
    using SAF.StringResources;

    /// <summary>
    /// The base class for all queue routes.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of message processed by the route. 
    /// </typeparam>
    public abstract class QueueRouteBase<TMessage> : ProcessEngineBase, IPriorityQueueRoute<TMessage>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// The to string format.
        /// </summary>
        private const string ToStringFormat = "{0} [ID: {1}]";

        /// <summary>
        /// The request queue name.
        /// </summary>
        private const string RequestQueueName = "{0} Request Queue";

        /// <summary>
        /// The action event proxy.
        /// </summary>
        private readonly IActionEventProxy actionEventProxy;

        /// <summary>
        /// The request queue.
        /// </summary>
        private readonly ItemProducer<MessageRoutingRequest<TMessage>> requestQueue;

        /// <summary>
        /// The queue monitor.
        /// </summary>
        private readonly QueueMonitor<TMessage> queueMonitor;

        /// <summary>
        /// The pool monitor.
        /// </summary>
        private readonly List<QueueMonitor<TMessage>> subscribers = new List<QueueMonitor<TMessage>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueRouteBase{TMessage}"/> class. 
        /// Initializes a new instance of the <see cref="T:SAF.MessageQueuing.QueueRouteBase`1"/> class.
        /// </summary>
        /// <param name="actionEventProxy">
        /// The action Event Proxy.
        /// </param>
        /// <param name="profileProvider">
        /// The profile provider for the message.
        /// </param>
        /// <param name="name">
        /// The name of the queue.
        /// </param>
        protected QueueRouteBase(IActionEventProxy actionEventProxy, IRoutingProfileProvider<TMessage> profileProvider, string name)
            : base(name)
        {
            if (actionEventProxy == null)
            {
                throw new ArgumentNullException("actionEventProxy");
            }

            if (profileProvider == null)
            {
                throw new ArgumentNullException("profileProvider");
            }

            this.actionEventProxy = actionEventProxy;
            this.InstanceIdentifier = Guid.NewGuid();
            string requestName = String.Format(RequestQueueName, this.Name);
            this.requestQueue = this.CreateProducer(profileProvider.PriorityComparer, requestName);
            this.queueMonitor = new QueueMonitor<TMessage>(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueRouteBase{TMessage}"/> class. 
        /// Initializes a new instance of the <see cref="T:SAF.MessageQueuing.QueueRouteBase`1"/> class.
        /// </summary>
        /// <param name="actionEventProxy">
        /// The action Event Proxy.
        /// </param>
        /// <param name="comparer">
        /// The comparer for the message priority.
        /// </param>
        /// <param name="name">
        /// The name of the queue.
        /// </param>
        protected QueueRouteBase(IActionEventProxy actionEventProxy, IComparer<TMessage> comparer, string name)
            : base(name)
        {
            if (actionEventProxy == null)
            {
                throw new ArgumentNullException("actionEventProxy");
            }

            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            this.actionEventProxy = actionEventProxy;
            this.InstanceIdentifier = Guid.NewGuid();
            string requestName = String.Format(RequestQueueName, this.Name);
            this.requestQueue = this.CreateProducer(comparer, requestName);
            this.queueMonitor = new QueueMonitor<TMessage>(this);
        }

        /// <summary>
        /// Occurs when a queue processing request is received.
        /// </summary>
        public virtual event EventHandler<RequestEventArgs<TMessage>> RequestReceived;

        /// <summary>
        /// Occurs when a queue processing request is completed.
        /// </summary>
        public virtual event EventHandler<ResponseEventArgs<TMessage>> RequestCompleted;

        /// <summary>
        /// Gets the instance identifier.
        /// </summary>
        public Guid InstanceIdentifier { get; private set; }

        /// <summary>
        /// Gets the state of the current queue.
        /// </summary>
        public IPriorityQueueState QueueState
        {
            get
            {
                return this.queueMonitor;
            }
        }

        /// <summary>
        /// Gets the messages waiting in the queue.
        /// </summary>
        public MessageRoutingRequestCollection<TMessage> QueuedMessages
        {
            get
            {
                return new MessageRoutingRequestCollection<TMessage>(this.requestQueue.QueuedItems.ToList());
            }
        }

        /// <summary>
        /// Subscribes a queue monitor with the current route.
        /// </summary>
        /// <param name="monitor">
        /// The queue monitor to subscribe.
        /// </param>
        public void StartSubscription(QueueMonitor<TMessage> monitor)
        {
            lock (this.subscribers)
            {
                this.subscribers.Add(monitor);
            }
        }

        /// <summary>
        /// Ends the subscription of a queue monitor to the current route.
        /// </summary>
        /// <param name="monitor">
        /// The queue monitor to unsubscribe.
        /// </param>
        public void EndSubscription(QueueMonitor<TMessage> monitor)
        {
            lock (this.subscribers)
            {
                this.subscribers.Remove(monitor);
            }
        }

        /// <summary>
        /// Sends a message to the service route.
        /// </summary>
        /// <param name="message">
        /// The message to send to the service route.
        /// </param>
        /// <exception cref="ComponentAbortedException">
        /// The queue has been aborted and is no longer taking requests.
        /// </exception>
        public void SendMessage(MessageRoutingRequest<TMessage> message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (this.queueMonitor.QueueAborted)
            {
                throw new ComponentAbortedException(String.Format(ErrorMessages.ComponentAbortedAndRefusingRequests, this));
            }

            // We do not notify the subscribers that we have a pending message. This allows them to notify pending within their own 
            // send method and avoid race conditions.
            this.queueMonitor.NotifyPending();
            this.StartProcess();
            this.requestQueue.ProduceItem(message);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:SAF.MessageQueuing.PriorityQueueRoute`1"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:SAF.MessageQueuing.PriorityQueueRoute`1"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return String.Format(ToStringFormat, this.GetType().Name, this.InstanceIdentifier);
        }

        /// <summary>
        /// Processes a message routing request.
        /// </summary>
        /// <param name="routingRequest">
        /// The routing request.
        /// </param>
        protected abstract void ProcessMessage(MessageRoutingRequest<TMessage> routingRequest);

        /// <summary>
        /// Cancels the current process.
        /// </summary>
        protected override void CancelProcess()
        {
            this.requestQueue.Cancel();
            this.requestQueue.ProcessStarted -= this.CheckForProcessStarted;
            this.requestQueue.ItemsProduced -= this.ProcessMessages;
            this.requestQueue.ProcessStopped -= this.CheckForProcessStopped;
        }

        /// <summary>
        /// Determines whether the process is stopping.
        /// </summary>
        /// <returns>True if the process is stopping, otherwise false.</returns>
        protected override bool IsStopping()
        {
            return this.queueMonitor.MessagesProcessed == this.queueMonitor.MessageRequests;
        }

        /// <summary>
        /// Creates the message producer.
        /// </summary>
        /// <param name="messagePriorityComparer">
        /// The message priority comparer.
        /// </param>
        /// <param name="requestQueueName">
        /// The request queue name.
        /// </param>
        /// <returns>
        /// A <see cref="T:SAF.ProcessEngine.ItemProducer`1"/> instance with the specified priority comparer and request queue name.
        /// </returns>
        private ItemProducer<MessageRoutingRequest<TMessage>> CreateProducer(
            IComparer<TMessage> messagePriorityComparer, 
            string requestQueueName)
        {
            var comparer = new MessageRoutingRequestComparer<TMessage>(messagePriorityComparer);
            var messageRoutingRequests = new ConcurrentSortedCollection<MessageRoutingRequest<TMessage>>(comparer);

            var producer = new ItemProducer<MessageRoutingRequest<TMessage>>(requestQueueName, messageRoutingRequests)
            {
                MaxQueueLength = Int64.MaxValue
            };

            // Check for process started if the directive producer is starting.
            producer.ProcessStarted += this.CheckForProcessStarted;

            // Process messages provided by callers of the ProduceItem method.
            producer.ItemsProduced += this.ProcessMessages;

            // Check to see if the process has stopped.
            producer.ProcessStopped += this.CheckForProcessStopped;
            return producer;
        }

        /// <summary>
        /// Handles the response from the message processing action.
        /// </summary>
        /// <param name="entryEvent">
        /// The entry event.
        /// </param>
        /// <param name="error">
        /// The error, if any, associated with message processing.
        /// </param>
        private void HandleResponse(MessageEntry<TMessage> entryEvent, Exception error)
        {
            var exitEvent = error == null ? new MessageExit<TMessage>(entryEvent) : new MessageExit<TMessage>(entryEvent, error);
            this.queueMonitor.NotifyResponse(exitEvent);

            lock (this.subscribers)
            {
                foreach (var subscriber in this.subscribers)
                {
                    subscriber.NotifyResponse(exitEvent);
                }
            }

            this.OnRequestCompleted(new ResponseEventArgs<TMessage>(exitEvent));
        }

        /// <summary>
        /// Checks whether the process has started.
        /// </summary>
        /// <param name="sender">
        /// The sender of the <see cref="ProcessStartedEventArgs"/> event.
        /// </param>
        /// <param name="e">
        /// <see cref="ProcessStartedEventArgs"/> associated with the event.
        /// </param>
        private void CheckForProcessStarted(object sender, ProcessStartedEventArgs e)
        {
            this.StartProcess();
        }

        /// <summary>
        /// Processes messages produced by the request queue.
        /// </summary>
        /// <param name="sender">
        /// The sender of the <see cref="ItemsProducedEventArgs"/> event.
        /// </param>
        /// <param name="e">
        /// <see cref="ItemsProducedEventArgs"/> associated with the event.
        /// </param>
        private void ProcessMessages(object sender, ItemsProducedEventArgs e)
        {
            while (this.Canceled == false && this.requestQueue.ItemQueueConsumer.ConsumeNext())
            {
                var current = this.requestQueue.ItemQueueConsumer.Current;
                var entryEvent = new MessageEntry<TMessage>(this, current);
                this.queueMonitor.NotifyReceipt(entryEvent);

                lock (this.subscribers)
                {
                    foreach (var subscriber in this.subscribers)
                    {
                        subscriber.NotifyReceipt(entryEvent);
                    }
                }

                try
                {
                    this.OnRequestReceived(new RequestEventArgs<TMessage>(entryEvent));
                }
                catch (Exception ex)
                {
                    var messageExit = new MessageExit<TMessage>(entryEvent, ex);
                    this.queueMonitor.NotifyResponse(messageExit);

                    lock (this.subscribers)
                    {
                        foreach (var subscriber in this.subscribers)
                        {
                            subscriber.NotifyResponse(messageExit);
                        }
                    }

                    throw;
                } 
                
                Exception error = null;

                try
                {
                    this.ProcessMessage(current);
                }
                catch (DomainException ex)
                {
                    error = ex;
                }
                catch (Exception ex)
                {
                    Trace.TraceError("[{0}] Failing queue on message '{1}': {2}", this, current.Message, ex.Message);
                    error = ex;
                    throw;
                }
                finally
                {
                    this.HandleResponse(entryEvent, error);
                }
            }
        }

        /// <summary>
        /// Checks whether the process has stopped.
        /// </summary>
        /// <param name="sender">
        /// The sender of the <see cref="ProcessStoppedEventArgs"/> event.
        /// </param>
        /// <param name="e">
        /// <see cref="ProcessStoppedEventArgs"/> associated with the event.
        /// </param>
        private void CheckForProcessStopped(object sender, ProcessStoppedEventArgs e)
        {
            var responseAbortError = e.EventError as QueueAbortException<MessageRoutingRequest<TMessage>>;
            var error = e.EventError;

            if (responseAbortError != null)
            {
                this.actionEventProxy.RecordAction(responseAbortError.AbortedItem, responseAbortError);
                this.queueMonitor.NotifyQueueAbort();
                var pendingItems = new List<MessageRoutingRequest<TMessage>>(responseAbortError.PendingItems);

                error = new MessageRoutingAbortException<TMessage>(
                    responseAbortError.Message, 
                    responseAbortError.AbortedItem, 
                    new MessageRoutingRequestCollection<TMessage>(pendingItems), 
                    responseAbortError);
            }
            else if (e.EventError != null)
            {
                this.actionEventProxy.RecordAction(this, e.EventError);
                this.queueMonitor.NotifyQueueAbort();
                this.requestQueue.Cancel();
                string message = String.Format(ErrorMessages.ComponentExitingWithUnhandledError, this, e.EventError.Message);
                error = new MessageRoutingAbortException<TMessage>(
                    message, 
                    null, 
                    new MessageRoutingRequestCollection<TMessage>(this.requestQueue.QueuedItems.ToList()), 
                    e.EventError);
            }

            this.StopProcessIfStopping(error);
        }

        /// <summary>
        /// The on request received.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnRequestReceived(RequestEventArgs<TMessage> e)
        {
            var handler = this.RequestReceived;

            if (handler == null)
            {
                return;
            }

            handler(this, e);
        }

        /// <summary>
        /// The on request completed.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnRequestCompleted(ResponseEventArgs<TMessage> e)
        {
            var handler = this.RequestCompleted;

            if (handler == null)
            {
                return;
            }

            handler(this, e);
        }
    }
}