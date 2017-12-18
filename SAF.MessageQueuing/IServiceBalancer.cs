// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceBalancer.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for balancing service usage for multiple service endpoints.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;

    /// <summary>
    /// Provides an interface for balancing service usage for multiple service endpoints.
    /// </summary>
    /// <typeparam name="TServiceConfiguration">
    /// The type of service configuration used to configure service communication.
    /// </typeparam>
    public interface IServiceBalancer<TServiceConfiguration>
    {
        #region Public Properties

        /// <summary>
        /// Gets the next service configuration based on the balancing algorithm.
        /// </summary>
        TServiceConfiguration NextService { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Notifies the balancer that a request was sent.
        /// </summary>
        /// <param name="service">
        /// The service configuration that is the target of the request.
        /// </param>
        /// <returns>
        /// A <see cref="Guid"/> representing the unique identifier of the request.
        /// </returns>
        Guid NotifyRequestSent(TServiceConfiguration service);

        /// <summary>
        /// Notifies the balancer that a response was received.
        /// </summary>
        /// <param name="service">
        /// The service configuration that is the source of the response.
        /// </param>
        /// <param name="requestId">
        /// The request instance identifier.
        /// </param>
        void NotifyResponseReceived(TServiceConfiguration service, Guid requestId);

        #endregion
    }
}