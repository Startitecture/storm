// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PendingMessageQueueRoute.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The profile provider queue route.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;

    using SAF.ActionTracking;

    /// <summary>
    /// The pending message queue route.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of message waiting to be routed.
    /// </typeparam>
    public class PendingMessageQueueRoute<TMessage> : QueueRouteBase<TMessage>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// The profile provider.
        /// </summary>
        private readonly IRoutingProfileProvider<TMessage> profileProvider;

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="PendingMessageQueueRoute{TMessage}"/> class.
        /// </summary>
        /// <param name="actionEventProxy">
        /// The action event proxy.
        /// </param>
        /// <param name="profileProvider">
        /// The profile provider.
        /// </param>
        /// <param name="name">
        /// The name of the route.
        /// </param>
        public PendingMessageQueueRoute(IActionEventProxy actionEventProxy, IRoutingProfileProvider<TMessage> profileProvider, string name)
            : base(actionEventProxy, profileProvider, name)
        {
            if (profileProvider == null)
            {
                throw new ArgumentNullException("profileProvider");
            }

            this.profileProvider = profileProvider;
        }

        /// <summary>
        /// Processes a priority message within the queue.
        /// </summary>
        /// <param name="routingRequest">
        /// The message to process.
        /// </param>
        protected override void ProcessMessage(MessageRoutingRequest<TMessage> routingRequest)
        {
            if (routingRequest == null)
            {
                throw new ArgumentNullException("routingRequest");
            }

            if (routingRequest.Canceled)
            {
                return;
            }

            var currentLocation = routingRequest.InitialLocation == null ? null : routingRequest.InitialLocation.ServiceRoute;
            var configuration = new RoutingConfiguration<TMessage>(this.profileProvider, routingRequest.Message, currentLocation);
            routingRequest.LoadRoutingConfiguration(configuration);
        }

        #endregion
    }
}