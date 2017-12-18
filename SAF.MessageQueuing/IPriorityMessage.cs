// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPriorityMessage.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Defines the IPriorityRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;

    /// <summary>
    /// Provides an interface to items that support requests based on priority.
    /// </summary>
    public interface IPriorityMessage : IComparable
    {
        /// <summary>
        /// Gets the globally unique ID for the request.
        /// </summary>
        Guid RequestGuid { get; }

        /// <summary>
        /// Gets the request time.
        /// </summary>
        DateTimeOffset RequestTime { get; }

        /// <summary>
        /// Gets the processing deadline for this request.
        /// </summary>
        DateTimeOffset Deadline { get; }

        /// <summary>
        /// Gets the amount of time prior to the deadline when the request's priority should be escalated.
        /// </summary>
        TimeSpan EscalationThreshold { get; }
    }
}