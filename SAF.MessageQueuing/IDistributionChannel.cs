// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDistributionChannel.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.MessageQueuing
{
    using System;

    using SAF.Core;
    using SAF.Observer;

    /// <summary>
    /// Provides an interface for classes that act as distribution channels for message routing.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of message routed by the channel.
    /// </typeparam>
    public interface IDistributionChannel<TMessage> : IDisposable
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// Occurs when the channel becomes available.
        /// </summary>
        event EventHandler IsAvailable;

        /// <summary>
        /// Occurs when the channel becomes unavailable.
        /// </summary>
        event EventHandler IsUnavailable;

        #region Public Properties

        /// <summary>
        /// Gets the name of the distribution channel.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the location of the distribution channel.
        /// </summary>
        string Location { get; }

        /// <summary>
        /// Gets the component status for the distribution channel.
        /// </summary>
        ComponentStatus ComponentStatus { get; }

        /// <summary>
        /// Gets the active messages in the distribution channel.
        /// </summary>
        MessageRoutingRequestCollection<TMessage> ActiveMessages { get; } 

        /// <summary>
        /// Gets the aborted messages for the distribution channel.
        /// </summary>
        MessageRoutingRequestCollection<TMessage> AbortedMessages { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Sends the message routing request to the distribution channel.
        /// </summary>
        /// <param name="routingRequest">
        /// The routing request to send.
        /// </param>
        void Send(MessageRoutingRequest<TMessage> routingRequest);

        /// <summary>
        /// Determines whether the specified message is active in the current channel.
        /// </summary>
        /// <param name="message">
        /// The message to locate.
        /// </param>
        /// <returns>
        /// <c>true</c> if the current channel is actively processing the message; otherwise, <c>false</c>.
        /// </returns>
        bool IsActive(TMessage message);

        /// <summary>
        /// Cancels the message in the routing status.
        /// </summary>
        /// <param name="messageStatus">
        /// The routing status containing the message to cancel.
        /// </param>
        void Cancel(IRoutingStatus<TMessage> messageStatus);

        /// <summary>
        /// Requeues the specified message.
        /// </summary>
        /// <param name="message">
        /// The message to requeue.
        /// </param>
        void Requeue(TMessage message);

        #endregion
    }
}