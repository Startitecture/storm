// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZeroRetryPolicy.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   A failure policy that immediately sends the request to the failure queue.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;

    /// <summary>
    /// A failure policy that immediately sends the request to the failure queue.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of <see cref="IPriorityMessage"/> that is being routed.
    /// </typeparam>
    public class ZeroRetryPolicy<TMessage> : IRoutingFailurePolicy<TMessage>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// Gets failure response for the specified request.
        /// </summary>
        /// <param name="message">
        /// The message that failed to route.
        /// </param>
        /// <param name="routingError">
        /// The error associated with the failure.
        /// </param>
        /// <returns>
        /// A <see cref="FailureResponse"/> that indicates the response for the failed routing action.
        /// </returns>
        public FailureResponse GetFailureResponse(TMessage message, Exception routingError)
        {
            return FailureResponse.SendToFailureRoute;
        }

        /// <summary>
        /// Finalizes the specified message. This implementation performs no action.
        /// </summary>
        /// <param name="message">
        /// The message to finalize.
        /// </param>
        public void FinalizeMessage(TMessage message)
        {
        }
    }
}
