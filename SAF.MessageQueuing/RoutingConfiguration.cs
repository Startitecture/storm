// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoutingConfiguration.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   An internal class that contains the configuration of a routing profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;
    using System.Linq;

    /// <summary>
    /// An internal class that contains the configuration of a routing profile.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of message that is routed.
    /// </typeparam>
    public class RoutingConfiguration<TMessage>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SAF.MessageQueuing.RoutingConfiguration`1"/> class.
        /// </summary>
        /// <param name="profileProvider">
        /// The profile to create a routing configuration for.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="currentLocation">
        /// The service route, if any, where the message is currently located.
        /// </param>
        public RoutingConfiguration(IRoutingProfileProvider<TMessage> profileProvider, TMessage message, IServiceRoute<TMessage> currentLocation)
        {
            if (profileProvider == null)
            {
                throw new ArgumentNullException("profileProvider");
            }

            var profile = profileProvider.ResolveProfile(message);
            this.ServiceRoutes = profile.RoutingPath;

            // If the message is new then create a configuration based only on the message. Otherwise, create one based on the last 
            // status of the message.
            var isPending = currentLocation == null
                            || currentLocation is PendingMessageQueuePool<TMessage>
                            || currentLocation is PendingMessageQueueRoute<TMessage>;

            if (isPending)
            {
                // Don't bother getting the continuation if the message is still pending.
                return;
            }

            var messageContinuation = profileProvider.ContinuationProvider.GetReentryRoute(message, currentLocation, profile);

            if (messageContinuation == null || !profile.RoutingPath.Contains(messageContinuation.ServiceRoute))
            {
                // Don't attempt to set up a route with no continuation path.
                return;
            }

            Func<IServiceRoute<TMessage>, bool> doesNotMatchRoute =
                route => !ServiceRouteEqualityComparer<TMessage>.NameAndType.Equals(route, messageContinuation.ServiceRoute);

            this.ServiceRoutes = new ServiceRoutingPath<TMessage>(profile.RoutingPath.SkipWhile(doesNotMatchRoute).ToArray());
        }

        /// <summary>
        /// Gets the service routes.
        /// </summary>
        public ServiceRoutingPath<TMessage> ServiceRoutes { get; private set; }
    }
}