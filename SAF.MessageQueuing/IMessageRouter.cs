// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageRouter.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for classes that route messages.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;

    using SAF.Core;
    using SAF.ProcessEngine;

    /// <summary>
    /// Provides an interface for classes that route messages.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of message to route.
    /// </typeparam>
    public interface IMessageRouter<TMessage> : IDisposable
        where TMessage : IEquatable<TMessage>, IComparable, IComparable<TMessage>
    {
        /// <summary>
        /// Gets the active messages for the current router.
        /// </summary>
        MessageRoutingRequestCollection<TMessage> ActiveMessages { get; }

        /// <summary>
        /// Gets the aborted messages for all active service routes of the <see cref="T:SAF.MessageQueuing.IQueuePool`1"/> type.
        /// </summary>
        MessageRoutingRequestCollection<TMessage> AbortedMessages { get; }

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
        void Send(MessageRoutingRequest<TMessage> routingRequest);

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
        void Requeue(TMessage message, INotifyMessageRouted<TMessage> notification);

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
        void CancelRequest(IRoutingStatus<TMessage> routingStatus, INotifyMessageRouted<TMessage> notification);

        /// <summary>
        /// Determines if the specified message is actively routing.
        /// </summary>
        /// <param name="message">
        /// The message to locate.
        /// </param>
        /// <returns>
        /// <c>true</c> if the message is actively routing; otherwise, <c>false</c>.
        /// </returns>
        bool IsActive(TMessage message);
    }
}