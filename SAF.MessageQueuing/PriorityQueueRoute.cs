// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PriorityQueueRoute.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Processes requests based on their priority.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;
    using System.Collections.Generic;

    using SAF.ActionTracking;

    /// <summary>
    /// Processes requests based on their priority.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of <see cref="IPriorityMessage"/> that the queue handles.
    /// </typeparam>
    public abstract class PriorityQueueRoute<TMessage> : QueueRouteBase<TMessage>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        #region Fields

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SAF.MessageQueuing.PriorityQueueRoute`1"/> class.
        /// </summary>
        /// <param name="actionEventProxy">
        /// The action event proxy.
        /// </param>
        /// <param name="comparer">
        /// The comparer for the message priority.
        /// </param>
        protected PriorityQueueRoute(IActionEventProxy actionEventProxy, IComparer<TMessage> comparer)
            : this(actionEventProxy, comparer, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SAF.MessageQueuing.PriorityQueueRoute`1"/> class.
        /// </summary>
        /// <param name="actionEventProxy">
        /// The action event proxy.
        /// </param>
        /// <param name="comparer">
        /// The comparer for the message priority.
        /// </param>
        /// <param name="name">
        /// The name of the route.
        /// </param>
        protected PriorityQueueRoute(IActionEventProxy actionEventProxy, IComparer<TMessage> comparer, string name)
            : base(actionEventProxy, comparer, name)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Processes a message routing request.
        /// </summary>
        /// <param name="routingRequest">
        /// The routing request.
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

            this.ProcessMessage(routingRequest.Message);
        }

        /// <summary>
        /// Processes a priority message within the queue.
        /// </summary>
        /// <param name="message">
        /// The message to process.
        /// </param>
        protected abstract void ProcessMessage(TMessage message);

        #region Event Handlers

        #endregion

        #region Event Methods

        #endregion

        #endregion
    }
}