// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgreSqlRepository.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   An IEntityRepository implementation for PostgreSQL.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.PostgreSql
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Model;

    /// <summary>
    /// An <see cref="IEntityRepository{TModel}"/> implementation for PostgreSQL.
    /// </summary>
    /// <typeparam name="TModel">
    /// The type of domain model represented by the repository.
    /// </typeparam>
    /// <typeparam name="TEntity">
    /// The type of entity stored in the repository.
    /// </typeparam>
    public class PostgreSqlRepository<TModel, TEntity> : EntityRepository<TModel, TEntity>, IPostgreSqlRepository<TModel, TEntity>
        where TEntity : class, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlRepository{TModel,TEntity}"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        /// <param name="entityMapper">
        /// The entity mapper.
        /// </param>
        public PostgreSqlRepository(IRepositoryProvider repositoryProvider, IEntityMapper entityMapper)
            : base(repositoryProvider, entityMapper)
        {
        }

        /// <summary>
        /// Inserts a list of items into the repository.
        /// </summary>
        /// <param name="items">
        /// The items to insert.
        /// </param>
        /// <param name="insertAction">
        /// The insert action to take, or null to take the default insert action.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item to insert..
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items"/> is null.
        /// </exception>
        public void Insert<TItem>(IEnumerable<TItem> items, Action<JsonInsert<TEntity>> insertAction)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var jsonInsert = new JsonInsert<TEntity>(this.RepositoryProvider.DatabaseContext);
            insertAction?.Invoke(jsonInsert);
            jsonInsert.Execute(items);
        }

        /// <inheritdoc />
        public async Task InsertAsync<TItem>(IEnumerable<TItem> items, Action<JsonInsert<TEntity>> insertAction)
        {
            await this.InsertAsync(items, insertAction, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task InsertAsync<TItem>(IEnumerable<TItem> items, Action<JsonInsert<TEntity>> insertAction, CancellationToken cancellationToken)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var jsonInsert = new JsonInsert<TEntity>(this.RepositoryProvider.DatabaseContext);
            insertAction?.Invoke(jsonInsert);
            await jsonInsert.ExecuteAsync(items, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Inserts a list of items into the repository.
        /// </summary>
        /// <param name="items">
        /// The items to insert.
        /// </param>
        /// <param name="insertAction">
        /// The insert action to take, or null to take the default insert action.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item to insert..
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items"/> is null.
        /// </exception>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <typeparamref name="TItem"/> items.
        /// </returns>
        public IEnumerable<TItem> InsertForResults<TItem>(
            IEnumerable<TItem> items,
            Action<JsonInsert<TEntity>> insertAction)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var jsonInsert = new JsonInsert<TEntity>(this.RepositoryProvider.DatabaseContext);
            insertAction?.Invoke(jsonInsert);
            var insertedEntities = jsonInsert.ExecuteForResults(items);
            return this.EntityMapper.Map<List<TItem>>(insertedEntities);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<TItem> InsertForResultsAsync<TItem>(IEnumerable<TItem> items, Action<JsonInsert<TEntity>> insertAction)
        {
            await foreach (var item in this.InsertForResultsAsync(items, insertAction, CancellationToken.None).ConfigureAwait(false))
            {
                yield return item;
            }
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<TItem> InsertForResultsAsync<TItem>(
            IEnumerable<TItem> items,
            Action<JsonInsert<TEntity>> insertAction,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var jsonInsert = new JsonInsert<TEntity>(this.RepositoryProvider.DatabaseContext);
            insertAction?.Invoke(jsonInsert);

            await foreach (var item in jsonInsert.ExecuteForResultsAsync(items, cancellationToken).ConfigureAwait(false))
            {
                yield return this.EntityMapper.Map<TItem>(item);
            }
        }
    }
}