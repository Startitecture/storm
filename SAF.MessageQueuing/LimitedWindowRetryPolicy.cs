// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LimitedWindowRetryPolicy.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The limited window retry policy.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;
    using System.Collections.Concurrent;

    /// <summary>
    /// The limited window retry policy.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of message evaluated by the policy.
    /// </typeparam>
    public class LimitedWindowRetryPolicy<TMessage> : IRoutingFailurePolicy<TMessage>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        #region Fields

        /// <summary>
        /// The max attempts.
        /// </summary>
        private readonly int maxAttempts;

        /// <summary>
        /// The retry dictionary.
        /// </summary>
        private readonly ConcurrentDictionary<TMessage, int> retryDictionary = new ConcurrentDictionary<TMessage, int>();

        /// <summary>
        /// The wait time.
        /// </summary>
        private readonly TimeSpan waitTime;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LimitedWindowRetryPolicy{TMessage}"/> class.
        /// </summary>
        /// <param name="maxAttempts">
        /// The max attempts.
        /// </param>
        public LimitedWindowRetryPolicy(int maxAttempts)
            : this(maxAttempts, TimeSpan.FromMilliseconds(10))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LimitedWindowRetryPolicy{TMessage}"/> class.
        /// </summary>
        /// <param name="maxAttempts">
        /// The max attempts.
        /// </param>
        /// <param name="waitTime">
        /// The wait time.
        /// </param>
        public LimitedWindowRetryPolicy(int maxAttempts, TimeSpan waitTime)
        {
            this.maxAttempts = maxAttempts;
            this.waitTime = waitTime;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Finalizes the specified message.
        /// </summary>
        /// <param name="message">
        /// The message to finalize.
        /// </param>
        public void FinalizeMessage(TMessage message)
        {
            int retries;
            this.retryDictionary.TryRemove(message, out retries);
        }

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
        public FailureResponse GetFailureResponse(TMessage message, Exception routingError)
        {
            int attempts = this.retryDictionary.AddOrUpdate(message, 1, (guid, i) => i + 1);

            if (attempts < this.maxAttempts)
            {
                return new FailureResponse(this.waitTime);
            }

            int finalAttempts;
            this.retryDictionary.TryRemove(message, out finalAttempts);
            return FailureResponse.SendToFailureRoute;
        }

        #endregion
    }
}