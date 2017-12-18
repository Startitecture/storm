// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PriorityRequestBase.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The base class for priority requests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;

    using SAF.Core;

    /// <summary>
    /// The base class for priority requests.
    /// </summary>
    public abstract class PriorityRequestBase : IPriorityRequest, IEquatable<IPriorityRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityRequestBase"/> class.
        /// </summary>
        /// <param name="deadline">
        /// The deadline for delivery of the request.
        /// </param>
        /// <param name="escalationThreshold">
        /// The amount of time prior to the deadline when the request should be escalated relative to other requests.
        /// </param>
        protected PriorityRequestBase(DateTimeOffset deadline, TimeSpan escalationThreshold)
            : this(Guid.NewGuid(), DateTimeOffset.Now, deadline, escalationThreshold)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityRequestBase"/> class.
        /// </summary>
        /// <param name="requestIdentifier">
        /// The globally unique request identifier.
        /// </param>
        /// <param name="requestTime">
        /// The request time.
        /// </param>
        /// <param name="deadline">
        /// The deadline for delivery of the request.
        /// </param>
        /// <param name="escalationThreshold">
        /// The amount of time prior to the deadline when the request should be escalated relative to other requests.
        /// </param>
        protected PriorityRequestBase(
            Guid requestIdentifier, DateTimeOffset requestTime, DateTimeOffset deadline, TimeSpan escalationThreshold)
        {
            this.EscalationThreshold = escalationThreshold;
            this.Deadline = deadline;
            this.RequestTime = requestTime;
            this.RequestGuid = requestIdentifier;
        }

        /// <summary>
        /// Gets the globally unique ID for the request.
        /// </summary>
        public Guid RequestGuid { get; private set; }

        /// <summary>
        /// Gets the request time.
        /// </summary>
        public DateTimeOffset RequestTime { get; private set; }

        /// <summary>
        /// Gets the processing deadline for this request.
        /// </summary>
        public DateTimeOffset Deadline { get; private set; }

        /// <summary>
        /// Gets the amount of time relative to the deadline when the request's priority should be escalated.
        /// </summary>
        public TimeSpan EscalationThreshold { get; private set; }

        /// <summary>
        /// Gets the error, if any, that caused the request to fail.
        /// </summary>
        public DomainException RequestError { get; private set; }

        /// <summary>
        /// Fails the request with the error that prevented the request from being completed.
        /// </summary>
        /// <param name="requestError">
        /// The exception that prevented the request from being completed.
        /// </param>
        public void FailRequest(DomainException requestError)
        {
            this.RequestError = requestError;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public abstract bool Equals(IPriorityRequest other);
    }
}