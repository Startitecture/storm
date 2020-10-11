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

    using JetBrains.Annotations;

    using Startitecture.Orm.Model;

    /// <summary>
    /// Provides an interface to concrete entity repositories.
    /// </summary>
    public interface IRepositoryProvider : IDisposable
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
        /// Gets or sets a value indicating whether to enable caching for the current provider.
        /// </summary>
        bool EnableCaching { get; set; }

        /// <summary>
        /// Gets or sets the amount of time after which cached entities will expire.
        /// </summary>
        TimeSpan CacheExpiration { get; set; }

        /// <summary>
        /// Changes the database of the current provider.
        /// </summary>
        /// <param name="databaseName">
        /// The name of the database to switch to.
        /// </param>
        void ChangeDatabase(string databaseName);

        /// <summary>
        /// Starts a transaction in the repository.
        /// </summary>
        /// <returns>
        /// The <see cref="System.Data.IDbTransaction"/> started by the provider.
        /// </returns>
        IDbTransaction StartTransaction();

        /// <summary>
        /// Start a transaction in the repository.
        /// </summary>
        /// <param name="isolationLevel">
        /// The isolation level for the transaction.
        /// </param>
        /// <returns>
        /// The <see cref="System.Data.IDbTransaction"/> started by the provider.
        /// </returns>
        IDbTransaction StartTransaction(IsolationLevel isolationLevel);

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
        /// Gets a scalar result from the specified query.
        /// </summary>
        /// <param name="selection">
        /// A selection for the specified scalar to return.
        /// </param>
        /// <typeparam name="T">
        /// The type of scalar to return.
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T GetScalar<T>(ISelection selection);

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
        /// Selects a collection of dynamic objects matching the <paramref name="selection"/>.
        /// </summary>
        /// <param name="selection">
        /// The selection specifying the results to return.
        /// </param>
        /// <returns>
        /// An <see cref="System.Collections.Generic.IEnumerable{T}"/> of dynamic objects matching the selection.
        /// </returns>
        IEnumerable<dynamic> DynamicSelect(ISelection selection);

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
        /// Executes the specified operation.
        /// </summary>
        /// <param name="executionStatement">
        /// The execution statement.
        /// </param>
        /// <param name="parameterValues">
        /// The parameter values.
        /// </param>
        void Execute([NotNull] string executionStatement, [NotNull] params object[] parameterValues);

        /// <summary>
        /// Executes the specified operation for a scalar result.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the result value.
        /// </typeparam>
        /// <param name="executionStatement">
        /// The execution statement.
        /// </param>
        /// <param name="parameterValues">
        /// The parameter values.
        /// </param>
        /// <returns>
        /// The first column of the first row of the result as a type of <typeparamref name="T"/>.
        /// </returns>
        T ExecuteScalar<T>([NotNull] string executionStatement, [NotNull] params object[] parameterValues);

        /// <summary>
        /// Executes the specified operation for a table result.
        /// </summary>
        /// <param name="executionStatement">
        /// The execution statement.
        /// </param>
        /// <param name="parameterValues">
        /// The parameter values.
        /// </param>
        /// <returns>
        /// An <see cref="System.Collections.Generic.IEnumerable{T}"/> of objects returned by the statement.
        /// </returns>
        IEnumerable<object> ExecuteForResult([NotNull] string executionStatement, [NotNull] params object[] parameterValues);

        /// <summary>
        /// Executes the specified operation for a table result.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the result.
        /// </typeparam>
        /// <param name="executionStatement">
        /// The execution statement.
        /// </param>
        /// <param name="parameterValues">
        /// The parameter values.
        /// </param>
        /// <returns>
        /// An <see cref="System.Collections.Generic.IEnumerable{T}"/> of entities in the type of <typeparamref name="T"/>.
        /// </returns>
        IEnumerable<T> ExecuteForResult<T>([NotNull] string executionStatement, [NotNull] params object[] parameterValues);
    }
}