// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageRouter.cs" company="TractManager, Inc.">
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
    using System.Threading;
    using System.Threading.Tasks;

    using SAF.ActionTracking;
    using SAF.Core;
    using SAF.Data;
    using SAF.ProcessEngine;
    using SAF.StringResources;

    /// <summary>
    /// Routes documents between services specified by the service routes initialized using the constructor.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of <see cref="IPriorityMessage"/> handled by the router.
    /// </typeparam>
    public sealed class MessageRouter<TMessage> : ProcessEngineBase, IMessageRouter<TMessage>
        where TMessage : IEquatable<TMessage>, IComparable, IComparable<TMessage>
    {
        #region Constants

        /// <summary>
        /// The to string format.
        /// </summary>
        private const string ToStringFormat = "{0} [{1}]";

        /// <summary>
        /// The log routing actions.
        /// </summary>
        private const string LogRoutingActions = "LogMessageRoutingActions";

        /// <summary>
        /// The routing queue name format.
        /// </summary>
        private const string RoutingQueueNameFormat = "{0}RoutingQueue";

        /// <summary>
        /// The response queue name format.
        /// </summary>
        private const string ResponseQueueNameFormat = "{0}ResponseQueue";

        #endregion

        #region Fields

        /// <summary>
        /// The profile provider.
        /// </summary>
        private readonly IRoutingProfileProvider<TMessage> profileProvider;

        /// <summary>
        /// The service route container.
        /// </summary>
        private readonly IServiceRouteContainer<TMessage> serviceRouteContainer;

        /// <summary>
        /// The failed route analyzer.
        /// </summary>
        private readonly IRoutingFailurePolicy<TMessage> failurePolicy;

        /// <summary>
        /// The repository factory.
        /// </summary>
        private readonly IRoutingRepositoryFactory<TMessage> repositoryFactory;

        /// <summary>
        /// The repository provider factory.
        /// </summary>
        private readonly IRepositoryProviderFactory repositoryProviderFactory;

        /// <summary>
        /// The instance identifier.
        /// </summary>
        private readonly Guid instanceIdentifier = Guid.NewGuid();

        /// <summary>
        /// The registered routes.
        /// </summary>
        private readonly ConcurrentBag<IServiceRoute<TMessage>> registeredRoutes = new ConcurrentBag<IServiceRoute<TMessage>>();

        /// <summary>
        /// The open requests.
        /// </summary>
        private readonly List<MessageRoutingRequest<TMessage>> openRequests = new List<MessageRoutingRequest<TMessage>>();

        /// <summary>
        /// The duplicate equality comparer.
        /// </summary>
        private readonly MessageDuplicateEqualityComparer<TMessage> duplicateEqualityComparer; 

        /// <summary>
        /// The message lock.
        /// </summary>
        private readonly object messageLock = new object();

        /// <summary>
        /// The provider queue pool.
        /// </summary>
        private readonly IQueuePool<TMessage> requestPool;

        /// <summary>
        /// The routing producer.
        /// </summary>
        private readonly ItemProducer<MessageRoutingRequest<TMessage>> routingProducer;

        /// <summary>
        /// The response producer.
        /// </summary>
        private readonly ItemProducer<MessageExit<TMessage>> responseProducer;

        /// <summary>
        /// The action event proxy.
        /// </summary>
        private readonly IActionEventProxy actionEventProxy;

        /// <summary>
        /// Determines whether the router is currently saving an item.
        /// </summary>
        private bool messagePending;

        /// <summary>
        /// Indicates whether the current instance is disposed.
        /// </summary>
        private bool isDisposed;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRouter{TMessage}"/> class. 
        /// Initializes a new instance of the <see cref="T:SAF.MessageQueuing.MessageRouter`1"/> class.
        /// </summary>
        /// <param name="profileProvider">
        /// The routing profile provider.
        /// </param>
        /// <param name="serviceRouteContainer">
        /// The service route container.
        /// </param>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="actionEventProxy">
        /// The action event proxy.
        /// </param>
        /// <param name="repositoryProviderFactory">
        /// The repository provider factory.
        /// </param>
        public MessageRouter(
            IRoutingProfileProvider<TMessage> profileProvider, 
            IServiceRouteContainer<TMessage> serviceRouteContainer, 
            IRoutingRepositoryFactory<TMessage> repositoryFactory, 
            IActionEventProxy actionEventProxy, 
            IRepositoryProviderFactory repositoryProviderFactory)
        {
            if (profileProvider == null)
            {
                throw new ArgumentNullException("profileProvider");
            }

            if (serviceRouteContainer == null)
            {
                throw new ArgumentNullException("serviceRouteContainer");
            }

            if (repositoryFactory == null)
            {
                throw new ArgumentNullException("repositoryFactory");
            }

            if (repositoryProviderFactory == null)
            {
                throw new ArgumentNullException("repositoryProviderFactory");
            }

            if (profileProvider.FailureRoute == null)
            {
                throw new ArgumentException(ValidationMessages.FailureRouteNotSpecified);
            }

            this.profileProvider = profileProvider;
            this.serviceRouteContainer = serviceRouteContainer;
            this.repositoryFactory = repositoryFactory;
            this.actionEventProxy = actionEventProxy;
            this.repositoryProviderFactory = repositoryProviderFactory;
            this.duplicateEqualityComparer =
                new MessageDuplicateEqualityComparer<TMessage>(this.profileProvider.DuplicateEqualityComparer);

            this.failurePolicy = this.profileProvider.FailurePolicy;
            this.profileProvider.FailureRoute.RequestReceived += this.UpdateMessageLocation;
            this.profileProvider.FailureRoute.RequestCompleted += this.FinalizeMessage;

            this.registeredRoutes.Add(this.profileProvider.FailureRoute);

            this.requestPool = new PendingMessageQueuePool<TMessage>(
                this.profileProvider, 
                StaticQueuingPolicy.DefaultPolicy, 
                this.actionEventProxy, 
                this.profileProvider.PendingMessageQueueName);

            // This will allow location of the request pool.
            this.serviceRouteContainer.Register(this.requestPool);

            this.requestPool.RequestReceived += this.UpdateMessageLocation;
            this.requestPool.RequestCompleted += this.NotifyMessageRequestReady;

            var routingQueueName = String.Format(RoutingQueueNameFormat, typeof(TMessage));
            this.routingProducer = new ItemProducer<MessageRoutingRequest<TMessage>>(
                routingQueueName, 
                new ConcurrentSortedCollection<MessageRoutingRequest<TMessage>>(
                    new MessageRoutingRequestComparer<TMessage>(profileProvider.PriorityComparer)));

            this.routingProducer.ItemsProduced += this.ConsumeRoutingRequests;
            this.routingProducer.ProcessStopped += this.HandleRoutingStopped;

            var responseQueueName = String.Format(ResponseQueueNameFormat, typeof(TMessage));
            this.responseProducer = new ItemProducer<MessageExit<TMessage>>(
                responseQueueName, 
                new ConcurrentSortedCollection<MessageExit<TMessage>>(
                    new MessageExitComparer<TMessage>(profileProvider.PriorityComparer)));

            this.responseProducer.ItemsProduced += this.ConsumeRoutingResponses;
            this.responseProducer.ProcessStopped += this.HandleRoutingStopped;
        }

        #endregion

        /// <summary>
        /// Gets the active messages for the current router.
        /// </summary>
        public MessageRoutingRequestCollection<TMessage> ActiveMessages
        {
            get
            {
                return new MessageRoutingRequestCollection<TMessage>(this.openRequests);
            }
        }

        /// <summary>
        /// Gets the aborted messages for all active service routes of the <see cref="T:SAF.MessageQueuing.IQueuePool`1"/> type.
        /// </summary>
        [DoNotLog]
        public MessageRoutingRequestCollection<TMessage> AbortedMessages
        {
            get
            {
                if (this.isDisposed)
                {
                    throw new ObjectDisposedException(Convert.ToString(this));
                }

                List<IServiceRoute<TMessage>> serviceRoutes;

                lock (this.registeredRoutes)
                {
                    serviceRoutes = this.registeredRoutes.ToList();
                }

                return
                    new MessageRoutingRequestCollection<TMessage>(
                        serviceRoutes.OfType<IQueuePool<TMessage>>()
                            .SelectMany(x => x.AbortedMessages)
                            .Union(this.requestPool.AbortedMessages)
                            .ToList());
            }
        }

        #region Public Methods and Operators

        /// <summary>
        /// Starts document processing for a processing request at the initial service route.
        /// </summary>
        /// <param name="routingRequest">
        /// The routing request for the message router.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="routingRequest"/> is null.
        /// </exception>
        /// <exception cref="BusinessException">
        /// The message has already been sent to the router.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// The router has been disposed.
        /// </exception>
        public void Send(MessageRoutingRequest<TMessage> routingRequest)
        {
            if (routingRequest == null)
            {
                throw new ArgumentNullException("routingRequest");
            }

            lock (this.messageLock)
            {
                if (this.Canceled)
                {
                    throw new ComponentAbortedException(String.Format(ErrorMessages.ComponentAbortedAndRefusingRequests, this));
                }
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(Convert.ToString(this));
            }

            // Mark the message as pending to prevent the router from stopping mid-dispatch. Then, cancel the duplicates if any to 
            // ensure the most recent message is getting processed. If another message supersedes this one, an exception will be 
            // thrown. Only evaluate the message if it succeeded and depend on the caller to handle other exceptions.
            lock (this.messageLock)
            {
                try
                {
                    this.messagePending = true;
                    this.CancelDuplicates(routingRequest);

                    var messageTransaction = routingRequest.Message as ITransactionContext;

                    if (messageTransaction == null || messageTransaction.TransactionProvider == null)
                    {
                        using (var repositoryProvider = this.repositoryProviderFactory.Create())
                        {
                            var routingRepository = this.repositoryFactory.Create(repositoryProvider);
                            routingRepository.SaveMessage(routingRequest.Message);
                        }
                    }
                    else
                    {
                        var routingRepository = this.repositoryFactory.Create(messageTransaction.TransactionProvider);
                        routingRepository.SaveMessage(routingRequest.Message);
                    }
                }
                catch
                {
                    this.messagePending = false;
                    throw;
                }

                try
                {
                    routingRequest.RoutingNotification.OnMessageEvaluated(new MessageEvaluation<TMessage>(routingRequest));
                }
                catch (Exception ex)
                {
                    // Fail and terminate because we've saved the message.
                    routingRequest.FailRequest(ex);
                    this.TerminateRequest(routingRequest);
                    throw;
                }

                try
                {
                    this.SendRequest(routingRequest);
                }
                catch (Exception ex)
                {
                    // Fail and terminate because we've saved the message. Rethrow as MessageDispatchException to indicate the 
                    // evaluation passed.
                    routingRequest.FailRequest(ex);
                    this.TerminateRequest(routingRequest);
                    throw new MessageDispatchException<TMessage>(ex.Message, routingRequest, ex);
                }
                finally
                {
                    this.messagePending = false;
                }
            }
        }

        /// <summary>
        /// Requeues a pending or delivered message routing request.
        /// </summary>
        /// <param name="message">
        /// The message to re-queue.
        /// </param>
        /// <param name="notification">
        /// The routing notification.
        /// </param>
        /// <exception cref="ObjectDisposedException">
        /// The router has been disposed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="notification"/> is null.
        /// </exception>
        /// <exception cref="ComponentAbortedException">
        /// The component has been canceled or aborted.
        /// </exception>
        /// <exception cref="BusinessException">
        /// The request has not already been saved in the routing repository.
        /// </exception>
        public void Requeue(TMessage message, INotifyMessageRouted<TMessage> notification)
        {
            if (Evaluate.IsNull(message))
            {
                throw new ArgumentNullException("message");
            }

            if (notification == null)
            {
                throw new ArgumentNullException("notification");
            }

            if (this.Canceled)
            {
                throw new ComponentAbortedException(String.Format(ErrorMessages.ComponentAbortedAndRefusingRequests, this));
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(Convert.ToString(this));
            }

            lock (this.messageLock)
            {
                var routingRequest = this.FindActiveRequest(message);

                if (routingRequest == null)
                {
                    this.messagePending = true;

                    try
                    {
                        var messageTransaction = message as ITransactionContext;

                        if (messageTransaction == null || messageTransaction.TransactionProvider == null)
                        {
                            using (var repositoryProvider = this.repositoryProviderFactory.Create())
                            {
                                routingRequest = this.OpenSavedRequest(message, notification, repositoryProvider);
                            }
                        }
                        else
                        {
                            routingRequest = this.OpenSavedRequest(message, notification, messageTransaction.TransactionProvider);
                        }

                        if (routingRequest.CurrentLocation != null && routingRequest.CurrentLocation.ServiceRoute == null)
                        {
                            routingRequest.CurrentLocation.ResolveRoute(this.serviceRouteContainer);
                        }
                    }
                    catch
                    {
                        this.messagePending = false;
                        throw;
                    }

                    // Always fail the request if something goes wrong.
                    try
                    {
                        this.CancelDuplicates(routingRequest);
                        routingRequest.ClearFailure();
                        routingRequest.RoutingNotification.OnMessageEvaluated(new MessageEvaluation<TMessage>(routingRequest));
                        this.SendRequest(routingRequest);
                    }
                    catch (Exception ex)
                    {
                        routingRequest.FailRequest(ex);
                        this.TerminateRequest(routingRequest);
                        throw new MessageDispatchException<TMessage>(ex.Message, routingRequest, ex);
                    }
                    finally
                    {
                        this.messagePending = false;
                    }
                }
                else
                {
                    // Request a restart of the message.
                    routingRequest.RequestRestart();
                }
            }
        }

        /// <summary>
        /// Cancels message routing for the specified routing status.
        /// </summary>
        /// <param name="routingStatus">
        /// The message routing status.
        /// </param>
        /// <param name="notification">
        /// The routing notification.
        /// </param>
        /// <exception cref="ObjectDisposedException">
        /// The router has been disposed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="routingStatus"/> or <paramref name="notification"/> is null.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// The component has been disposed.
        /// </exception>
        /// <exception cref="ComponentAbortedException">
        /// The component has been canceled or aborted.
        /// </exception>
        /// <exception cref="BusinessException">
        /// The request has not already been saved in the routing repository, or has already been delivered.
        /// </exception>
        public void CancelRequest(IRoutingStatus<TMessage> routingStatus, INotifyMessageRouted<TMessage> notification)
        {
            if (routingStatus == null)
            {
                throw new ArgumentNullException("routingStatus");
            }

            if (notification == null)
            {
                throw new ArgumentNullException("notification");
            }

            if (this.Canceled)
            {
                throw new ComponentAbortedException(String.Format(ErrorMessages.ComponentAbortedAndRefusingRequests, this));
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(Convert.ToString(this));
            }

            if (routingStatus.ServiceRoute == null)
            {
                routingStatus.ResolveRoute(this.serviceRouteContainer);
            }

            using (var repositoryProvider = this.repositoryProviderFactory.Create())
            {
                var routingRepository = this.repositoryFactory.Create(repositoryProvider);

                if (routingRepository.IsSaved(routingStatus.Message) == false)
                {
                    throw new BusinessException(routingStatus, ValidationMessages.PendingRequestCouldNotBeFound);
                }

                if (routingRepository.IsDelivered(routingStatus.Message))
                {
                    throw new BusinessException(routingStatus, ValidationMessages.DeliveryAlreadyCompleted);
                }
            }

            var message = routingStatus.Message;

            lock (this.messageLock)
            {
                MessageRoutingRequest<TMessage> activeRequest = this.FindActiveRequest(message);

                if (activeRequest == null)
                {
                    // Re-animate the request.
                    activeRequest = new MessageRoutingRequest<TMessage>(routingStatus, notification);
                    activeRequest.RoutingNotification.OnMessageEvaluated(new MessageEvaluation<TMessage>(activeRequest));
                    activeRequest.Cancel();
                    this.TerminateRequest(activeRequest);
                }
                else
                {
                    activeRequest.Cancel();
                }
            }
        }

        /// <summary>
        /// Determines if the specified message is actively routing.
        /// </summary>
        /// <param name="message">
        /// The message to locate.
        /// </param>
        /// <returns>
        /// <c>true</c> if the message is actively routing; otherwise, <c>false</c>.
        /// </returns>
        public bool IsActive(TMessage message)
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(Convert.ToString(this));
            }

            return this.FindActiveRequest(message) != null;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:SAF.MessageQueuing.MessageRouter`1" />.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents the current <see cref="T:SAF.MessageQueuing.MessageRouter`1" />.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return String.Format(ToStringFormat, this.GetType().ToRuntimeName(), this.instanceIdentifier);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.isDisposed = true;
            this.actionEventProxy.RecordAction(this);
            this.CancelProcess();
        }

        #endregion

        #region Methods

        #region ProcessEngine Overrides

        /// <summary>
        /// Determines whether the process is stopping.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the process is stopping; otherwise <c>false</c>.
        /// </returns>
        protected override bool IsStopping()
        {
            lock (this.messageLock)
            {
                return !this.messagePending 
                       && !this.requestPool.IsBusy 
                       && !this.routingProducer.IsBusy
                       && !this.responseProducer.IsBusy;
            }
        }

        /// <summary>
        /// Cancels the current process.
        /// </summary>
        protected override void CancelProcess()
        {
            this.actionEventProxy.RecordAction(this);

            lock (this.registeredRoutes)
            {
                IServiceRoute<TMessage> removedRoute;

                while (this.registeredRoutes.TryTake(out removedRoute))
                {
                    ////Trace.TraceInformation("Removing events for '{0}'", removedRoute);
                    removedRoute.RequestReceived -= this.UpdateMessageLocation;
                    removedRoute.RequestCompleted -= this.HandleMessageReturn;
                }
            }

            this.profileProvider.FailureRoute.RequestReceived -= this.UpdateMessageLocation;
            this.profileProvider.FailureRoute.RequestCompleted -= this.FinalizeMessage;

            this.requestPool.Cancel();

            this.serviceRouteContainer.Remove(this.requestPool);
            this.requestPool.RequestReceived -= this.UpdateMessageLocation;
            this.requestPool.RequestCompleted -= this.NotifyMessageRequestReady;

            this.routingProducer.Cancel();
            this.responseProducer.Cancel();
            this.routingProducer.ItemsProduced -= this.ConsumeRoutingRequests;
            this.routingProducer.ProcessStopped -= this.HandleRoutingStopped;
            this.responseProducer.ItemsProduced -= this.ConsumeRoutingResponses;
            this.responseProducer.ProcessStopped -= this.HandleRoutingStopped;
        }

        #endregion

        /// <summary>
        /// Routes a message with the specified delay.
        /// </summary>
        /// <param name="routingRequest">
        /// The message routingRequest to delay.
        /// </param>
        /// <param name="nextRoute">
        /// The next Route.
        /// </param>
        /// <param name="delay">
        /// The amount of time to wait before routing the message.
        /// </param>
        private static void SendWithDelay(
            MessageRoutingRequest<TMessage> routingRequest, 
            IServiceRoute<TMessage> nextRoute, 
            TimeSpan delay)
        {
            if (delay > TimeSpan.Zero)
            {
                // Wait the specified amount of time.
                Thread.Sleep(delay);
            }

            SendToRoute(routingRequest, nextRoute);
        }

        /// <summary>
        /// Sends a message request to the specified route.
        /// </summary>
        /// <param name="routingRequest">
        /// The routing request to send.
        /// </param>
        /// <param name="nextRoute">
        /// The next route.
        /// </param>
        private static void SendToRoute(MessageRoutingRequest<TMessage> routingRequest, IServiceRoute<TMessage> nextRoute)
        {
            if (routingRequest == null)
            {
                throw new ArgumentNullException("routingRequest");
            }

            if (nextRoute == null)
            {
                throw new ArgumentNullException("nextRoute");
            }

            var requestEvent = new MessageEntry<TMessage>(nextRoute, routingRequest);
            routingRequest.RoutingNotification.OnMessageRouting(requestEvent);
            nextRoute.SendMessage(routingRequest);
        }

        /// <summary>
        /// Resends the routing request to the last route it was in after a specified delay period.
        /// </summary>
        /// <param name="routingRequest">
        /// The routing request.
        /// </param>
        /// <param name="waitTime">
        /// The wait time.
        /// </param>
        /// <param name="blockQueue">
        /// A value indicating whether to block the current queue (true) or not (false).
        /// </param>
        private void ResendWithDelay(MessageRoutingRequest<TMessage> routingRequest, TimeSpan waitTime, bool blockQueue)
        {
            if (routingRequest.CurrentLocation == null)
            {
                var message = String.Format(
                    ErrorMessages.CurrentRoutingStatusRequired, 
                    ApplicationOperationContext.Current.CurrentAction);

                throw new OperationException(routingRequest, message);
            }

            // Resend the message back to the route.
            if (blockQueue)
            {
                SendWithDelay(routingRequest, routingRequest.CurrentLocation.ServiceRoute, waitTime);
            }
            else
            {
                Task.Factory.StartNew(() => SendWithDelay(routingRequest, routingRequest.CurrentLocation.ServiceRoute, waitTime))
                    .ContinueWith(
                        taskResult =>
                        {
                            if (taskResult.Exception == null)
                            {
                                return;
                            }

                            this.actionEventProxy.RecordAction(routingRequest, taskResult.Exception);
                            taskResult.Exception.Handle(exception => exception is DomainException);
                        }, 
                        TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
            }
        }

        /// <summary>
        /// Cancels any duplicates of the specified request, or the request itself if it is not the most recent request.
        /// </summary>
        /// <param name="routingRequest">
        /// The routing request to evaluate.
        /// </param>
        /// <exception cref="BusinessException">
        /// <paramref name="routingRequest"/> is not the most recent duplicate request.
        /// </exception>
        private void CancelDuplicates(MessageRoutingRequest<TMessage> routingRequest)
        {
            // First add the existing request to the open requests list.
            this.openRequests.Add(routingRequest);

            // Next create a list of duplicate requests.
            var activeDuplicates =
                this.openRequests.Where(x => this.duplicateEqualityComparer.Equals(routingRequest, x))
                    .OrderByDescending(x => x.Message, this.profileProvider.SequenceComparer)
                    .ToList();

            // If there is more than one duplicate, then supersede it.
            if (activeDuplicates.Count > 1)
            {
                foreach (var request in activeDuplicates.Skip(1))
                {
                    request.Supersede();
                }
            }

            // If the current request got canceled, throw an error.
            if (routingRequest.Canceled)
            {
                throw new BusinessException(routingRequest, routingRequest.RoutingError.Message);
            }
        }

        /// <summary>
        /// Starts a message routing request.
        /// </summary>
        /// <param name="routingRequest">
        /// The routing request to start.
        /// </param>
        private void SendRequest(MessageRoutingRequest<TMessage> routingRequest)
        {
            if (routingRequest.Canceled || routingRequest.RoutingError != null)
            {
                this.TerminateRequest(routingRequest);
            }
            else
            {
                var messageEntry = new MessageEntry<TMessage>(this.requestPool, routingRequest);
                routingRequest.StartRequest(messageEntry);
                routingRequest.RoutingNotification.OnMessageRouting(messageEntry);
                this.requestPool.SendMessage(routingRequest);
            }
        }

        /// <summary>
        /// Registers route events with the router.
        /// </summary>
        /// <param name="configuration">
        /// The profile configuration containing the routes to register.
        /// </param>
        private void RegisterRouteEvents(RoutingConfiguration<TMessage> configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            // Register service routes only one time. The key thing to understand is that service routes may raise events that this 
            // router did not initiate.
            lock (this.registeredRoutes)
            {
                // Register each route only one time.
                foreach (var serviceRoute in configuration.ServiceRoutes.Except(this.registeredRoutes))
                {
                    this.registeredRoutes.Add(serviceRoute);
                    serviceRoute.RequestReceived += this.UpdateMessageLocation;
                    serviceRoute.RequestCompleted += this.HandleMessageReturn;

                    if (ConfigurationManager.AppSettings.IsTrue(LogRoutingActions))
                    {
                        this.actionEventProxy.RecordAction(serviceRoute);
                    }
                }
            }
        }

        /// <summary>
        /// Routes a message based on the current status of the routing request.
        /// </summary>
        /// <param name="routingRequest">
        /// The message routing request to route.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="routingRequest"/> is null.
        /// </exception>
        /// <exception cref="OperationException">
        /// The routing configuration was null for the request and the message was not already failed.
        /// </exception>
        private void RouteMessage(MessageRoutingRequest<TMessage> routingRequest)
        {
            if (routingRequest == null)
            {
                throw new ArgumentNullException("routingRequest");
            }

            if (routingRequest.Canceled)
            {
                this.TerminateRequest(routingRequest);
                return;
            }

            IServiceRoute<TMessage> nextRoute;
            var currentStatus = routingRequest.CurrentLocation;
            var isNewMessage = currentStatus == null || currentStatus.ServiceRoute == null
                               || currentStatus.ServiceRoute is PendingMessageQueueRoute<TMessage>
                               || currentStatus.ServiceRoute is PendingMessageQueuePool<TMessage>;

            FailureResponse? failureResponse = null;

            if (routingRequest.RoutingError != null)
            {
                if (isNewMessage == false)
                {
                    failureResponse = this.failurePolicy.GetFailureResponse(routingRequest.Message, routingRequest.RoutingError);
                }

                nextRoute = this.profileProvider.FailureRoute;
            }
            else if (routingRequest.RoutingConfiguration == null)
            {
                throw new OperationException(routingRequest, ErrorMessages.RoutingConfigurationNotFound, routingRequest.RoutingError);
            }
            else if (isNewMessage)
            {
                nextRoute = routingRequest.RoutingConfiguration.ServiceRoutes.First;
            }
            else
            {
                nextRoute = routingRequest.RoutingConfiguration.ServiceRoutes.GetNext(currentStatus.ServiceRoute);
            }

            if (failureResponse.HasValue && failureResponse.Value.Retry)
            {
                routingRequest.ClearFailure();
                this.ResendWithDelay(routingRequest, failureResponse.Value.WaitTime, failureResponse.Value.BlockQueue);
            }
            else if (nextRoute != null)
            {
                SendToRoute(routingRequest, nextRoute);
            }
            else
            {
                throw new OperationException(routingRequest, ErrorMessages.MessageRequestAlreadyComplete, routingRequest.RoutingError);
            }
        }

        /// <summary>
        /// The handle message response.
        /// </summary>
        /// <param name="responseEvent">
        /// The response event.
        /// </param>
        private void ContinueOrFinalizeMessage(MessageExit<TMessage> responseEvent)
        {
            var routingRequest = responseEvent.RoutingRequest;

            if (routingRequest.Canceled)
            {
                this.TerminateRequest(routingRequest);
                return;
            }

            var noCurrentRoute = routingRequest.CurrentLocation == null || routingRequest.CurrentLocation.ServiceRoute == null;

            if (noCurrentRoute)
            {
                var routeError = new OperationException(
                    routingRequest, 
                    ErrorMessages.CurrentRouteNotFound, 
                    routingRequest.RoutingError);

                responseEvent = new MessageExit<TMessage>(responseEvent, routeError);
            }

            responseEvent.RoutingRequest.RoutingNotification.OnMessageReturned(responseEvent);

            var lastRoute = routingRequest.RoutingConfiguration.ServiceRoutes.Last;
            var serviceRoute = responseEvent.ServiceRoute;
            var failureRoute = this.profileProvider.FailureRoute;
            var isFailureRoute = ServiceRouteEqualityComparer<TMessage>.NameAndType.Equals(serviceRoute, failureRoute);
            var isLastRoute = ServiceRouteEqualityComparer<TMessage>.NameAndType.Equals(serviceRoute, lastRoute);

            if (responseEvent.EventError != null)
            {
                // Fail the message and return it for routing.
                routingRequest.FailRequest(responseEvent.EventError);
                this.routingProducer.ProduceItem(routingRequest);
            }
            else if (routingRequest.RequeuePending)
            {
                // Start over.
                routingRequest.ClearFailure();
                this.SendRequest(routingRequest);
            }
            else if (isFailureRoute || isLastRoute)
            {
                // Finalize the message as it has finished or failed.
                this.FinalizeMessage(routingRequest);
            }
            else
            {
                // Route to the next node.
                this.routingProducer.ProduceItem(routingRequest);
            }
        }

        /// <summary>
        /// Finds an active request in the open requests concurrent bag.
        /// </summary>
        /// <param name="message">
        /// The message to find.
        /// </param>
        /// <returns>
        /// The active <see cref="T:SAF.MessageQueuing.MessageRoutingRequest`1"/> for the message, or <c>null</c> if the message is not
        /// active.
        /// </returns>
        private MessageRoutingRequest<TMessage> FindActiveRequest(TMessage message)
        {
            return this.openRequests.Find(request => this.profileProvider.IdentityComparer.Equals(request.Message, message));
        }

        /// <summary>
        /// Finds a routing request in the routing repository.
        /// </summary>
        /// <param name="message">
        /// The message to retrieve the request for.
        /// </param>
        /// <param name="notification">
        /// The notification to apply to the routing request.
        /// </param>
        /// <param name="repositoryProvider">
        /// The repository provider for the routing repository.
        /// </param>
        /// <returns>
        /// The <see cref="T:SAF.MessageQueuing.MessageRoutingRequest`1"/> associated with the message.
        /// </returns>
        private MessageRoutingRequest<TMessage> OpenSavedRequest(
            TMessage message, 
            INotifyMessageRouted<TMessage> notification, 
            IRepositoryProvider repositoryProvider)
        {
            MessageRoutingRequest<TMessage> routingRequest;
            var routingRepository = this.repositoryFactory.Create(repositoryProvider);

            // The routing request is not active so we will treat it like a new request.
            if (routingRepository.IsDelivered(message))
            {
                // Undo the delivery.
                ////Trace.TraceInformation("Reopening {0}", message);
                var delivery = routingRepository.ReopenRequest(message);
                routingRequest = new MessageRoutingRequest<TMessage>(delivery, notification);
            }
            else
            {
                // The message is neither active or delivered so we need to reanimate it.
                ////Trace.TraceInformation("Reanimating {0}", message);
                IRoutingStatus<TMessage> routingStatus = routingRepository.GetRoutingStatus(message);
                routingRequest = new MessageRoutingRequest<TMessage>(routingStatus, notification);
            }

            return routingRequest;
        }

        /// <summary>
        /// Terminates the specified request.
        /// </summary>
        /// <param name="request">
        /// The request to terminate.
        /// </param>
        private void TerminateRequest(MessageRoutingRequest<TMessage> request)
        {
            MessageEntry<TMessage> messageEntry;

            if (request.CurrentLocation == null)
            {
                messageEntry = new MessageEntry<TMessage>(this.requestPool, request);
            }
            else if (request.CurrentLocation.ServiceRoute == null)
            {
                messageEntry = new MessageEntry<TMessage>(this.requestPool, request);
                request.CurrentLocation.UpdateLocation(messageEntry);
            }
            else
            {
                messageEntry = new MessageEntry<TMessage>(request);
            }

            // TODO: Could be a duplicate notification if the request is active.
            request.RoutingNotification.OnMessageReceived(messageEntry);

            // Close out the route
            var responseEvent = new MessageExit<TMessage>(messageEntry, request.RoutingError);
            request.RoutingNotification.OnMessageReturned(responseEvent);
            this.FinalizeMessage(request);
        }

        /// <summary>
        /// Finalizes a message for delivery.
        /// </summary>
        /// <param name="routingRequest">
        /// The response event.
        /// </param>
        private void FinalizeMessage(MessageRoutingRequest<TMessage> routingRequest)
        {
            lock (this.messageLock)
            {
                this.openRequests.Remove(routingRequest);
                this.profileProvider.FailurePolicy.FinalizeMessage(routingRequest.Message);
                this.profileProvider.FinalizeRequest(routingRequest);
                routingRequest.RoutingNotification.OnMessageDelivered(routingRequest);
            }
        }

        #region Event Handlers

        /// <summary>
        /// Updates the location of a message routing request.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="eventArgs">
        /// The event data associated with the event.
        /// </param>
        private void UpdateMessageLocation(object sender, RequestEventArgs<TMessage> eventArgs)
        {
            var requestEvent = eventArgs.RequestEvent;
            var routingRequest = requestEvent.RoutingRequest;

            MessageExit<TMessage> exitEvent;

            if (routingRequest.CurrentLocation != null && routingRequest.CurrentLocation.ServiceRoute != null)
            {
                // Create the routed event first before updating the location. This constructor uses the current status of the request.
                exitEvent = new MessageExit<TMessage>(routingRequest);
            }
            else
            {
                // Assume this was an abandoned request and use the request pool as the routing point.
                var routingEntry = new MessageEntry<TMessage>(this.requestPool, routingRequest);
                routingRequest.RoutingNotification.OnMessageRouting(routingEntry);
                exitEvent = new MessageExit<TMessage>(routingEntry);
            }

            try
            {
                routingRequest.RoutingNotification.OnMessageReceived(requestEvent);
            }
            catch (Exception ex)
            {
                exitEvent = new MessageExit<TMessage>(exitEvent, ex);
                throw;
            }
            finally
            {
                routingRequest.RoutingNotification.OnMessageRouted(exitEvent);
            }
        }

        /// <summary>
        /// Notifies subscribers that a routing profile has been selected for a message.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="eventArgs">
        /// The event data associated with the event.
        /// </param>
        private void NotifyMessageRequestReady(object sender, ResponseEventArgs<TMessage> eventArgs)
        {
            var responseEvent = eventArgs.ResponseEvent;
            var routingRequest = responseEvent.RoutingRequest;

            if (routingRequest.Canceled)
            {
                this.TerminateRequest(routingRequest);
                return;
            }

            if (routingRequest.RoutingConfiguration == null)
            {
                if (responseEvent.EventError == null)
                {
                    var routingError = new OperationException(routingRequest, ErrorMessages.RoutingConfigurationNotFound);
                    responseEvent = new MessageExit<TMessage>(responseEvent, routingError);
                }
            }
            else
            {
                this.RegisterRouteEvents(routingRequest.RoutingConfiguration);
            }

            responseEvent.RoutingRequest.RoutingNotification.OnMessageReturned(responseEvent);
            this.routingProducer.ProduceItem(routingRequest);
        }

        /// <summary>
        /// Consumes routing requests for the message routing producer queue.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="eventArgs">
        /// The event data associated with the event.
        /// </param>
        private void ConsumeRoutingRequests(object sender, ItemsProducedEventArgs eventArgs)
        {
            while (this.routingProducer.ItemQueueConsumer.ConsumeNext())
            {
                var current = this.routingProducer.ItemQueueConsumer.Current;

                try
                {
                    this.RouteMessage(current);
                }
                catch (DomainException ex)
                {
                    this.actionEventProxy.RecordAction(current, ex);
                }
                catch (Exception ex)
                {
                    this.actionEventProxy.RecordAction(current, ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Consumes message exit events from the message response producer queue.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="eventArgs">
        /// The event data associated with the event.
        /// </param>
        private void ConsumeRoutingResponses(object sender, ItemsProducedEventArgs eventArgs)
        {
            while (this.responseProducer.ItemQueueConsumer.ConsumeNext())
            {
                var current = this.responseProducer.ItemQueueConsumer.Current;

                try
                {
                    this.ContinueOrFinalizeMessage(current);
                }
                catch (DomainException ex)
                {
                    this.actionEventProxy.RecordAction(current, ex);
                }
                catch (Exception ex)
                {
                    this.actionEventProxy.RecordAction(current, ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Handles the request completion events from service routes.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="eventArgs">
        /// The event data associated with the event.
        /// </param>
        /// <exception cref="OperationException">
        /// The sender of the event is either not a <see cref="T:SAF.MessageQueuing.IServiceRoute`1"/> for 
        /// <typeparamref name="TMessage"/> items or was not provided in the original list of service routes.
        /// </exception>
        private void HandleMessageReturn(object sender, ResponseEventArgs<TMessage> eventArgs)
        {
            var responseEvent = eventArgs.ResponseEvent;
            this.responseProducer.ProduceItem(responseEvent);
        }

        /// <summary>
        /// Handles events that indicate routing has stopped.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="eventArgs">
        /// The event data associated with the event.
        /// </param>
        private void HandleRoutingStopped(object sender, ProcessStoppedEventArgs eventArgs)
        {
            if (eventArgs.EventError != null)
            {
                this.actionEventProxy.RecordAction(sender, eventArgs.EventError);
            }
        }

        /// <summary>
        /// Finalizes a delivered message.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="eventArgs">
        /// The event data associated with the event.
        /// </param>
        /// <exception cref="OperationException">
        /// The sender of the event is either not a <see cref="T:SAF.MessageQueuing.IServiceRoute`1"/> for 
        /// <typeparamref name="TMessage"/> items or was not provided in the original list of service routes.
        /// </exception>
        private void FinalizeMessage(object sender, ResponseEventArgs<TMessage> eventArgs)
        {
            var sourceRoute = sender as IServiceRoute<TMessage>;
            var routingRequest = eventArgs.ResponseEvent.RoutingRequest;

            if (sourceRoute == null)
            {
                string message = String.Format(
                    ErrorMessages.EventSourceNotOfExpectedType, 
                    eventArgs.ResponseEvent, 
                    sender.GetType(), 
                    typeof(IServiceRoute<TMessage>));

                throw new OperationException(routingRequest, message);
            }

            var responseEvent = eventArgs.ResponseEvent;
            routingRequest.RoutingNotification.OnMessageReturned(responseEvent);
            this.FinalizeMessage(routingRequest);
        }

        #endregion

        #endregion
    }
}