// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRoutingFailurePolicy.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for routing failure policies.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;

    /// <summary>
    /// Provides an interface for route failure policies.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of message that is being routed.
    /// </typeparam>
    public interface IRoutingFailurePolicy<in TMessage>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        #region Public Methods and Operators
        
        /// <summary>
        /// Gets a failure response for the specified request.
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
        FailureResponse GetFailureResponse(TMessage message, Exception routingError);

        /// <summary>
        /// Finalizes the specified message.
        /// </summary>
        /// <param name="message">
        /// The message to finalize.
        /// </param>
        void FinalizeMessage(TMessage message);

        #endregion
    }
}