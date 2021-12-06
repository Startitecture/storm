// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISqlClientRepository.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// Provides an interface for SQL Client repositories.
    /// </summary>
    /// <typeparam name="TModel">
    /// The type of model represented by the repository.
    /// </typeparam>
    /// <typeparam name="TEntity">
    /// The type of entity stored in the repository.
    /// </typeparam>
    public interface ISqlClientRepository<TModel, TEntity> : IEntityRepository<TModel>
        where TEntity : class, new()
    {
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
        /// <paramref name="items"/> is null.
        /// </exception>
        void Insert<TItem>([NotNull] IEnumerable<TItem> items, Action<TransactSqlInsertBase<TEntity>> insertAction);

        /// <summary>
        /// Inserts a set of items into the repository then maps the results back to the <typeparamref name="TItem"/> type.
        /// </summary>
        /// <param name="items">
        /// The items to insert.
        /// </param>
        /// <param name="insertAction">
        /// The insert action to take, or null to take the default insert action.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token for this task.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item to insert. This type should represent a User-Defined Table Type and be decorated with a <see cref="TableTypeAttribute"/>.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items"/> is null.
        /// </exception>
        /// <returns>
        /// The <see cref="Task"/> that is performing the insert.
        /// </returns>
        Task InsertAsync<TItem>(
            [NotNull] IEnumerable<TItem> items,
            Action<TransactSqlInsertBase<TEntity>> insertAction,
            CancellationToken cancellationToken);

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
        IEnumerable<TItem> InsertForResults<TItem>(
            [NotNull] IEnumerable<TItem> items,
            Action<TransactSqlInsertBase<TEntity>> insertAction);

        /// <summary>
        /// Inserts a set of items into the repository then maps the results back to the <typeparamref name="TItem"/> type.
        /// </summary>
        /// <param name="items">
        /// The items to insert.
        /// </param>
        /// <param name="insertAction">
        /// The insert action to take, or null to take the default insert action.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token for this task.
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
        IAsyncEnumerable<TItem> InsertForResultsAsync<TItem>(
            [NotNull] IEnumerable<TItem> items,
            Action<TransactSqlInsertBase<TEntity>> insertAction,
            CancellationToken cancellationToken);

        /// <summary>
        /// Merges a set of items into the repository.
        /// </summary>
        /// <param name="items">
        /// The items to merge.
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
        void Merge<TItem>([NotNull] IEnumerable<TItem> items, Action<TransactSqlMergeBase<TEntity>> mergeAction);

        /// <summary>
        /// Merges a set of items into the repository.
        /// </summary>
        /// <param name="items">
        /// The items to merge.
        /// </param>
        /// <param name="mergeAction">
        /// The merge action to take, or null to take the default merge action.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token for this task.
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
        /// <returns>
        /// The <see cref="Task"/> that is executing the merge operation.
        /// </returns>
        Task MergeAsync<TItem>(
            [NotNull] IEnumerable<TItem> items,
            Action<TransactSqlMergeBase<TEntity>> mergeAction,
            CancellationToken cancellationToken);

        /// <summary>
        /// Merges a set of items into the repository then maps the results back to the <typeparamref name="TItem"/> type.
        /// </summary>
        /// <param name="items">
        /// The items to merge.
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
        IEnumerable<TItem> MergeForResults<TItem>(
            [NotNull] IEnumerable<TItem> items,
            Action<TransactSqlMergeBase<TEntity>> mergeAction);

        /// <summary>
        /// Merges a set of items into the repository then maps the results back to the <typeparamref name="TItem"/> type.
        /// </summary>
        /// <param name="items">
        /// The items to merge.
        /// </param>
        /// <param name="mergeAction">
        /// The merge action to take, or null to take the default merge action.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token for this task.
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
        IAsyncEnumerable<TItem> MergeForResultsAsync<TItem>(
            [NotNull] IEnumerable<TItem> items,
            Action<TransactSqlMergeBase<TEntity>> mergeAction,
            CancellationToken cancellationToken);
    }
}