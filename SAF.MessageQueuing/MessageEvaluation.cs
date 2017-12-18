// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageEvaluation.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains the result of a message evaluation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;

    /// <summary>
    /// Contains the result of a message evaluation.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of <see cref="IPriorityMessage"/> that has been evaluated.
    /// </typeparam>
    public class MessageEvaluation<TMessage>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// The to string format.
        /// </summary>
        private const string ToStringFormat = "Evaluated '{0}' at {1}";

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEvaluation{TMessage}"/> class.
        /// </summary>
        /// <param name="routingRequest">
        /// The message.
        /// </param>
        public MessageEvaluation(MessageRoutingRequest<TMessage> routingRequest)
            ////: this(routingRequest, null)
        {
            this.RoutingRequest = routingRequest;
            this.ResponseTime = DateTimeOffset.Now;
        }

        /////// <summary>
        /////// Initializes a new instance of the <see cref="MessageEvaluation{TMessage}"/> class.
        /////// </summary>
        /////// <param name="routingRequest">
        /////// The message.
        /////// </param>
        /////// <param name="evaluationError">
        /////// The error associated with the evaluation.
        /////// </param>
        ////public MessageEvaluation(MessageRoutingRequest<TMessage> routingRequest, Exception evaluationError)
        ////{
        ////    if (routingRequest == null)
        ////    {
        ////        throw new ArgumentNullException("routingRequest");
        ////    }

        ////    this.RoutingRequest = routingRequest;
        ////    this.ResponseTime = DateTimeOffset.Now;
        ////    this.EvaluationError = evaluationError;
        ////}

        /// <summary>
        /// Gets the message.
        /// </summary>
        public MessageRoutingRequest<TMessage> RoutingRequest { get; private set; }

        /// <summary>
        /// Gets the response time.
        /// </summary>
        public DateTimeOffset ResponseTime { get; private set; }

        /////// <summary>
        /////// Gets the error, if any, associated with the evaluation.
        /////// </summary>
        ////public Exception EvaluationError { get; private set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="MessageEvaluation{TMessage}"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="MessageEvaluation{TMessage}"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return String.Format(ToStringFormat, this.RoutingRequest, this.ResponseTime);
            ////,
            ////    this.EvaluationError == null ? "Accepted" : this.EvaluationError.Message);
        }
    }
}
