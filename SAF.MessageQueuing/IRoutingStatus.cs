// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRoutingStatus.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface to message routing status items.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;

    /// <summary>
    /// Provides an interface to message routing status items.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of <see cref="IPriorityMessage"/> being routed.
    /// </typeparam>
    public interface IRoutingStatus<TMessage>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// Gets the service route that last accepted the message.
        /// </summary>
        IServiceRoute<TMessage> ServiceRoute { get; }

        /// <summary>
        /// Gets the globally unique ID for the routing event.
        /// </summary>
        Guid EventGuid { get; }

        /// <summary>
        /// Gets the time the message was accepted by the service route.
        /// </summary>
        DateTimeOffset EntryTime { get; }

        /// <summary>
        /// Gets the message being processed.
        /// </summary>
        TMessage Message { get; }

        /// <summary>
        /// Updates the location of the message.
        /// </summary>
        /// <param name="requestEvent">
        /// The request event.
        /// </param>
        void UpdateLocation(MessageEntry<TMessage> requestEvent);

        /// <summary>
        /// Resolves the route for the current routing status.
        /// </summary>
        /// <param name="routeContainer">
        /// The route container that contains the original route.
        /// </param>
        void ResolveRoute(IServiceRouteContainer<TMessage> routeContainer);
    }
}