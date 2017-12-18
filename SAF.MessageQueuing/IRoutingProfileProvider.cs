// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRoutingProfileProvider.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface to classes that resolve document routing profiles.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides an interface to classes that resolve document routing profiles.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of <see cref="IPriorityMessage"/> that is routed by the resolved profile.
    /// </typeparam>
    public interface IRoutingProfileProvider<TMessage>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// Gets the name to apply to the pending message queue.
        /// </summary>
        string PendingMessageQueueName { get; }

        /// <summary>
        /// Gets the equality comparer for determining message identity.
        /// </summary>
        IEqualityComparer<TMessage> IdentityComparer { get; }
        
        /// <summary>
        /// Gets the comparer for determining message priority. Messages ordered first will have the highest priority.
        /// </summary>
        IComparer<TMessage> PriorityComparer { get; }

        /// <summary>
        /// Gets the comparer for determining message sequence. Messages ordered first will be considered the oldest in the sequence.
        /// </summary>
        IComparer<TMessage> SequenceComparer { get; }
        
        /// <summary>
        /// Gets the equality comparer for superseding duplicate messages.
        /// </summary>
        IEqualityComparer<TMessage> DuplicateEqualityComparer { get; }

        /// <summary>
        /// Gets the failure route.
        /// </summary>
        IServiceRoute<TMessage> FailureRoute { get; } 
        
        /// <summary>
        /// Gets the failure policy for routing failed messages.
        /// </summary>
        IRoutingFailurePolicy<TMessage> FailurePolicy { get; } 
        
        /// <summary>
        /// Gets the continuation provider.
        /// </summary>
        IRoutingContinuationProvider<TMessage> ContinuationProvider { get; }

        /// <summary>
        /// Resolves a routing profile for the specified <typeparamref name="TMessage"/>/>.
        /// </summary>
        /// <param name="message">
        /// The message to resolve a profile for.
        /// </param>
        /// <returns>
        /// The <see cref="T:SAF.MessageQueuing.IMessageRoutingProfile`1"/> that matches the request, or <c>null</c> if no profile 
        /// could be resolved.
        /// </returns>
        IMessageRoutingProfile<TMessage> ResolveProfile(TMessage message);

        /// <summary>
        /// Finalizes a routing request.
        /// </summary>
        /// <param name="routingRequest">
        /// The delivery to finalize.
        /// </param>
        void FinalizeRequest(MessageRoutingRequest<TMessage> routingRequest);
    }
}
