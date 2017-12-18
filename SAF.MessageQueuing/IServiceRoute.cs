// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceRoute.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for routing requests to services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;

    using SAF.Core;

    /// <summary>
    /// Provides an interface for routing requests to services.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of request that the service handles.
    /// </typeparam>
    public interface IServiceRoute<TMessage> : INamedComponent
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// Occurs when a request is received.
        /// </summary>
        event EventHandler<RequestEventArgs<TMessage>> RequestReceived;

        /// <summary>
        /// Occurs when a request is completed.
        /// </summary>
        event EventHandler<ResponseEventArgs<TMessage>> RequestCompleted;

        /// <summary>
        /// Gets the instance identifier for the current service route.
        /// </summary>
        Guid InstanceIdentifier { get; }

        /// <summary>
        /// Sends a message to the service route.
        /// </summary>
        /// <param name="message">
        /// The message to send to the service route.
        /// </param>
        void SendMessage(MessageRoutingRequest<TMessage> message);
    }
}