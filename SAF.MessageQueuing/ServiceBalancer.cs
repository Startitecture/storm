// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceBalancer.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Determines service availability based on the least recently used service.
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
    /// Determines service availability based service request history.
    /// </summary>
    /// <typeparam name="TService">
    /// The type of service that is balanced.
    /// </typeparam>
    /// <remarks>
    /// Instances of this class compare services based on request history. All previously completed requests are removed with each 
    /// subsequent service response.
    /// </remarks>
    public class ServiceBalancer<TService> : IServiceBalancer<TService>
    {
        /// <summary>
        /// The service request histories.
        /// </summary>
        private readonly List<ServiceRequestHistory<TService>> serviceRequestHistories;

        /// <summary>
        /// The service availability comparer.
        /// </summary>
        private readonly IComparer<IServiceAvailabilityMetric> availabilityComparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SAF.MessageQueuing.ServiceBalancer`1"/> class.
        /// </summary>
        /// <param name="services">
        /// The services available to the balancer.
        /// </param>
        /// <param name="availabilityComparer">
        /// The comparer to use to compare service usage. The comparer should order <see cref="ServiceRequestStatus"/> items so
        /// that the most preferable services precede the least preferable services.
        /// </param>
        public ServiceBalancer(IEnumerable<TService> services, IComparer<IServiceAvailabilityMetric> availabilityComparer)
        {
            if (services == null)
            {
                throw new ArgumentNullException("services");
            }

            if (availabilityComparer == null)
            {
                throw new ArgumentNullException("availabilityComparer");
            }

            this.serviceRequestHistories =
                new List<ServiceRequestHistory<TService>>(services.Select(service => new ServiceRequestHistory<TService>(service)));

            this.availabilityComparer = availabilityComparer;
        }

        /// <summary>
        /// Gets the next service configuration based on the balancing algorithm.
        /// </summary>
        public TService NextService
        {
            get
            {
                return this.GetNextServiceEndpoint();
            }
        }

        /// <summary>
        /// Notifies the balancer that a request was sent.
        /// </summary>
        /// <param name="service">
        /// The service configuration that is the target of the request.
        /// </param>
        /// <returns>
        /// A <see cref="Guid"/> representing the unique identifier of the request.
        /// </returns>
        public Guid NotifyRequestSent(TService service)
        {
            ServiceRequestHistory<TService> serviceHistory =
                this.serviceRequestHistories.FirstOrDefault(requestHistory => Evaluate.Equals(requestHistory.Service, service));

            if (serviceHistory == null)
            {
                serviceHistory = new ServiceRequestHistory<TService>(service);
                this.serviceRequestHistories.Add(serviceHistory);
            }

            return serviceHistory.Send();
        }

        /// <summary>
        /// Notifies the balancer that a response was received.
        /// </summary>
        /// <param name="service">
        /// The service configuration that is the source of the response.
        /// </param>
        /// <param name="requestId">
        /// The request instance identifier.
        /// </param>
        public void NotifyResponseReceived(TService service, Guid requestId)
        {
            ServiceRequestHistory<TService> serviceHistory;

            try
            {
                serviceHistory = this.serviceRequestHistories.First(requestHistory => Evaluate.Equals(requestHistory.Service, service));
            }
            catch (InvalidOperationException ex)
            {
                throw new BusinessException(
                    service, String.Format(ErrorMessages.ServiceConfigurationNotFound, service), ex);
            }

            serviceHistory.AcceptResponse(requestId);
        }

        /// <summary>
        /// Gets the next AABBY service endpoint to use for queuing OCR requests.
        /// </summary>
        /// <returns>
        /// The <typeparamref name="TService"/> for the chosen endpoint.
        /// </returns>
        private TService GetNextServiceEndpoint()
        {
            try
            {
                return this.serviceRequestHistories.OrderBy(x => x, this.availabilityComparer).First().Service;
            }
            catch (InvalidOperationException ex)
            {
                throw new BusinessException(this, ErrorMessages.ServiceElementsNotLoaded, ex);
            }
        }
    }
}
