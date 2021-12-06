// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepositoryProvider.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface to concrete repositories.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq.Expressions;
    using System.Runtime.Caching;
    using System.Threading;
    using System.Threading.Tasks;

    using Startitecture.Orm.Model;

    /// <summary>
    /// Provides an interface to concrete entity repositories.
    /// </summary>
    public interface IRepositoryProvider : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Occurs when the provider is disposed.
        /// </summary>
        event EventHandler Disposed;

        /// <summary>
        /// Gets the database context.
        /// </summary>
        IDatabaseContext DatabaseContext { get; }

        /// <summary>
        /// Gets the entity definition provider.
        /// </summary>
        IEntityDefinitionProvider EntityDefinitionProvider { get; }

        /// <summary>
        /// Gets a value indicating whether the current instance is disposed.
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Gets or sets the cache item policy for single items retrieved using the FirstOrDefault methods.
        /// </summary>
        CacheItemPolicy CacheItemPolicy { get; set; }

        /// <summary>
        /// Starts a transaction in the repository.
        /// </summary>
        /// <returns>
        /// The <see cref="IDbTransaction"/> started by the provider.
        /// </returns>
        ITransactionContext BeginTransaction();

        /// <summary>
        /// Starts a transaction in the repository.
        /// </summary>
        /// <param name="cancellationToken">
        /// The cancellation token for this task.
        /// </param>
        /// <returns>
        /// The <see cref="IDbTransaction"/> started by the provider.
        /// </returns>
        Task<ITransactionContext> BeginTransactionAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Start a transaction in the repository.
        /// </summary>
        /// <param name="isolationLevel">
        /// The isolation level for the transaction.
        /// </param>
        /// <returns>
        /// The <see cref="IDbTransaction"/> started by the provider.
        /// </returns>
        ITransactionContext BeginTransaction(IsolationLevel isolationLevel);

        /// <summary>
        /// Start a transaction in the repository.
        /// </summary>
        /// <param name="isolationLevel">
        /// The isolation level for the transaction.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token for this task.
        /// </param>
        /// <returns>
        /// The <see cref="IDbTransaction"/> started by the provider.
        /// </returns>
        Task<ITransactionContext> BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken);

        /// <summary>
        /// Determines whether an entity exists given the specified unique key.
        /// </summary>
        /// <param name="selection">
        /// A selection for the specified entity or entities to query for existence.
        /// </param>
        /// <returns>
        /// <c>true</c> if the entity exists; otherwise, <c>false</c>.
        /// </returns>
        bool Contains(IEntitySet selection);

        /// <summary>
        /// Determines whether an entity exists given the specified unique key.
        /// </summary>
        /// <param name="selection">
        /// A selection for the specified entity or entities to query for existence.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token for this task.
        /// </param>
        /// <returns>
        /// <c>true</c> if the entity exists; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> ContainsAsync(IEntitySet selection, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a scalar result from the specified query.
        /// </summary>
        /// <param name="selection">
        /// A selection for the specified scalar to return.
        /// </param>
        /// <typeparam name="T">
        /// The type of scalar to return.
        /// </typeparam>
        /// <returns>
        /// The scalar value as a <typeparamref name="T"/>.
        /// </returns>
        T GetScalar<T>(ISelection selection);

        /// <summary>
        /// Gets a scalar result from the specified query.
        /// </summary>
        /// <param name="selection">
        /// A selection for the specified scalar to return.
        /// </param>
        /// <param name="cancellationToken">
        /// Gets the cancellation token for this task.
        /// </param>
        /// <typeparam name="T">
        /// The type of scalar to return.
        /// </typeparam>
        /// <returns>
        /// The scalar value as a <typeparamref name="T"/>.
        /// </returns>
        Task<T> GetScalarAsync<T>(ISelection selection, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the first entity matching the selection, or the default value if the entity cannot be found.
        /// </summary>
        /// <typeparam name="T">
        /// The type of entity in the repository.
        /// </typeparam>
        /// <param name="entitySet">
        /// A selection for the specified entity to return.
        /// </param>
        /// <returns>
        /// The first <typeparamref name="T"/> entity matching the filter, or the default value if no matching entity is found.
        /// </returns>
        T FirstOrDefault<T>(EntitySet<T> entitySet);

        /// <summary>
        /// Gets the first entity matching the selection, or the default value if the entity cannot be found.
        /// </summary>
        /// <typeparam name="T">
        /// The type of entity in the repository.
        /// </typeparam>
        /// <param name="entitySet">
        /// A selection for the specified entity to return.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token for this task.
        /// </param>
        /// <returns>
        /// The first <typeparamref name="T"/> entity matching the filter, or the default value if no matching entity is found.
        /// </returns>
        Task<T> FirstOrDefaultAsync<T>(EntitySet<T> entitySet, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the first dynamic result matching the <paramref name="selection"/>, or the default value if the entity cannot be found.
        /// </summary>
        /// <param name="selection">
        /// A selection for the specified result to return.
        /// </param>
        /// <returns>
        /// The first dynamic result matching the filter, or null if no matching entity is found.
        /// </returns>
        dynamic DynamicFirstOrDefault(ISelection selection);

        /// <summary>
        /// Gets the first dynamic result matching the <paramref name="selection"/>, or the default value if the entity cannot be found.
        /// </summary>
        /// <param name="selection">
        /// A selection for the specified result to return.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token for this task.
        /// </param>
        /// <returns>
        /// The first dynamic result matching the filter, or null if no matching entity is found.
        /// </returns>
        Task<dynamic> DynamicFirstOrDefaultAsync(ISelection selection, CancellationToken cancellationToken);

        /// <summary>
        /// Selects a matching list of entities from the repository.
        /// </summary>
        /// <typeparam name="T">
        /// The type of entity in the repository.
        /// </typeparam>
        /// <param name="selection">
        /// A selection for the specified entities to return.
        /// </param>
        /// <returns>
        /// A collection of entities that match the filter.
        /// </returns>
        IEnumerable<T> SelectEntities<T>(EntitySet<T> selection);

        /// <summary>
        /// Selects a matching list of entities from the repository.
        /// </summary>
        /// <typeparam name="T">
        /// The type of entity in the repository.
        /// </typeparam>
        /// <param name="selection">
        /// A selection for the specified entities to return.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token for this class.
        /// </param>
        /// <returns>
        /// A collection of entities that match the filter.
        /// </returns>
        IAsyncEnumerable<T> SelectEntitiesAsync<T>(EntitySet<T> selection, CancellationToken cancellationToken);

        /// <summary>
        /// Selects a collection of dynamic objects matching the <paramref name="selection"/>.
        /// </summary>
        /// <param name="selection">
        /// The selection specifying the results to return.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of dynamic objects matching the selection.
        /// </returns>
        IEnumerable<dynamic> DynamicSelect(ISelection selection);

        /// <summary>
        /// Selects a collection of dynamic objects matching the <paramref name="selection"/>.
        /// </summary>
        /// <param name="selection">
        /// The selection specifying the results to return.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token for this task.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of dynamic objects matching the selection.
        /// </returns>
        IAsyncEnumerable<dynamic> DynamicSelectAsync(ISelection selection, CancellationToken cancellationToken);

        /// <summary>
        /// Inserts an entity into the repository.
        /// </summary>
        /// <typeparam name="T">
        /// The type of entity in the repository.
        /// </typeparam>
        /// <param name="entity">
        /// The entity to insert.
        /// </param>
        /// <returns>
        /// The inserted <typeparamref name="T"/>.
        /// </returns>
        T Insert<T>(T entity);

        /// <summary>
        /// Inserts an entity into the repository.
        /// </summary>
        /// <typeparam name="T">
        /// The type of entity in the repository.
        /// </typeparam>
        /// <param name="entity">
        /// The entity to insert.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token for this task.
        /// </param>
        /// <returns>
        /// The inserted <typeparamref name="T"/>.
        /// </returns>
        Task<T> InsertAsync<T>(T entity, CancellationToken cancellationToken);

        /// <summary>
        /// Updates a set of entities in the repository.
        /// </summary>
        /// <param name="updateSet">
        /// The update set that defines the entities and entity attributes to update.
        /// </param>
        /// <typeparam name="T">
        /// The type of entity to update.
        /// </typeparam>
        /// <returns>
        /// The number of affected entities as an <see cref="int"/>.
        /// </returns>
        /// <remarks>
        /// The number of affected entities can include rows affected by triggers on the target table.
        /// </remarks>
        int Update<T>(UpdateSet<T> updateSet);

        /// <summary>
        /// Updates a set of entities in the repository.
        /// </summary>
        /// <param name="updateSet">
        /// The update set that defines the entities and entity attributes to update.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token for this task.
        /// </param>
        /// <typeparam name="T">
        /// The type of entity to update.
        /// </typeparam>
        /// <returns>
        /// The number of affected entities as an <see cref="int"/>.
        /// </returns>
        /// <remarks>
        /// The number of affected entities can include rows affected by triggers on the target table.
        /// </remarks>
        Task<int> UpdateAsync<T>(UpdateSet<T> updateSet, CancellationToken cancellationToken);

        /// <summary>
        /// Updates a selection of entities in the repository.
        /// </summary>
        /// <typeparam name="T">
        /// The type of entity in the repository.
        /// </typeparam>
        /// <param name="entity">
        /// The entity that contains the update.
        /// </param>
        /// <param name="setExpressions">
        /// A optional set of expressions that explicitly select the columns to update. If empty, all non-key columns are updated.
        /// </param>
        void UpdateSingle<T>(T entity, params Expression<Func<T, object>>[] setExpressions);

        /// <summary>
        /// Updates a selection of entities in the repository.
        /// </summary>
        /// <typeparam name="T">
        /// The type of entity in the repository.
        /// </typeparam>
        /// <param name="entity">
        /// The entity that contains the update.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token for this task.
        /// </param>
        /// <param name="setExpressions">
        /// A optional set of expressions that explicitly select the columns to update. If empty, all non-key columns are updated.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> that is performing the update.
        /// </returns>
        Task UpdateSingleAsync<T>(T entity, CancellationToken cancellationToken, params Expression<Func<T, object>>[] setExpressions);

        /// <summary>
        /// Deletes the entities matching the set.
        /// </summary>
        /// <param name="entitySet">
        /// The entity set to delete.
        /// </param>
        /// <returns>
        /// The number of deleted entities as an <see cref="int"/>.
        /// </returns>
        int Delete(IEntitySet entitySet);

        /// <summary>
        /// Deletes the entities matching the set.
        /// </summary>
        /// <param name="entitySet">
        /// The entity set to delete.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token for this task.
        /// </param>
        /// <returns>
        /// The number of deleted entities as an <see cref="int"/>.
        /// </returns>
        Task<int> DeleteAsync(IEntitySet entitySet, CancellationToken cancellationToken);
    }
}