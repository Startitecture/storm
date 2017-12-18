// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueueRouteFactory.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    /// <summary>
    /// Creates queue routes by invoking their default constructor.
    /// </summary>
    /// <typeparam name="TQueue">
    /// The type of queue route to create.
    /// </typeparam>
    public class QueueRouteFactory<TQueue> : IQueueRouteFactory<TQueue>
        where TQueue : new()
    {
        /// <summary>
        /// The route factory.
        /// </summary>
        private static readonly QueueRouteFactory<TQueue> RouteFactory = new QueueRouteFactory<TQueue>();

        /// <summary>
        /// Prevents a default instance of the <see cref="T:SAF.MessageQueuing.QueueRouteFactory`1"/> class from being created.
        /// </summary>
        private QueueRouteFactory()
        {
        }

        /// <summary>
        /// Gets the default queue route factory.
        /// </summary>
        public static QueueRouteFactory<TQueue> Default
        {
            get
            {
                return RouteFactory;
            }
        }

        #region Public Methods and Operators

        /// <summary>
        /// Creates a new queue route.
        /// </summary>
        /// <returns>
        /// A new queue route instance.
        /// </returns>
        public TQueue Create()
        {
            return new TQueue();
        }

        #endregion
    }
}