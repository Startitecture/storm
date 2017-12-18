// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimedPriorityMessage.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The base class for priority requests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;

    /// <summary>
    /// The base class for timed priority requests.
    /// </summary>
    public abstract class TimedPriorityMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimedPriorityMessage"/> class.
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
        protected TimedPriorityMessage(
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
    }
}