﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryRepositoryAdapter.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.Data.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Linq.Expressions;

    using JetBrains.Annotations;

    using SAF.Core;
    using SAF.StringResources;

    /// <summary>
    /// Implements the <see cref="IRepositoryAdapter"/> interface using parameterized queries.
    /// </summary>
    public class QueryRepositoryAdapter : IRepositoryAdapter
    {
        /// <summary>
        /// The data context.
        /// </summary>
        private readonly IDatabaseContext dataContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryRepositoryAdapter"/> class.
        /// </summary>
        /// <param name="dataContext">
        /// The data context.
        /// </param>
        public QueryRepositoryAdapter(IDatabaseContext dataContext)
        {
            this.dataContext = dataContext;
        }

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
        public bool Contains<TDataItem>([NotNull] ItemSelection<TDataItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            // Always remember to supply this method with an array of values!
            return this.dataContext.ExecuteScalar<int>(selection.ContainsStatement, selection.PropertyValues.ToArray()) > 0;
        }

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
        public TDataItem FirstOrDefault<TDataItem>([NotNull] ItemSelection<TDataItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            try
            {
                var statement = selection.SelectionStatement;

                ////Trace.TraceInformation("Using unique query: {0} [{1}]", sql.SQL, String.Join(", ", sql.Arguments));
                return this.dataContext.FirstOrDefault<TDataItem>(statement, selection.PropertyValues.ToArray());
            }
            catch (InvalidOperationException ex)
            {
                throw new RepositoryException(selection, ex.Message, ex);
            }
            catch (DataException ex)
            {
                throw new RepositoryException(selection, ex.Message, ex);
            }
            catch (SqlException ex)
            {
                throw new RepositoryException(selection, ex.Message, ex);
            }
        }

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
        public IEnumerable<TDataItem> SelectItems<TDataItem>(ItemSelection<TDataItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            try
            {
                var statement = selection.SelectionStatement;

                ////Trace.TraceInformation("Using select query: {0} [{1}]", sql.SQL, String.Join(", ", sql.Arguments));
                return this.dataContext.Fetch<TDataItem>(statement, selection.PropertyValues.ToArray());
            }
            catch (InvalidOperationException ex)
            {
                throw new RepositoryException(selection, ex.Message, ex);
            }
            catch (DataException ex)
            {
                throw new RepositoryException(selection, ex.Message, ex);
            }
            catch (SqlException ex)
            {
                throw new RepositoryException(selection, ex.Message, ex);
            }
        }

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
        /// The 1-based page number of the page to retrieve.
        /// </param>
        /// <returns>
        /// A collection of items that match the filter.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// The repository could not be queried.
        /// </exception>
        public Page<TDataItem> SelectItems<TDataItem>(ItemSelection<TDataItem> selection, long pageSize, long page)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), ValidationMessages.ValueMustBeGreaterThanZero);
            }

            if (page < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(page), ValidationMessages.ValueMustBeGreaterThanZero);
            }

            try
            {
                var statement = selection.SelectionStatement;

                ////Trace.TraceInformation("Using select query: {0} [{1}]", sql.SQL, String.Join(", ", sql.Arguments));
                return this.dataContext.FetchPage<TDataItem>(page, pageSize, statement, selection.PropertyValues.ToArray());
            }
            catch (InvalidOperationException ex)
            {
                throw new RepositoryException(selection, ex.Message, ex);
            }
            catch (DataException ex)
            {
                throw new RepositoryException(selection, ex.Message, ex);
            }
            catch (SqlException ex)
            {
                throw new RepositoryException(selection, ex.Message, ex);
            }
        }

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
        /// <exception cref="SAF.Data.RepositoryException">
        /// The insert operation failed, or there was an error mapping between the model and the data item.
        /// </exception>
        public TDataItem Insert<TDataItem>(TDataItem dataItem)
            where TDataItem : ITransactionContext
        {
            if (Evaluate.IsNull(dataItem))
            {
                throw new ArgumentNullException(nameof(dataItem));
            }

            object result;

            try
            {
                result = this.dataContext.Insert(dataItem);
            }
            catch (InvalidOperationException ex)
            {
                throw new RepositoryException(dataItem, ex.Message, ex);
            }
            catch (DataException ex)
            {
                throw new RepositoryException(dataItem, ex.Message, ex);
            }
            catch (SqlException ex)
            {
                throw new RepositoryException(dataItem, ex.Message, ex);
            }

            if (result == null)
            {
                string message = String.Format(ErrorMessages.DataItemInsertionFailed, typeof(TDataItem).Name, dataItem);
                throw new RepositoryException(dataItem, message);
            }

            return dataItem;
        }

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
        public int Update<TDataItem>(
            [NotNull] TDataItem dataItem,
            [NotNull] ItemSelection<TDataItem> selection,
            [NotNull] params Expression<Func<TDataItem, object>>[] setExpressions)
        {
            if (dataItem == null)
            {
                throw new ArgumentNullException(nameof(dataItem));
            }

            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            if (setExpressions == null)
            {
                throw new ArgumentNullException(nameof(setExpressions));
            }

            var transactSqlUpdate = new TransactSqlUpdate<TDataItem>(selection);
            var updateOperation = setExpressions.Any() ? transactSqlUpdate.Set(dataItem, setExpressions) : transactSqlUpdate.Set(dataItem);

            try
            {
                // Always use ToArray()!
                return this.dataContext.Execute(updateOperation.ExecutionStatement, updateOperation.ExecutionParameters.ToArray());
            }
            catch (InvalidOperationException ex)
            {
                throw new RepositoryException(dataItem, ex.Message, ex);
            }
            catch (DataException ex)
            {
                throw new RepositoryException(dataItem, ex.Message, ex);
            }
            catch (SqlException ex)
            {
                throw new RepositoryException(dataItem, ex.Message, ex);
            }
        }

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
        public int DeleteSelection<TDataItem>(ItemSelection<TDataItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            try
            {
                return this.dataContext.Execute(selection.RemovalStatement, selection.PropertyValues.ToArray());
            }
            catch (InvalidOperationException ex)
            {
                throw new RepositoryException(selection, ex.Message, ex);
            }
            catch (DataException ex)
            {
                throw new RepositoryException(selection, ex.Message, ex);
            }
            catch (SqlException ex)
            {
                throw new RepositoryException(selection, ex.Message, ex);
            }
        }
    }
}
