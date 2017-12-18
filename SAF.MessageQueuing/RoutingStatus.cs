// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoutingStatus.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains the current status of a message within the routing context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;

    using JetBrains.Annotations;

    using SAF.Core;

    /// <summary>
    /// Contains the current status of a message within the routing context.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of <see cref="IPriorityMessage"/> being routed.
    /// </typeparam>
    public class RoutingStatus<TMessage> : IRoutingStatus<TMessage>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// The service route name.
        /// </summary>
        private readonly string serviceRouteName;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutingStatus{TMessage}"/> class.
        /// </summary>
        /// <param name="eventId">
        /// The event GUID for the routing event.
        /// </param>
        /// <param name="entryTime">
        /// The time the message entered the service route.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="serviceRoute">
        /// The service route.
        /// </param>
        protected internal RoutingStatus(
            Guid eventId,
            DateTimeOffset entryTime,
            TMessage message,
            IServiceRoute<TMessage> serviceRoute)
        {
            ////this.serviceRouteName = serviceRoute.Name;
            this.Message = message;
            this.EntryTime = entryTime;
            this.EventGuid = eventId;
            this.ServiceRoute = serviceRoute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutingStatus{TMessage}"/> class.
        /// </summary>
        /// <param name="eventId">
        /// The event GUID for the routing event.
        /// </param>
        /// <param name="entryTime">
        /// The time the message entered the service route.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="serviceRouteName">
        /// The service Route name.
        /// </param>
        protected RoutingStatus(
            Guid eventId,
            DateTimeOffset entryTime,
            TMessage message,
            [NotNull] string serviceRouteName)
        {
            this.serviceRouteName = serviceRouteName;
            this.Message = message;
            this.EntryTime = entryTime;
            this.EventGuid = eventId;
        }

        /// <summary>
        /// Gets the service route that last accepted the message.
        /// </summary>
        public IServiceRoute<TMessage> ServiceRoute { get; private set; }

        /// <summary>
        /// Gets the globally unique ID for the routing event.
        /// </summary>
        public Guid EventGuid { get; private set; }

        /// <summary>
        /// Gets the time the message was accepted by the service route.
        /// </summary>
        public DateTimeOffset EntryTime { get; private set; }

        /// <summary>
        /// Gets the message being processed.
        /// </summary>
        public TMessage Message { get; private set; }

        /// <summary>
        /// Resolves the route for the current routing status.
        /// </summary>
        /// <param name="routeContainer">
        /// The route container that contains the original route.
        /// </param>
        public void ResolveRoute([NotNull] IServiceRouteContainer<TMessage> routeContainer)
        {
            if (routeContainer == null)
            {
                throw new ArgumentNullException("routeContainer");
            }

            if (this.serviceRouteName == null)
            {
                return;
            }

            try
            {
                this.ServiceRoute = routeContainer.Resolve(this.serviceRouteName);
            }
            catch (DomainException)
            {
                this.ServiceRoute = null;
            }
        }

        /// <summary>
        /// Updates the location of the message.
        /// </summary>
        /// <param name="requestEvent">
        /// The request event.
        /// </param>
        public void UpdateLocation(MessageEntry<TMessage> requestEvent)
        {
            if (requestEvent == null)
            {
                throw new ArgumentNullException("requestEvent");
            }

            this.ServiceRoute = requestEvent.ServiceRoute;
            this.EntryTime = requestEvent.InitiationTime;
            this.EventGuid = requestEvent.Identifier;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return Convert.ToString(this.ServiceRoute);
        }
    }
}
