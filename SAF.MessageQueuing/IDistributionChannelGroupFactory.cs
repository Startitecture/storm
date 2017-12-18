// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDistributionChannelGroupFactory.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;

    /// <summary>
    /// Provides an interface for classes that create distribution channel groups.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of message distributed by the channel group.
    /// </typeparam>
    public interface IDistributionChannelGroupFactory<TMessage>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// Creates a distribution channel group.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:SAF.MessageQueuing.IDistributionChannelGroup`1"/> instance.
        /// </returns>
        IDistributionChannelGroup<TMessage> Create();
    }
}