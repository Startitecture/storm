// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INotifyMessageRouted.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for notifying subscribers of message routing events.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;

    /// <summary>
    /// Provides an interface for notifying subscribers of message routing events.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of <see cref="IPriorityMessage"/> that is being routed.
    /// </typeparam>
    public interface INotifyMessageRouted<TMessage>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// Notifies the caller that a message has been evaluated for routing.
        /// </summary>
        /// <param name="evaluationEvent">
        /// The evaluation event data.
        /// </param>
        void OnMessageEvaluated(MessageEvaluation<TMessage> evaluationEvent);

        /// <summary>
        /// Notifies the caller that a message is being routed to a queue.
        /// </summary>
        /// <param name="requestEvent">
        /// The request event data.
        /// </param>
        void OnMessageRouting(MessageEntry<TMessage> requestEvent);

        /// <summary>
        /// Notifies the caller that a message was routed to a queue.
        /// </summary>
        /// <param name="responseEvent">
        /// The request event data.
        /// </param>
        void OnMessageRouted(MessageExit<TMessage> responseEvent);

        /// <summary>
        /// Notifies the caller that a message was received by a queue.
        /// </summary>
        /// <param name="requestEvent">
        /// The request event data.
        /// </param>
        void OnMessageReceived(MessageEntry<TMessage> requestEvent);

        /// <summary>
        /// Notifies the caller that a message has returned from a queue.
        /// </summary>
        /// <param name="responseEvent">
        /// The response event data.
        /// </param>
        void OnMessageReturned(MessageExit<TMessage> responseEvent);

        /// <summary>
        /// Notifies the caller that a message was completed.
        /// </summary>
        /// <param name="routingRequest">
        /// The document delivery message.
        /// </param>
        void OnMessageDelivered(MessageRoutingRequest<TMessage> routingRequest);
    }
}
