// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityProvider.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides access to a store of entities.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Persistence
{
    using System;
    using System.Collections.Generic;

    using SAF.Data.Providers;

    /// <summary>
    /// Provides access to a store of entities.
    /// </summary>
    /// <typeparam name="TSource">The type of data source that provides access to the entities.</typeparam>
    /// <typeparam name="TEntity">The type of entities in the store.</typeparam>
    public class EntityProvider<TSource, TEntity> : IDataProvider<TSource, TEntity>
        where TSource : IEntityDataSource
    {
        /// <summary>
        /// The internal adapter used by the provider.
        /// </summary>
        private readonly IEntityAdapter<TEntity> adapter;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityProvider&lt;TSource, TEntity&gt;"/> class.
        /// </summary>
        /// <param name="adapter">The adapter used by the provider.</param>
        public EntityProvider(IEntityAdapter<TEntity> adapter)
        {
            this.adapter = adapter;
        }

        /// <summary>
        /// Retrieves items from the entity adapter.
        /// </summary>
        /// <param name="dataSource">
        /// The data source of the entities. The Location property is expected to be the connection string.
        /// </param>
        /// <returns>An enumerable of the entities in the data source.</returns>
        public IEnumerable<TEntity> GetItems(TSource dataSource)
        {
            if (dataSource == null)
            {
                throw new ArgumentNullException("dataSource");
            }

            this.adapter.Initialize(dataSource.Name, dataSource.ConnectionString);
            this.adapter.CommandAdapter.Open();

            foreach (TEntity entity in this.adapter.SelectAllItems())
            {
                yield return entity;
            }

            this.adapter.CommandAdapter.Close();
        }
    }
}
