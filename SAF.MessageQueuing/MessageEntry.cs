// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageEntry.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.MessageQueuing
{
    using System;

    using SAF.ActionTracking;
    using SAF.Core;
    using SAF.StringResources;

    /// <summary>
    /// Contains information about the entry of a request into a queue.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of <see cref="IPriorityMessage"/> entering the queue.
    /// </typeparam>
    public class MessageEntry<TMessage>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// The to string format.
        /// </summary>
        private const string ToStringFormat = "[{0}] {1} -> {2} at {3}";

        /// <summary>
        /// The route format.
        /// </summary>
        private const string RouteFormat = "Route {0}";

        /// <summary>
        /// The message type.
        /// </summary>
        private readonly string messageType = typeof(TMessage).ToRuntimeName();

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEntry{TMessage}"/> class. 
        /// Initializes a new instance of the <see cref="T:SAF.MessageQueuing.MessageEntry`1"/> class.
        /// </summary>
        /// <param name="serviceRoute">
        /// The service route.
        /// </param>
        /// <param name="routingRequest">
        /// The message request. 
        /// </param>
        public MessageEntry(IServiceRoute<TMessage> serviceRoute, MessageRoutingRequest<TMessage> routingRequest)
        {
            if (serviceRoute == null)
            {
                throw new ArgumentNullException("serviceRoute");
            }

            if (routingRequest == null)
            {
                throw new ArgumentNullException("routingRequest");
            }

            this.InitiationTime = DateTimeOffset.Now;
            this.Request = ActionRequest.Create(
                String.Format(RouteFormat, this.messageType), 
                serviceRoute.Name, 
                routingRequest.Message, 
                String.Format(ActionMessages.RoutingMessageToQueue, this.messageType, serviceRoute.Name), 
                routingRequest.Message.ToPropertyValueString());

            this.ServiceRoute = serviceRoute;
            this.RoutingRequest = routingRequest;
            this.RoutingRequest.UpdateLocation(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEntry{TMessage}"/> class.
        /// </summary>
        /// <param name="serviceRoute">
        /// The service route.
        /// </param>
        /// <param name="requestEvent">
        /// The request event.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="serviceRoute"/> or <paramref name="requestEvent"/> is <c>null</c>.
        /// </exception>
        internal MessageEntry(IServiceRoute<TMessage> serviceRoute, MessageEntry<TMessage> requestEvent)
        {
            if (serviceRoute == null)
            {
                throw new ArgumentNullException("serviceRoute");
            }

            if (requestEvent == null)
            {
                throw new ArgumentNullException("requestEvent");
            }

            this.InitiationTime = requestEvent.InitiationTime;
            this.Request = requestEvent.Request;
            this.ServiceRoute = serviceRoute;
            this.RoutingRequest = requestEvent.RoutingRequest;
            this.RoutingRequest.UpdateLocation(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEntry{TMessage}"/> class.
        /// </summary>
        /// <param name="serviceRoute">
        /// The service route.
        /// </param>
        /// <param name="responseEvent">
        /// The response event.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="serviceRoute"/> or <paramref name="responseEvent"/> is <c>null</c>.
        /// </exception>
        internal MessageEntry(IServiceRoute<TMessage> serviceRoute, MessageExit<TMessage> responseEvent)
        {
            if (serviceRoute == null)
            {
                throw new ArgumentNullException("serviceRoute");
            }

            if (responseEvent == null)
            {
                throw new ArgumentNullException("responseEvent");
            }

            this.InitiationTime = responseEvent.InitiationTime;
            this.Request = responseEvent.Request;
            this.ServiceRoute = serviceRoute;
            this.RoutingRequest = responseEvent.RoutingRequest;
            this.RoutingRequest.UpdateLocation(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEntry{TMessage}"/> class.
        /// </summary>
        /// <param name="routingRequest">
        /// The routing request.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="routingRequest"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The current routing status is not set, or the service route for the current routing status is not set.
        /// </exception>
        internal MessageEntry(MessageRoutingRequest<TMessage> routingRequest)
        {
            if (routingRequest == null)
            {
                throw new ArgumentNullException("routingRequest");
            }

            var currentStatus = routingRequest.CurrentLocation;

            if (currentStatus == null)
            {
                throw new ArgumentException(ValidationMessages.RoutingStatusNotSet);
            }

            if (currentStatus.ServiceRoute == null)
            {
                throw new ArgumentException(ValidationMessages.RoutingStatusRouteNotSet);
            }

            var serviceRoute = currentStatus.ServiceRoute;
            this.InitiationTime = currentStatus.EntryTime;
            this.Request = ActionRequest.Create(
                currentStatus.EventGuid, 
                String.Format(RouteFormat, this.messageType), 
                serviceRoute.Name, 
                routingRequest.Message, 
                String.Format(ActionMessages.RoutingMessageToQueue, this.messageType, serviceRoute.Name), 
                routingRequest.Message.ToPropertyValueString());

            this.ServiceRoute = serviceRoute;
            this.RoutingRequest = routingRequest;
            this.RoutingRequest.UpdateLocation(this);
        }

        /// <summary>
        /// Gets the request associated with the event.
        /// </summary>
        public ActionRequest Request { get; private set; }

        /// <summary>
        /// Gets the globally unique ID for the current event.
        /// </summary>
        public Guid Identifier
        {
            get
            {
                return this.Request.GlobalIdentifier;
            }
        }

        /// <summary>
        /// Gets the time that the request entered the queue.
        /// </summary>
        public DateTimeOffset InitiationTime { get; private set; }

        /// <summary>
        /// Gets the document processing request.
        /// </summary>
        public MessageRoutingRequest<TMessage> RoutingRequest { get; private set; }

        /// <summary>
        /// Gets the queue reporting the event.
        /// </summary>
        public IServiceRoute<TMessage> ServiceRoute { get; private set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current request entry event.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current request entry event.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return String.Format(ToStringFormat, this.Identifier, this.RoutingRequest, this.ServiceRoute, this.InitiationTime);
        }

        /////// <summary>
        /////// Returns a list of validation errors for the current entity.
        /////// </summary>
        /////// <returns>
        /////// A list of validation errors for the current entity. If the list is empty, the entity is correctly formed.
        /////// </returns>
        ////public IEnumerable<string> Validate()
        ////{
        ////    return
        ////        this.CreateValidation()
        ////            .IsNotEmpty(entry => entry.Identifier)
        ////            .IsNotEmpty(entry => entry.InitiationTime)
        ////            .IsSet(entry => entry.ServiceRoute)
        ////            .IsSet(entry => entry.RoutingRequest)
        ////            .Evaluate();
        ////}
    }
}