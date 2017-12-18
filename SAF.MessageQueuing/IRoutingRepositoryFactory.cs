// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRoutingRepositoryFactory.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface to factories that create IQueueRepository items.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;

    using SAF.Data;

    /// <summary>
    /// Provides an interface to factories that create <see cref="T:SAF.MessageQueuing.IRoutingRepository`1"/> items.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of message being routed.
    /// </typeparam>
    public interface IRoutingRepositoryFactory<TMessage>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// Creates a new repository.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider for the repository.
        /// </param>
        /// <returns>
        /// A new <see cref="T:SAF.MessageQueuing.IRoutingRepository`1"/> with the specified provider.
        /// </returns>
        IRoutingRepository<TMessage> Create(IRepositoryProvider repositoryProvider);
    }
}