// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityServiceBase.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Base class for entity-based services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.EntityServices
{
    using System;
    using System.Diagnostics;

    using SAF.Core;
    using SAF.Data;

    /// <summary>
    /// Base class for entity-based services.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of entity that the service manages.
    /// </typeparam>
    public class EntityServiceBase<TEntity>
        where TEntity : IValidatingEntity
    {
        /// <summary>
        /// The entity proxy.
        /// </summary>
        private readonly Lazy<IEntityProxy<TEntity>> entityProxy;

        /// <summary>
        /// The service watch.
        /// </summary>
        private readonly Stopwatch serviceWatch = Stopwatch.StartNew();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SAF.EntityServices.EntityServiceBase`1"/> class.
        /// </summary>
        /// <param name="proxyFactory">
        /// The entity proxy factory.
        /// </param>
        protected EntityServiceBase(IEntityProxyFactory proxyFactory)
        {
            this.entityProxy = new Lazy<IEntityProxy<TEntity>>(proxyFactory.Create<TEntity>);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityServiceBase{TEntity}"/> class.
        /// </summary>
        /// <param name="proxyFactory">
        /// The entity proxy factory.
        /// </param>
        /// <param name="providerFactory">
        /// The repository provider factory.
        /// </param>
        protected EntityServiceBase(IEntityProxyFactory proxyFactory, IRepositoryProviderFactory providerFactory)
        {
            this.entityProxy = new Lazy<IEntityProxy<TEntity>>(() => proxyFactory.Create<TEntity>(providerFactory));
        }

        /// <summary>
        /// Gets the environment proxy.
        /// </summary>
        protected IEntityProxy<TEntity> EntityProxy
        {
            get
            {
                return this.entityProxy.Value;
            }
        }

        /// <summary>
        /// Gets the service watch.
        /// </summary>
        protected Stopwatch ServiceWatch
        {
            get
            {
                return this.serviceWatch;
            }
        }
    }
}
