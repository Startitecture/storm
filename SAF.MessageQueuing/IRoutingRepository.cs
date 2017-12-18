// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRoutingRepository.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for saving queue-related items to a repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;

    /// <summary>
    /// Provides an interface for saving queue-related items to a repository.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of message routed in the repository.
    /// </typeparam>
    public interface IRoutingRepository<TMessage>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// Saves a processing request to the repository.
        /// </summary>
        /// <param name="message">
        /// The message to save.
        /// </param>
        /// <returns>
        /// The saved message.
        /// </returns>
        TMessage SaveMessage(TMessage message);

        /// <summary>
        /// Saves an entry event.
        /// </summary>
        /// <param name="requestEvent">
        /// The request event to save.
        /// </param>
        void SaveEntryEvent(MessageEntry<TMessage> requestEvent);

        /// <summary>
        /// Saves an exit event.
        /// </summary>
        /// <param name="responseEvent">
        /// The response event to save.
        /// </param>
        void SaveExitEvent(MessageExit<TMessage> responseEvent);

        /// <summary>
        /// Saves a delivery event.
        /// </summary>
        /// <param name="deliveryEvent">
        /// The delivery event for the message delivery.
        /// </param>
        void SaveDeliveryEvent(MessageDelivery<TMessage> deliveryEvent);

        /// <summary>
        /// Determines if the specified message has been saved.
        /// </summary>
        /// <param name="message">
        /// The message to query for.
        /// </param>
        /// <returns>
        /// <c>true</c> if the message is found in the repository; otherwise, <c>false</c>.
        /// </returns>
        bool IsSaved(TMessage message);

        /// <summary>
        /// Determines whether a request has been delivered.
        /// </summary>
        /// <param name="message">
        /// The message to evaluate.
        /// </param>
        /// <returns>
        /// <c>true</c> if the request has been delivered; otherwise, <c>false</c>.
        /// </returns>
        bool IsDelivered(TMessage message);

        /// <summary>
        /// Selects pending requests from the routing repository.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="T:SAF.MessageQueuing.IRoutingStatus`1"/> elements that have not yet been delivered to the final 
        /// queue.
        /// </returns>
        // ReSharper disable once ReturnTypeCanBeEnumerable.Global
        RoutingStatusCollection<TMessage> SelectPendingRequests();

        /// <summary>
        /// Gets the pending request status for the specified message.
        /// </summary>
        /// <param name="message">
        /// The message to evaluate. 
        /// </param>
        /// <returns>
        /// The <see cref="T:SAF.MessageQueuing.IRoutingStatus`1"/> for the specified message.
        /// </returns>
        IRoutingStatus<TMessage> GetRoutingStatus(TMessage message);

        /// <summary>
        /// Reopens a routing request for the specified message.
        /// </summary>
        /// <param name="message">
        /// The message to reopen.
        /// </param>
        /// <returns>
        /// The reopened message.
        /// </returns>
        TMessage ReopenRequest(TMessage message);
    }
}