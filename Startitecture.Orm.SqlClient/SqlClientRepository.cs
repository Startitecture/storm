// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlClientRepository.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   A repository implementation for SQL Server.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using JetBrains.Annotations;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// A repository implementation for SQL Server.
    /// </summary>
    /// <typeparam name="TModel">
    /// The type of domain model represented by the repository.
    /// </typeparam>
    /// <typeparam name="TEntity">
    /// The type of entity stored in the repository.
    /// </typeparam>
    public class SqlClientRepository<TModel, TEntity> : EntityRepository<TModel, TEntity>
        where TEntity : class, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlClientRepository{TModel,TEntity}"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        /// <param name="entityMapper">
        /// The entity mapper.
        /// </param>
        public SqlClientRepository(IRepositoryProvider repositoryProvider, IEntityMapper entityMapper)
            : this(repositoryProvider, entityMapper, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlClientRepository{TModel,TEntity}"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        /// <param name="entityMapper">
        /// The entity mapper.
        /// </param>
        /// <param name="selectionComparer">
        /// The selection comparer for ordering selection results.
        /// </param>
        public SqlClientRepository(IRepositoryProvider repositoryProvider, IEntityMapper entityMapper, IComparer<TEntity> selectionComparer)
            : base(repositoryProvider, entityMapper, selectionComparer)
        {
        }

        /// <summary>
        /// Inserts a set of items into the repository then maps the results back to the <typeparamref name="TItem"/> type.
        /// </summary>
        /// <param name="items">
        /// The items to insert.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item to insert. This type should represent a User-Defined Table Type and be decorated with a <see cref="TableTypeAttribute"/>.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items"/> is null.
        /// </exception>
        public void Insert<TItem>([NotNull] IEnumerable<TItem> items)
        {
            this.Insert(items, null);
        }

        /// <summary>
        /// Inserts a set of items into the repository then maps the results back to the <typeparamref name="TItem"/> type.
        /// </summary>
        /// <param name="items">
        /// The items to insert.
        /// </param>
        /// <param name="insertAction">
        /// The insert action to take, or null to take the default insert action.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item to insert. This type should represent a User-Defined Table Type and be decorated with a <see cref="TableTypeAttribute"/>.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items"/> or <paramref name="insertAction"/> is null.
        /// </exception>
        public void Insert<TItem>([NotNull] IEnumerable<TItem> items, Action<TableValuedInsert<TEntity>> insertAction)
        {
            this.Insert(items, null, insertAction);
        }

        /// <summary>
        /// Inserts a set of items into the repository then maps the results back to the <typeparamref name="TItem"/> type.
        /// </summary>
        /// <param name="items">
        /// The items to insert.
        /// </param>
        /// <param name="transaction">
        /// The transaction for the operation, or null to perform the operation without a transaction.
        /// </param>
        /// <param name="insertAction">
        /// The insert action to take, or null to take the default insert action.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item to insert. This type should represent a User-Defined Table Type and be decorated with a <see cref="TableTypeAttribute"/>.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items"/> is null.
        /// </exception>
        public void Insert<TItem>([NotNull] IEnumerable<TItem> items, IDbTransaction transaction, Action<TableValuedInsert<TEntity>> insertAction)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var commandProvider = new TableValuedCommandProvider(this.RepositoryProvider.DatabaseContext);
            var insertCommand = new TableValuedInsert<TEntity>(commandProvider, transaction);
            insertAction?.Invoke(insertCommand);
            insertCommand.Execute(items);
        }

        /// <summary>
        /// Inserts a set of items into the repository then maps the results back to the <typeparamref name="TItem"/> type.
        /// </summary>
        /// <param name="items">
        /// The items to insert.
        /// </param>
        /// <param name="insertAction">
        /// The insert action to take, or null to take the default insert action.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item to insert. This type should represent a User-Defined Table Type and be decorated with a <see cref="TableTypeAttribute"/>.
        /// </typeparam>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <typeparamref name="TItem"/> items.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items"/> is null.
        /// </exception>
        public IEnumerable<TItem> InsertForResults<TItem>(
            [NotNull] IEnumerable<TItem> items,
            Action<TableValuedInsert<TEntity>> insertAction)
        {
            return this.InsertForResults(items, null, insertAction);
        }

        /// <summary>
        /// Inserts a set of items into the repository then maps the results back to the <typeparamref name="TItem"/> type.
        /// </summary>
        /// <param name="items">
        /// The items to insert.
        /// </param>
        /// <param name="transaction">
        /// The transaction for the operation, or null to perform the operation without a transaction.
        /// </param>
        /// <param name="insertAction">
        /// The insert action to take, or null to take the default insert action.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item to insert. This type should represent a User-Defined Table Type and be decorated with a <see cref="TableTypeAttribute"/>.
        /// </typeparam>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <typeparamref name="TItem"/> items.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items"/> is null.
        /// </exception>
        public IEnumerable<TItem> InsertForResults<TItem>(
            [NotNull] IEnumerable<TItem> items,
            IDbTransaction transaction,
            Action<TableValuedInsert<TEntity>> insertAction)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var commandProvider = new TableValuedCommandProvider(this.RepositoryProvider.DatabaseContext);
            var insertCommand = new TableValuedInsert<TEntity>(commandProvider, transaction);
            insertAction?.Invoke(insertCommand);
            var entities = insertCommand.ExecuteForResults(items);
            return this.EntityMapper.Map<List<TItem>>(entities);
        }

        /// <summary>
        /// Merges a set of items into the repository.
        /// </summary>
        /// <param name="items">
        /// The items to merge.
        /// </param>
        /// <param name="transaction">
        /// The transaction for the operation, or null to perform the operation without a transaction.
        /// </param>
        /// <param name="mergeAction">
        /// The merge action to take, or null to take the default merge action.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item to merge. This type should represent a User-Defined Table Type and be decorated with a <see cref="TableTypeAttribute"/>.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items"/> or <paramref name="mergeAction"/> is null.
        /// </exception>
        /// <remarks>
        /// In SQL Server, MERGE operations are not guaranteed to be atomic.
        /// </remarks>
        public void Merge<TItem>([NotNull] IEnumerable<TItem> items, IDbTransaction transaction, Action<TableValuedMerge<TEntity>> mergeAction)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var commandProvider = new TableValuedCommandProvider(this.RepositoryProvider.DatabaseContext);
            var mergeCommand = new TableValuedMerge<TEntity>(commandProvider, transaction);
            mergeAction?.Invoke(mergeCommand);
            mergeCommand.Execute(items);
        }

        /// <summary>
        /// Merges a set of items into the repository then maps the results back to the <typeparamref name="TItem"/> type.
        /// </summary>
        /// <param name="items">
        /// The items to merge.
        /// </param>
        /// <param name="transaction">
        /// The transaction for the operation, or null to perform the operation without a transaction.
        /// </param>
        /// <param name="mergeAction">
        /// The merge action to take, or null to take the default merge action.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item to merge. This type should represent a User-Defined Table Type and be decorated with a <see cref="TableTypeAttribute"/>.
        /// </typeparam>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <typeparamref name="TItem"/> items.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items"/> or <paramref name="mergeAction"/> is null.
        /// </exception>
        /// <remarks>
        /// In SQL Server, MERGE operations are not guaranteed to be atomic.
        /// </remarks>
        public IEnumerable<TItem> MergeForResults<TItem>(
            [NotNull] IEnumerable<TItem> items,
            IDbTransaction transaction,
            Action<TableValuedMerge<TEntity>> mergeAction)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var commandProvider = new TableValuedCommandProvider(this.RepositoryProvider.DatabaseContext);
            var mergeCommand = new TableValuedMerge<TEntity>(commandProvider, transaction);
            mergeAction?.Invoke(mergeCommand);
            var entities = mergeCommand.ExecuteForResults(items);
            return this.EntityMapper.Map<List<TItem>>(entities);
        }
    }
}
