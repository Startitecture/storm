// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PendingMessageQueueRouteFactory.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Creates pending message queue routes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;

    using SAF.ActionTracking;

    /// <summary>
    /// Creates pending message queue routes.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of message being processed by the queue route.
    /// </typeparam>
    public class PendingMessageQueueRouteFactory<TMessage> : IQueueRouteFactory<PendingMessageQueueRoute<TMessage>>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// The action event proxy.
        /// </summary>
        private readonly IActionEventProxy actionEventProxy;

        /// <summary>
        /// The profile provider.
        /// </summary>
        private readonly IRoutingProfileProvider<TMessage> profileProvider;

        /// <summary>
        /// The route name.
        /// </summary>
        private readonly string routeName;

        /// <summary>
        /// Initializes a new instance of the <see cref="PendingMessageQueueRouteFactory{TMessage}"/> class.
        /// </summary>
        /// <param name="actionEventProxy">
        /// The action event proxy.
        /// </param>
        /// <param name="profileProvider">
        /// The profile provider.
        /// </param>
        /// <param name="routeName">
        /// The route name.
        /// </param>
        public PendingMessageQueueRouteFactory(
            IActionEventProxy actionEventProxy,
            IRoutingProfileProvider<TMessage> profileProvider,
            string routeName)
        {
            this.actionEventProxy = actionEventProxy;
            this.profileProvider = profileProvider;
            this.routeName = routeName;
        }

        /// <summary>
        /// Creates a new queue route.
        /// </summary>
        /// <returns>
        /// A new queue route instance.
        /// </returns>
        public PendingMessageQueueRoute<TMessage> Create()
        {
            return new PendingMessageQueueRoute<TMessage>(this.actionEventProxy, this.profileProvider, this.routeName);
        }
    }
}
