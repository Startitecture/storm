// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageRoutingProfile.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface to a message routing profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;

    /// <summary>
    /// Provides an interface to a message routing profile.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of <see cref="IPriorityMessage"/> that is routed.
    /// </typeparam>
    public interface IMessageRoutingProfile<TMessage>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// Gets the service routing path for the current profile.
        /// </summary>
        ServiceRoutingPath<TMessage> RoutingPath { get; }

        /// <summary>
        /// Determines whether the specified request matches the current profile.
        /// </summary>
        /// <param name="message">
        /// The request to examine.
        /// </param>
        /// <returns>
        /// <c>true</c> if the <paramref name="message"/> matches the current profile; otherwise, <c>false</c>.
        /// </returns>
        bool MatchesProfile(TMessage message);
    }
}