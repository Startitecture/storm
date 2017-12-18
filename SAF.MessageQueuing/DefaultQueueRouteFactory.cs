// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultQueueRouteFactory.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;

    /// <summary>
    /// Creates queue routes by invoking their default constructor.
    /// </summary>
    /// <typeparam name="TQueue">
    /// The type of queue route to create.
    /// </typeparam>
    public class DefaultQueueRouteFactory<TQueue> : IQueueRouteFactory<TQueue>
    {
        /// <summary>
        /// The constructor arguments.
        /// </summary>
        private readonly object[] constructorArguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultQueueRouteFactory{TQueue}"/> class.
        /// </summary>
        /// <param name="constructorArguments">
        /// The arguments to pass to the constructor.
        /// </param>
        public DefaultQueueRouteFactory(params object[] constructorArguments)
        {
            this.constructorArguments = constructorArguments;
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
            return (TQueue)Activator.CreateInstance(typeof(TQueue), this.constructorArguments);
        }

        #endregion
    }
}