// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceAvailabilityMetric.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for service availability metrics.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;

    /// <summary>
    /// Provides an interface for service availability metrics.
    /// </summary>
    public interface IServiceAvailabilityMetric
    {
        #region Public Properties

        /// <summary>
        /// Gets the request state for the current service. If there have not been any requests, this is 
        /// <see cref="ServiceRequestState.Pending"/>. If there are any open requests, this is 
        /// <see cref="ServiceRequestState.InProgress"/>. If all requests have been completed, this is 
        /// <see cref="ServiceRequestState.Completed"/>.
        /// </summary>
        ServiceRequestState RequestState { get; }

        /// <summary>
        /// Gets the total time taken by all completed requests since the service started.
        /// </summary>
        TimeSpan CompletedRequestTime { get; }

        /// <summary>
        /// Gets the time of the last response.
        /// </summary>
        DateTimeOffset LastResponse { get; }

        /// <summary>
        /// Gets the total number of completed requests.
        /// </summary>
        long CompletedRequests { get; }

        /// <summary>
        /// Gets the total amount of time spent on currently open requests.
        /// </summary>
        TimeSpan OpenRequestTime { get; }

        /// <summary>
        /// Gets the total number of currently open requests.
        /// </summary>
        int OpenRequests { get; }

        /// <summary>
        /// Gets the time of the last request.
        /// </summary>
        DateTimeOffset LastRequest { get; }

        /// <summary>
        /// Gets the static weight of the service, with higher weights indicating less availability.
        /// </summary>
        double ServiceWeight { get; }

        #endregion
    }
}