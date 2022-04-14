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
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

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
    public class SqlClientRepository<TModel, TEntity> : EntityRepository<TModel, TEntity>, ISqlClientRepository<TModel, TEntity>
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
            : base(repositoryProvider, entityMapper)
        {
        }

        /// <inheritdoc/>
        public void Insert<TItem>(IEnumerable<TItem> items, Action<TransactSqlInsertBase<TEntity>> insertAction)
        {
            TransactSqlInsertBase<TEntity> insertCommand;

            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (typeof(TItem).GetCustomAttribute<TableTypeAttribute>() == null)
            {
                insertCommand = new JsonInsert<TEntity>(this.RepositoryProvider.DatabaseContext);
            }
            else
            {
                insertCommand = new TableValuedInsert<TEntity>(this.RepositoryProvider.DatabaseContext);
            }

            insertAction?.Invoke(insertCommand);
            insertCommand.Execute(items);
        }

        /// <inheritdoc />
        public async Task InsertAsync<TItem>(
            IEnumerable<TItem> items,
            Action<TransactSqlInsertBase<TEntity>> insertAction,
            CancellationToken cancellationToken)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            TransactSqlInsertBase<TEntity> insertCommand;

            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (typeof(TItem).GetCustomAttribute<TableTypeAttribute>() == null)
            {
                insertCommand = new JsonInsert<TEntity>(this.RepositoryProvider.DatabaseContext);
            }
            else
            {
                insertCommand = new TableValuedInsert<TEntity>(this.RepositoryProvider.DatabaseContext);
            }

            insertAction?.Invoke(insertCommand);
            await insertCommand.ExecuteAsync(items, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public IEnumerable<TItem> InsertForResults<TItem>(
            IEnumerable<TItem> items,
            Action<TransactSqlInsertBase<TEntity>> insertAction)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            TransactSqlInsertBase<TEntity> insertCommand;

            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (typeof(TItem).GetCustomAttribute<TableTypeAttribute>() == null)
            {
                insertCommand = new JsonInsert<TEntity>(this.RepositoryProvider.DatabaseContext);
            }
            else
            {
                insertCommand = new TableValuedInsert<TEntity>(this.RepositoryProvider.DatabaseContext);
            }

            insertAction?.Invoke(insertCommand);
            var entities = insertCommand.ExecuteForResults(items);
            return this.EntityMapper.Map<List<TItem>>(entities);
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<TItem> InsertForResultsAsync<TItem>(
            IEnumerable<TItem> items,
            Action<TransactSqlInsertBase<TEntity>> insertAction,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            TransactSqlInsertBase<TEntity> insertCommand;

            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (typeof(TItem).GetCustomAttribute<TableTypeAttribute>() == null)
            {
                insertCommand = new JsonInsert<TEntity>(this.RepositoryProvider.DatabaseContext);
            }
            else
            {
                insertCommand = new TableValuedInsert<TEntity>(this.RepositoryProvider.DatabaseContext);
            }

            insertAction?.Invoke(insertCommand);

            await foreach (var item in insertCommand.ExecuteForResultsAsync(items, cancellationToken).ConfigureAwait(false))
            {
                yield return this.EntityMapper.Map<TItem>(item);
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// In SQL Server, MERGE operations are not guaranteed to be atomic.
        /// </remarks>
        public void Merge<TItem>(IEnumerable<TItem> items, Action<TransactSqlMergeBase<TEntity>> mergeAction)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            TransactSqlMergeBase<TEntity> mergeCommand;

            if (typeof(TItem).GetCustomAttribute<TableTypeAttribute>() == null)
            {
                mergeCommand = new JsonMerge<TEntity>(this.RepositoryProvider.DatabaseContext);
            }
            else
            {
                mergeCommand = new TableValuedMerge<TEntity>(this.RepositoryProvider.DatabaseContext);
            }

            mergeAction?.Invoke(mergeCommand);
            mergeCommand.Execute(items);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// In SQL Server, MERGE operations are not guaranteed to be atomic.
        /// </remarks>
        public async Task MergeAsync<TItem>(
            IEnumerable<TItem> items,
            Action<TransactSqlMergeBase<TEntity>> mergeAction,
            CancellationToken cancellationToken)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            TransactSqlMergeBase<TEntity> mergeCommand;

            if (typeof(TItem).GetCustomAttribute<TableTypeAttribute>() == null)
            {
                mergeCommand = new JsonMerge<TEntity>(this.RepositoryProvider.DatabaseContext);
            }
            else
            {
                mergeCommand = new TableValuedMerge<TEntity>(this.RepositoryProvider.DatabaseContext);
            }

            mergeAction?.Invoke(mergeCommand);
            await mergeCommand.ExecuteAsync(items, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// In SQL Server, MERGE operations are not guaranteed to be atomic.
        /// </remarks>
        public IEnumerable<TItem> MergeForResults<TItem>(
            IEnumerable<TItem> items,
            Action<TransactSqlMergeBase<TEntity>> mergeAction)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            TransactSqlMergeBase<TEntity> mergeCommand;

            if (typeof(TItem).GetCustomAttribute<TableTypeAttribute>() == null)
            {
                mergeCommand = new JsonMerge<TEntity>(this.RepositoryProvider.DatabaseContext);
            }
            else
            {
                mergeCommand = new TableValuedMerge<TEntity>(this.RepositoryProvider.DatabaseContext);
            }

            mergeAction?.Invoke(mergeCommand);
            var entities = mergeCommand.ExecuteForResults(items);
            return this.EntityMapper.Map<List<TItem>>(entities);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// In SQL Server, MERGE operations are not guaranteed to be atomic.
        /// </remarks>
        public async IAsyncEnumerable<TItem> MergeForResultsAsync<TItem>(
            IEnumerable<TItem> items,
            Action<TransactSqlMergeBase<TEntity>> mergeAction,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            TransactSqlMergeBase<TEntity> mergeCommand;

            if (typeof(TItem).GetCustomAttribute<TableTypeAttribute>() == null)
            {
                mergeCommand = new JsonMerge<TEntity>(this.RepositoryProvider.DatabaseContext);
            }
            else
            {
                mergeCommand = new TableValuedMerge<TEntity>(this.RepositoryProvider.DatabaseContext);
            }

            mergeAction?.Invoke(mergeCommand);

            await foreach (var item in mergeCommand.ExecuteForResultsAsync(items, cancellationToken).ConfigureAwait(false))
            {
                yield return this.EntityMapper.Map<TItem>(item);
            }
        }
    }
}