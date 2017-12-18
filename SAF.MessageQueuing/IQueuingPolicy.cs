// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQueuingPolicy.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.MessageQueuing
{
    using System;

    /// <summary>
    /// Provides an interface for determining whether to increase the current queue capacity.
    /// </summary>
    public interface IQueuingPolicy
    {
        /// <summary>
        /// Gets or sets the maximum queue count.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is less than one.
        /// </exception>
        int MaxQueueCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to trim idle queues. If true, only one idle queue will be allowed to remain in the
        /// idle queue collection for the queue pool.
        /// </summary>
        bool TrimIdleQueues { get; set; }

        #region Public Methods and Operators

        /// <summary>
        /// Determines whether queue creation should be allowed based on the current state of the queues.
        /// </summary>
        /// <param name="poolState">
        /// The pool state to examine.
        /// </param>
        /// <returns>
        /// <c>true</c> if creating another queue is allowed by the current policy; otherwise, <c>false</c>.
        /// </returns>
        PolicyDecision AllowQueueCreation(IObservablePoolState poolState);

        #endregion
    }
}