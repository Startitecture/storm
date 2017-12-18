// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQueueRouteFactory.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for classes that create queue routes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    /// <summary>
    /// Provides an interface for classes that create queue routes.
    /// </summary>
    /// <typeparam name="TQueue">
    /// The type of queue route to create.
    /// </typeparam>
    public interface IQueueRouteFactory<out TQueue>
    {
        #region Public Methods and Operators

        /// <summary>
        /// Creates a new queue route.
        /// </summary>
        /// <returns>
        /// A new queue route instance.
        /// </returns>
        TQueue Create();

        #endregion
    }
}