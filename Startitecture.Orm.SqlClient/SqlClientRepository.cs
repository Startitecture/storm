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
        /// <param name="insertAction">
        /// The insert action to take, or null to take the default insert action.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item to insert. This type should represent a User-Defined Table Type and be decorated with a <see cref="TableTypeAttribute"/>.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items"/> is null.
        /// </exception>
        public void Insert<TItem>(IEnumerable<TItem> items, Action<TransactSqlInsertBase<TEntity>> insertAction)
        {
            TransactSqlInsertBase<TEntity> insertCommand;

            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (typeof(TItem).GetCustomAttribute<TableTypeAttribute>() == null)
            {
                var commandProvider = new JsonParameterCommandFactory(this.RepositoryProvider.DatabaseContext);
                insertCommand = new JsonInsert<TEntity>(commandProvider, this.RepositoryProvider.DatabaseContext);
            }
            else
            {
                var commandProvider = new TableValuedParameterCommandFactory(this.RepositoryProvider.DatabaseContext);
                insertCommand = new TableValuedInsert<TEntity>(commandProvider, this.RepositoryProvider.DatabaseContext);
            }

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
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items"/> is null.
        /// </exception>
        /// <returns>
        /// The <see cref="Task"/> that is performing the insert.
        /// </returns>
        public async Task InsertAsync<TItem>(IEnumerable<TItem> items, Action<TransactSqlInsertBase<TEntity>> insertAction)
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
                var commandProvider = new JsonParameterCommandFactory(this.RepositoryProvider.DatabaseContext);
                insertCommand = new JsonInsert<TEntity>(commandProvider, this.RepositoryProvider.DatabaseContext);
            }
            else
            {
                var commandProvider = new TableValuedParameterCommandFactory(this.RepositoryProvider.DatabaseContext);
                insertCommand = new TableValuedInsert<TEntity>(commandProvider, this.RepositoryProvider.DatabaseContext);
            }

            insertAction?.Invoke(insertCommand);
            await insertCommand.ExecuteAsync(items).ConfigureAwait(false);
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
                var commandProvider = new JsonParameterCommandFactory(this.RepositoryProvider.DatabaseContext);
                insertCommand = new JsonInsert<TEntity>(commandProvider, this.RepositoryProvider.DatabaseContext);
            }
            else
            {
                var commandProvider = new TableValuedParameterCommandFactory(this.RepositoryProvider.DatabaseContext);
                insertCommand = new TableValuedInsert<TEntity>(commandProvider, this.RepositoryProvider.DatabaseContext);
            }

            insertAction?.Invoke(insertCommand);
            var entities = insertCommand.ExecuteForResults(items);
            return this.EntityMapper.Map<List<TItem>>(entities);
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
        public async Task<IEnumerable<TItem>> InsertForResultsAsync<TItem>(
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
                var commandProvider = new JsonParameterCommandFactory(this.RepositoryProvider.DatabaseContext);
                insertCommand = new JsonInsert<TEntity>(commandProvider, this.RepositoryProvider.DatabaseContext);
            }
            else
            {
                var commandProvider = new TableValuedParameterCommandFactory(this.RepositoryProvider.DatabaseContext);
                insertCommand = new TableValuedInsert<TEntity>(commandProvider, this.RepositoryProvider.DatabaseContext);
            }

            insertAction?.Invoke(insertCommand);
            var entities = await insertCommand.ExecuteForResultsAsync(items).ConfigureAwait(false);
            return this.EntityMapper.Map<List<TItem>>(entities);
        }

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
        public void Merge<TItem>(IEnumerable<TItem> items, Action<TransactSqlMergeBase<TEntity>> mergeAction)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            TransactSqlMergeBase<TEntity> mergeCommand;

            if (typeof(TItem).GetCustomAttribute<TableTypeAttribute>() == null)
            {
                var commandProvider = new JsonParameterCommandFactory(this.RepositoryProvider.DatabaseContext);
                mergeCommand = new JsonMerge<TEntity>(commandProvider, this.RepositoryProvider.DatabaseContext);
            }
            else
            {
                var commandProvider = new TableValuedParameterCommandFactory(this.RepositoryProvider.DatabaseContext);
                mergeCommand = new TableValuedMerge<TEntity>(commandProvider, this.RepositoryProvider.DatabaseContext);
            }

            mergeAction?.Invoke(mergeCommand);
            mergeCommand.Execute(items);
        }

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
        /// <returns>
        /// The <see cref="Task"/> that is executing the merge operation.
        /// </returns>
        public async Task MergeAsync<TItem>(IEnumerable<TItem> items, Action<TransactSqlMergeBase<TEntity>> mergeAction)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            TransactSqlMergeBase<TEntity> mergeCommand;

            if (typeof(TItem).GetCustomAttribute<TableTypeAttribute>() == null)
            {
                var commandProvider = new JsonParameterCommandFactory(this.RepositoryProvider.DatabaseContext);
                mergeCommand = new JsonMerge<TEntity>(commandProvider, this.RepositoryProvider.DatabaseContext);
            }
            else
            {
                var commandProvider = new TableValuedParameterCommandFactory(this.RepositoryProvider.DatabaseContext);
                mergeCommand = new TableValuedMerge<TEntity>(commandProvider, this.RepositoryProvider.DatabaseContext);
            }

            mergeAction?.Invoke(mergeCommand);
            await mergeCommand.ExecuteAsync(items).ConfigureAwait(false);
        }

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
                var commandProvider = new JsonParameterCommandFactory(this.RepositoryProvider.DatabaseContext);
                mergeCommand = new JsonMerge<TEntity>(commandProvider, this.RepositoryProvider.DatabaseContext);
            }
            else
            {
                var commandProvider = new TableValuedParameterCommandFactory(this.RepositoryProvider.DatabaseContext);
                mergeCommand = new TableValuedMerge<TEntity>(commandProvider, this.RepositoryProvider.DatabaseContext);
            }

            mergeAction?.Invoke(mergeCommand);
            var entities = mergeCommand.ExecuteForResults(items);
            return this.EntityMapper.Map<List<TItem>>(entities);
        }

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
        public async Task<IEnumerable<TItem>> MergeForResultsAsync<TItem>(
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
                var commandProvider = new JsonParameterCommandFactory(this.RepositoryProvider.DatabaseContext);
                mergeCommand = new JsonMerge<TEntity>(commandProvider, this.RepositoryProvider.DatabaseContext);
            }
            else
            {
                var commandProvider = new TableValuedParameterCommandFactory(this.RepositoryProvider.DatabaseContext);
                mergeCommand = new TableValuedMerge<TEntity>(commandProvider, this.RepositoryProvider.DatabaseContext);
            }

            mergeAction?.Invoke(mergeCommand);
            var entities = await mergeCommand.ExecuteForResultsAsync(items).ConfigureAwait(false);
            return this.EntityMapper.Map<List<TItem>>(entities);
        }
    }
}
