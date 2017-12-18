// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceRequestHistory.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Maintains a history of service requests for a specific service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SAF.Core;
    using SAF.StringResources;

    /// <summary>
    /// Maintains a history of service requests for a specific service.
    /// </summary>
    /// <typeparam name="TService">
    /// The type of service to maintain request history for.
    /// </typeparam>
    public class ServiceRequestHistory<TService> : IServiceAvailabilityMetric
    {
        #region Fields

        /// <summary>
        /// The request statuses.
        /// </summary>
        private readonly List<ServiceRequestStatus> requestStatuses = new List<ServiceRequestStatus>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRequestHistory{TService}"/> class.
        /// </summary>
        /// <param name="service">
        /// The service.
        /// </param>
        public ServiceRequestHistory(TService service)
            : this(service, 1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRequestHistory{TService}"/> class.
        /// </summary>
        /// <param name="service">
        /// The service.
        /// </param>
        /// <param name="serviceWeight">
        /// The service weight.
        /// </param>
        public ServiceRequestHistory(TService service, double serviceWeight)
        {
            this.Service = service;
            this.ServiceWeight = serviceWeight;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the request state for the current service. If there have not been any requests, this is 
        /// <see cref="ServiceRequestState.Pending"/>. If there are any open requests, this is 
        /// <see cref="ServiceRequestState.InProgress"/>. If all requests have been completed, this is 
        /// <see cref="ServiceRequestState.Completed"/>.
        /// </summary>
        public ServiceRequestState RequestState { get; private set; }

        /// <summary>
        /// Gets the total number of completed requests.
        /// </summary>
        public long CompletedRequests { get; private set; }

        /// <summary>
        /// Gets the total time taken by all completed requests since the service started.
        /// </summary>
        public TimeSpan CompletedRequestTime { get; private set; }

        /// <summary>
        /// Gets the time of the last response.
        /// </summary>
        public DateTimeOffset LastResponse { get; private set; }

        /// <summary>
        /// Gets the total amount of time spent on currently open requests.
        /// </summary>
        public TimeSpan OpenRequestTime
        {
            get
            {
                var now = DateTimeOffset.Now;
                return TimeSpan.FromTicks(this.requestStatuses.Sum(status => (now - status.RequestTime).Ticks));
            }
        }

        /// <summary>
        /// Gets the total number of currently open requests.
        /// </summary>
        public int OpenRequests
        {
            get
            {
                return this.requestStatuses.Count(x => x.RequestState == ServiceRequestState.InProgress);
            }
        }

        /// <summary>
        /// Gets the time of the last request.
        /// </summary>
        public DateTimeOffset LastRequest { get; private set; }

        /// <summary>
        /// Gets the service for which requests are being recorded.
        /// </summary>
        public TService Service { get; private set; }

        /// <summary>
        /// Gets the static weight of the service, with higher weights indicating less availability.
        /// </summary>
        public double ServiceWeight { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Adds an open request to the request history.
        /// </summary>
        /// <returns>
        /// A <see cref="Guid"/> representing the ID of the new request.
        /// </returns>
        public Guid Send()
        {
            var requestStatus = new ServiceRequestStatus();

            lock (this.requestStatuses)
            {
                requestStatus.Send();
                this.requestStatuses.Add(requestStatus);
                this.RequestState = ServiceRequestState.InProgress;
                this.LastRequest = requestStatus.RequestTime;
            }

            return requestStatus.RequestId;
        }

        /// <summary>
        /// Completes the service request for the specified request ID.
        /// </summary>
        /// <param name="requestId">
        /// The request ID.
        /// </param>
        /// <exception cref="BusinessException">
        /// The request ID could not be found, or was already accepted.
        /// </exception>
        public void AcceptResponse(Guid requestId)
        {
            ServiceRequestStatus requestStatus;

            try
            {
                requestStatus = this.requestStatuses.First(x => x.RequestId == requestId);
            }
            catch (InvalidOperationException ex)
            {
                throw new BusinessException(this, String.Format(ErrorMessages.ServiceRequestStatusNotFound, requestId, this), ex);
            }

            try
            {
                lock (this.requestStatuses)
                {
                    requestStatus.AcceptResponse();

                    this.LastResponse = requestStatus.ResponseTime;

                    // Update stats.
                    try
                    {
                        this.CompletedRequests++;
                    }
                    catch (OverflowException)
                    {
                        this.CompletedRequests = Int64.MaxValue;
                    }

                    try
                    {
                        this.CompletedRequestTime =
                            this.CompletedRequestTime.Add(requestStatus.ResponseTime - requestStatus.RequestTime);
                    }
                    catch (OverflowException)
                    {
                        this.CompletedRequestTime = TimeSpan.MaxValue;
                    }

                    if (this.requestStatuses.All(x => x.RequestState != ServiceRequestState.InProgress))
                    {
                        this.RequestState = ServiceRequestState.Completed;
                    }

                    this.requestStatuses.RemoveAll(
                        x => x.RequestState == ServiceRequestState.Completed && x.RequestId != requestStatus.RequestId);
                }
            }
            catch (InvalidOperationException ex)
            {
                throw new BusinessException(requestStatus, ex.Message, ex);
            }
        }

        /// <summary>
        /// Resets completion statistics.
        /// </summary>
        public void ResetCompletionStatistics()
        {
            lock (this.requestStatuses)
            {
                this.CompletedRequestTime = TimeSpan.Zero;
                this.CompletedRequests = 0;
            }
        }

        #endregion
    }
}