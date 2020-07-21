// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepositoryAdapter.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for interacting with data in a repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Startitecture.Orm.Model;

    /// <summary>
    /// Provides an interface for interacting with data in a repository.
    /// </summary>
    public interface IRepositoryAdapter
    {
        /// <summary>
        /// Determines if the repository contains the specified item.
        /// </summary>
        /// <param name="selection">
        /// The selection to search for.
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of item in the repository.
        /// </typeparam>
        /// <returns>
        /// <c>true</c> if the item exists; otherwise, <c>false</c>.
        /// </returns>
        bool Contains<TDataItem>(EntitySelection<TDataItem> selection);

        /// <summary>
        /// Gets the first or default item matching the specified candidate item.
        /// </summary>
        /// <param name="selection">
        /// The selection.
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of data item to retrieve.
        /// </typeparam>
        /// <returns>
        /// The first matching <typeparamref name="TDataItem"/>, or the default value if no item is found.
        /// </returns>
        TDataItem FirstOrDefault<TDataItem>(EntitySelection<TDataItem> selection);

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
        /// <exception cref="System.InvalidOperationException">
        /// The repository could not be queried.
        /// </exception>
        IEnumerable<TDataItem> SelectItems<TDataItem>(EntitySelection<TDataItem> selection);

        /// <summary>
        /// Gets a scalar result for the specified <paramref name="selection"/>.
        /// </summary>
        /// <param name="selection">
        /// The selection to get a scalar value for.
        /// </param>
        /// <typeparam name="T">
        /// The type of the scalar value.
        /// </typeparam>
        /// <returns>
        /// The scalar value as a type of <typeparamref name="T"/>.
        /// </returns>
        T ExecuteScalar<T>(ISelection selection);

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
        /// <exception cref="RepositoryException">
        /// The insert operation failed, or there was an error mapping between the model and the data item.
        /// </exception>
        TDataItem Insert<TDataItem>(TDataItem dataItem) where TDataItem : ITransactionContext;

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
        int Update<TDataItem>(TDataItem dataItem, EntitySelection<TDataItem> selection, params Expression<Func<TDataItem, object>>[] setExpressions);

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
        /// <exception cref="System.InvalidOperationException">
        /// The repository could not be updated.
        /// </exception>
        int DeleteSelection<TDataItem>(EntitySelection<TDataItem> selection);
    }
}