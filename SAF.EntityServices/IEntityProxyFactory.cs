// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityProxyFactory.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface to factories that create <see cref="T:SAF.EntityServices.IEntityProxy`1" /> items.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.EntityServices
{
    using SAF.Core;
    using SAF.Data;

    /// <summary>
    /// Provides an interface to factories that create <see cref="T:SAF.EntityServices.IEntityProxy`1"/> items.
    /// </summary>
    public interface IEntityProxyFactory
    {
        /// <summary>
        /// Creates a service proxy using the <see cref="T:SAF.EntityServices.IEntityProxy`1"/> interface.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The type of entity managed by the proxy.
        /// </typeparam>
        /// <returns>
        /// An <see cref="T:SAF.EntityServices.IEntityProxy`1"/> interface to a service proxy for the specified entity.
        /// </returns>
        IEntityProxy<TEntity> Create<TEntity>() 
            where TEntity : IValidatingEntity;

        /// <summary>
        /// Creates a service proxy using the <see cref="T:SAF.EntityServices.IEntityProxy`1"/> interface.
        /// </summary>
        /// <param name="providerFactory">
        /// The repository provider factory.
        /// </param>
        /// <typeparam name="TEntity">
        /// The type of entity managed by the proxy.
        /// </typeparam>
        /// <returns>
        /// An <see cref="T:SAF.EntityServices.IEntityProxy`1"/> interface to a service proxy for the specified entity.
        /// </returns>
        IEntityProxy<TEntity> Create<TEntity>(IRepositoryProviderFactory providerFactory)
            where TEntity : IValidatingEntity;
    }
}