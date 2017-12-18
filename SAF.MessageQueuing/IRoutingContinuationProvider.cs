// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRoutingContinuationProvider.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for message continuation policies.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;

    /// <summary>
    /// Provides an interface for message continuation policies.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of <see cref="IPriorityMessage"/> to provide continuation for.
    /// </typeparam>
    public interface IRoutingContinuationProvider<TMessage>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// Gets the reentry route for the specified message request.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="serviceRoute">
        /// The current service route.
        /// </param>
        /// <param name="profile">
        /// The message routing profile.
        /// </param>
        /// <returns>
        /// A <see cref="T:SAF.MessageQueuing.MessageContinuation`1"/> for the specified routing request, or <c>null</c> if a re-entry
        /// route cannot be determined.
        /// </returns>
        MessageContinuation<TMessage> GetReentryRoute(
            TMessage message,
            IServiceRoute<TMessage> serviceRoute,
            IMessageRoutingProfile<TMessage> profile);
    }
}
