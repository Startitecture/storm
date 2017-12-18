// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDistributionChannelGroup.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.MessageQueuing
{
    using System;

    using SAF.Core;
    using SAF.Observer;

    /// <summary>
    /// Provides an interface for classes that manage one or more message distribution channels.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of message that is distributed.
    /// </typeparam>
    public interface IDistributionChannelGroup<TMessage> : IDisposable
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// Gets the component status for the local distribution channel.
        /// </summary>
        ComponentStatus LocalStatus { get; }

        /// <summary>
        /// Gets the best channel based on the channel status comparer in the current instance.
        /// </summary>
        IDistributionChannel<TMessage> BestChannel { get; }

        /// <summary>
        /// Gets all aborted requests across the current group.
        /// </summary>
        MessageRoutingRequestCollection<TMessage> AbortedRequests { get; }

        /// <summary>
        /// Gets all active requests across the current group.
        /// </summary>
        MessageRoutingRequestCollection<TMessage> ActiveRequests { get; }

        /// <summary>
        /// Determines whether the message is active in the local context of the distribution group.
        /// </summary>
        /// <param name="message">
        /// The message to evaluate.
        /// </param>
        /// <returns>
        /// <c>true</c> if the message is active in local context of the distribution group; otherwise, <c>false</c>.
        /// </returns>
        bool IsLocallyActive(TMessage message);

        /// <summary>
        /// Gets the channel that is actively routing the specified message.
        /// </summary>
        /// <param name="message">
        /// The message to locate.
        /// </param>
        /// <returns>
        /// The <see cref="T:SAF.MessageQueuing.IDistributionChannel`1"/> that is actively routing the specified message, or null if 
        /// the message is not found in any of the distribution channels.
        /// </returns>
        IDistributionChannel<TMessage> GetCurrentChannel(TMessage message);

        /// <summary>
        /// Registers a distribution channel with the current group.
        /// </summary>
        /// <param name="channel">
        /// The channel to register.
        /// </param>
        void RegisterChannel(IDistributionChannel<TMessage> channel);

        /// <summary>
        /// Determines if the current group contains the specified channel.
        /// </summary>
        /// <param name="channel">
        /// The channel to query for.
        /// </param>
        /// <returns>
        /// <c>true</c> if the channel is contained within the group; otherwise, <c>false</c>.
        /// </returns>
        bool ContainsChannel(IDistributionChannel<TMessage> channel);
    }
}