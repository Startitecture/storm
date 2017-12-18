// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoutingProfileConfiguration.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   An internal class that contains the configuration of a routing profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System.Collections.Generic;

    /// <summary>
    /// An internal class that contains the configuration of a routing profile.
    /// </summary>
    /// <typeparam name="TRequest">
    /// The type of request that is routed.
    /// </typeparam>
    internal class RoutingProfileConfiguration<TRequest>
        where TRequest : IPriorityRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoutingProfileConfiguration{TRequest}"/> class.
        /// </summary>
        /// <param name="profile">
        /// The profile to create a routing profile configuration for.
        /// </param>
        public RoutingProfileConfiguration(IMessageRoutingProfile<TRequest> profile)
        {
            this.FailureRoute = profile.FailureRoute;
            this.ServiceRoutes = new LinkedList<IServiceRoute<TRequest>>(profile.ServiceRoutes);
        }

        /// <summary>
        /// Gets the failure route.
        /// </summary>
        public IServiceRoute<TRequest> FailureRoute { get; private set; }

        /// <summary>
        /// Gets the service routes.
        /// </summary>
        public LinkedList<IServiceRoute<TRequest>> ServiceRoutes { get; private set; }
    }
}