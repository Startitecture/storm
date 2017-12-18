// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PendingMessageQueuePool.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Manages pending message queue routes for the specified message type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;

    using SAF.ActionTracking;

    /// <summary>
    /// Manages pending message queue routes for the specified message type.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of message that will be processed by the queue pool.
    /// </typeparam>
    public class PendingMessageQueuePool<TMessage> :
        PriorityQueuePool<TMessage, PendingMessageQueueRoute<TMessage>>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PendingMessageQueuePool{TMessage}"/> class.
        /// </summary>
        /// <param name="profileProvider">
        /// The profile provider.
        /// </param>
        /// <param name="queuingPolicy">
        /// The queuing policy.
        /// </param>
        /// <param name="actionEventProxy">
        /// The action event proxy.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        public PendingMessageQueuePool(
            IRoutingProfileProvider<TMessage> profileProvider,
            IQueuingPolicy queuingPolicy,
            IActionEventProxy actionEventProxy,
            string name)
            : base(
                profileProvider,
                new PendingMessageQueueRouteFactory<TMessage>(actionEventProxy, profileProvider, name),
                queuingPolicy,
                actionEventProxy,
                name)
        {
        }

        #endregion
    }
}