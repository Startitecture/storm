﻿// --------------------------------------------------------------------------------------------------------------------
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
    using System.Threading.Tasks;

    using JetBrains.Annotations;

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
    public class PostgreSqlRepository<TModel, TEntity> : EntityRepository<TModel, TEntity>
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
        /// Initializes a new instance of the <see cref="PostgreSqlRepository{TModel,TEntity}"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        /// <param name="entityMapper">
        /// The entity mapper.
        /// </param>
        /// <param name="selectionComparer">
        /// The selection comparer.
        /// </param>
        public PostgreSqlRepository(IRepositoryProvider repositoryProvider, IEntityMapper entityMapper, IComparer<TEntity> selectionComparer)
            : base(repositoryProvider, entityMapper, selectionComparer)
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
        public void Insert<TItem>([NotNull] IEnumerable<TItem> items, Action<JsonInsert<TEntity>> insertAction)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var tableCommandProvider = new JsonCommandFactory(this.RepositoryProvider.DatabaseContext);
            var jsonInsert = new JsonInsert<TEntity>(tableCommandProvider, this.RepositoryProvider.DatabaseContext);
            insertAction?.Invoke(jsonInsert);
            jsonInsert.Execute(items);
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
        /// The <see cref="Task"/> that is performing the insert.
        /// </returns>
        public async Task InsertAsync<TItem>([NotNull] IEnumerable<TItem> items, Action<JsonInsert<TEntity>> insertAction)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var tableCommandProvider = new JsonCommandFactory(this.RepositoryProvider.DatabaseContext);
            var jsonInsert = new JsonInsert<TEntity>(tableCommandProvider, this.RepositoryProvider.DatabaseContext);
            insertAction?.Invoke(jsonInsert);
            await jsonInsert.ExecuteAsync(items).ConfigureAwait(false);
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
            [NotNull] IEnumerable<TItem> items,
            Action<JsonInsert<TEntity>> insertAction)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var tableCommandProvider = new JsonCommandFactory(this.RepositoryProvider.DatabaseContext);
            var jsonInsert = new JsonInsert<TEntity>(tableCommandProvider, this.RepositoryProvider.DatabaseContext);
            insertAction?.Invoke(jsonInsert);
            var insertedEntities = jsonInsert.ExecuteForResults(items);
            return this.EntityMapper.Map<List<TItem>>(insertedEntities);
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
        public async Task<IEnumerable<TItem>> InsertForResultsAsync<TItem>(
            [NotNull] IEnumerable<TItem> items,
            Action<JsonInsert<TEntity>> insertAction)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var tableCommandProvider = new JsonCommandFactory(this.RepositoryProvider.DatabaseContext);
            var jsonInsert = new JsonInsert<TEntity>(tableCommandProvider, this.RepositoryProvider.DatabaseContext);
            insertAction?.Invoke(jsonInsert);
            var insertedEntities = await jsonInsert.ExecuteForResultsAsync(items).ConfigureAwait(false);
            return this.EntityMapper.Map<List<TItem>>(insertedEntities);
        }
    }
}