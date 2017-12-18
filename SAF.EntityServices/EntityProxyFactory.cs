// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityProxyFactory.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   A factory for creating repositories.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.EntityServices
{
    using System;

    using JetBrains.Annotations;

    using SAF.ActionTracking;
    using SAF.Core;
    using SAF.Data;
    using SAF.Data.Providers;
    using SAF.Security;

    /// <summary>
    /// A factory for creating service proxies. Requires the following dependencies to be resolved using 
    /// <see cref="T:SAF.Core.Resolve`1"/>: <see cref="IEntityMapper"/>, and <see cref="IAccountInfoProvider"/>.
    /// </summary>
    /// <typeparam name="TDataItem">
    /// The type of data item that represents the entity in the database.
    /// </typeparam>
    public class EntityProxyFactory<TDataItem> : IEntityProxyFactory
        where TDataItem : class, ITransactionContext, new()
    {
        /// <summary>
        /// The repository provider factory.
        /// </summary>
        private readonly IRepositoryProviderFactory repositoryProviderFactory;

        /// <summary>
        /// The entity mapper.
        /// </summary>
        private readonly IEntityMapper entityMapper;

        /// <summary>
        /// The event repository factory.
        /// </summary>
        private readonly IEventRepositoryFactory eventRepositoryFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SAF.EntityServices.EntityProxyFactory`1"/> class. 
        /// </summary>
        /// <param name="repositoryProviderFactory">
        /// The repository provider factory.
        /// </param>
        /// <param name="entityMapper">
        /// The entity mapper.
        /// </param>
        /// <param name="eventRepositoryFactory">
        /// The event repository factory.
        /// </param>
        protected EntityProxyFactory(
            [NotNull] IRepositoryProviderFactory repositoryProviderFactory,
            [NotNull] IEntityMapper entityMapper,
            [NotNull] IEventRepositoryFactory eventRepositoryFactory)
        {
            if (repositoryProviderFactory == null)
            {
                throw new ArgumentNullException("repositoryProviderFactory");
            }

            if (entityMapper == null)
            {
                throw new ArgumentNullException("entityMapper");
            }

            if (eventRepositoryFactory == null)
            {
                throw new ArgumentNullException("eventRepositoryFactory");
            }

            this.repositoryProviderFactory = repositoryProviderFactory;
            this.entityMapper = entityMapper;
            this.eventRepositoryFactory = eventRepositoryFactory;
        }

        /// <summary>
        /// Creates a service proxy using the <see cref="T:SAF.EntityServices.IEntityProxy`1"/> interface.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The type of entity managed by the proxy.
        /// </typeparam>
        /// <returns>
        /// An <see cref="T:SAF.EntityServices.IEntityProxy`1"/> interface to a service proxy for the specified entity.
        /// </returns>
        public IEntityProxy<TEntity> Create<TEntity>() 
            where TEntity : IValidatingEntity
        {
            var actionEventProxy = new ActionEventProxy(
                EntityOperationContext.Current,
                ServiceExceptionHandler.Default,
                ServiceErrorMapping.Default,
                this.eventRepositoryFactory,
                false);

            return new EntityProxy<TEntity>(
                RepositoryFactory<TEntity, TDataItem>.Default,
                this.repositoryProviderFactory,
                this.entityMapper,
                actionEventProxy);
        }

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
        public IEntityProxy<TEntity> Create<TEntity>(IRepositoryProviderFactory providerFactory)
            where TEntity : IValidatingEntity
        {
            var actionEventProxy = new ActionEventProxy(
                EntityOperationContext.Current,
                ServiceExceptionHandler.Default,
                ServiceErrorMapping.Default,
                this.eventRepositoryFactory,
                false);

            return new EntityProxy<TEntity>(
                RepositoryFactory<TEntity, TDataItem>.Default,
                providerFactory,
                this.entityMapper,
                actionEventProxy);
        }
    }
}