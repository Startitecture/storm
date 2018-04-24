// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepositoryProvider.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
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
    using Startitecture.Orm.Query;

    /// <summary>
    /// Provides an interface to concrete repositories.
    /// </summary>
    public interface IRepositoryProvider : IDisposable, IDependencyItem
    {
        /// <summary>
        /// Occurs when the provider is disposed.
        /// </summary>
        event EventHandler Disposed;

        /// <summary>
        /// Gets the entity mapper.
        /// </summary>
        IEntityMapper EntityMapper { get; }

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
        /// Gets or sets the amount of time after which cached items will expire.
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
        /// The <see cref="IDbTransaction"/> started by the provider.
        /// </returns>
        IDbTransaction StartTransaction();

        /// <summary>
        /// Start a transaction in the repository.
        /// </summary>
        /// <param name="isolationLevel">
        /// The isolation level for the transaction.
        /// </param>
        /// <returns>
        /// The <see cref="IDbTransaction"/> started by the provider.
        /// </returns>
        IDbTransaction StartTransaction(IsolationLevel isolationLevel);

        /// <summary>
        /// Completes a transaction in the repository.
        /// </summary>
        void CompleteTransaction();

        /// <summary>
        /// Aborts a transaction, rolling back changes in the repository.
        /// </summary>
        void AbortTransaction();

        /// <summary>
        /// Determines whether an item exists given the specified unique key.
        /// </summary>
        /// <typeparam name="TDataItem">
        /// The type of data item in the repository.
        /// </typeparam>
        /// <param name="selection">
        /// A selection that contains the SQL filter and values to select the item.
        /// </param>
        /// <returns>
        /// <c>true</c> if the item exists; otherwise, <c>false</c>.
        /// </returns>
        bool Contains<TDataItem>(ItemSelection<TDataItem> selection)
            where TDataItem : ITransactionContext;

        /// <summary>
        /// Gets the first item matching the filter, or the default value if the item cannot be found.
        /// </summary>
        /// <typeparam name="TDataItem">
        /// The type of data item in the repository.
        /// </typeparam>
        /// <param name="selection">
        /// The data item that represents the item to retrieve.
        /// </param>
        /// <returns>
        /// The first <typeparamref name="TDataItem"/> item matching the filter, or the default value if no matching item is found.
        /// </returns>
        TDataItem GetFirstOrDefault<TDataItem>(ItemSelection<TDataItem> selection)
            where TDataItem : ITransactionContext;

        /// <summary>
        /// Selects a matching list of items from the repository.
        /// </summary>
        /// <typeparam name="TDataItem">
        /// The type of data item in the repository.
        /// </typeparam>
        /// <param name="selection">
        /// A selection that contains the SQL filter and values to select the item.
        /// </param>
        /// <returns>
        /// A collection of items that match the filter.
        /// </returns>
        IEnumerable<TDataItem> GetSelection<TDataItem>(ItemSelection<TDataItem> selection)
            where TDataItem : ITransactionContext;

        /// <summary>
        /// Selects a matching list of items from the repository.
        /// </summary>
        /// <typeparam name="TDataItem">
        /// The type of data item in the repository.
        /// </typeparam>
        /// <param name="selection">
        /// A selection that contains the SQL filter and values to select the item.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <param name="page">
        /// The 1-based page to return.
        /// </param>
        /// <returns>
        /// A collection of items that match the filter.
        /// </returns>
        Page<TDataItem> GetSelection<TDataItem>(ItemSelection<TDataItem> selection, long pageSize, long page)
            where TDataItem : ITransactionContext;

        /// <summary>
        /// Saves an item into the repository.
        /// </summary>
        /// <typeparam name="TDataItem">
        /// The type of data item in the repository.
        /// </typeparam>
        /// <param name="item">
        /// The item to save.
        /// </param>
        /// <returns>
        /// The saved item as a <typeparamref name="TDataItem"/>.
        /// </returns>
        /// <exception cref="RepositoryException">
        /// The item could not be saved in the repository.
        /// </exception>
        TDataItem Save<TDataItem>(TDataItem item)
            where TDataItem : ITransactionContext;

        /// <summary>
        /// Deletes the items matching the filter.
        /// </summary>
        /// <typeparam name="TDataItem">
        /// The type of data item in the repository.
        /// </typeparam>
        /// <param name="selection">
        /// A selection that contains the SQL filter and values to select the item.
        /// </param>
        /// <returns>
        /// The number of deleted items as an <see cref="int"/>.
        /// </returns>
        int DeleteItems<TDataItem>(ItemSelection<TDataItem> selection)
            where TDataItem : ITransactionContext;

        /// <summary>
        /// Inserts a data item into the repository.
        /// </summary>
        /// <typeparam name="TDataItem">
        /// The type of data item in the repository.
        /// </typeparam>
        /// <param name="dataItem">
        /// The data item to insert.
        /// </param>
        /// <returns>
        /// The inserted <typeparamref name="TDataItem"/>.
        /// </returns>
        TDataItem InsertItem<TDataItem>(TDataItem dataItem) 
            where TDataItem : ITransactionContext;

        /// <summary>
        /// Updates a selection of items in the repository.
        /// </summary>
        /// <typeparam name="TDataItem">
        /// The type of data item in the repository.
        /// </typeparam>
        /// <param name="dataItem">
        /// The item that contains the update.
        /// </param>
        /// <param name="setExpressions">
        /// A optional set of expressions that explicitly select the columns to update. If empty, all non-key columns are updated.
        /// </param>
        void UpdateItem<TDataItem>(TDataItem dataItem, params Expression<Func<TDataItem, object>>[] setExpressions);

        /// <summary>
        /// Updates a selection of items in the repository.
        /// </summary>
        /// <typeparam name="TDataItem">
        /// The type of data item in the repository.
        /// </typeparam>
        /// <param name="dataItem">
        /// The item that contains the update.
        /// </param>
        /// <param name="selection">
        /// The selection to update.
        /// </param>
        /// <param name="setExpressions">
        /// A optional set of expressions that explicitly select the columns to update. If empty, all non-key columns are updated.
        /// </param>
        /// <returns>
        /// The number of updated rows.
        /// </returns>
        int Update<TDataItem>(TDataItem dataItem, ItemSelection<TDataItem> selection, params Expression<Func<TDataItem, object>>[] setExpressions)
            where TDataItem : ITransactionContext;

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
        /// An <see cref="IEnumerable{T}"/> of objects returned by the statement.
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
        /// An <see cref="IEnumerable{T}"/> of items in the type of <typeparamref name="T"/>.
        /// </returns>
        IEnumerable<T> ExecuteForResult<T>([NotNull] string executionStatement, [NotNull] params object[] parameterValues);
    }
}