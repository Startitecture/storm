// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgreSqlRepository.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Defines the PostgreSqlRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.PostgreSql
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using JetBrains.Annotations;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Model;

    /// <summary>
    /// The postgre sql repository.
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
            this.Insert(items, null, insertAction);
        }

        /// <summary>
        /// Inserts a list of items into the repository.
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
        /// The type of item to insert..
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items"/> is null.
        /// </exception>
        public void Insert<TItem>([NotNull] IEnumerable<TItem> items, IDbTransaction transaction, Action<JsonInsert<TEntity>> insertAction)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var tableCommandProvider = new JsonCommandProvider(this.RepositoryProvider.DatabaseContext);
            var jsonInsert = new JsonInsert<TEntity>(tableCommandProvider, transaction);
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
        /// An <see cref="IEnumerable{T}"/> of <typeparamref name="TItem"/> items.
        /// </returns>
        public IEnumerable<TItem> InsertForResults<TItem>([NotNull] IEnumerable<TItem> items, Action<JsonInsert<TEntity>> insertAction)
        {
            return this.InsertForResults(items, null, insertAction);
        }

        /// <summary>
        /// Inserts a list of items into the repository.
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
        /// The type of item to insert..
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items"/> is null.
        /// </exception>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <typeparamref name="TItem"/> items.
        /// </returns>
        public IEnumerable<TItem> InsertForResults<TItem>([NotNull] IEnumerable<TItem> items, IDbTransaction transaction, Action<JsonInsert<TEntity>> insertAction)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var tableCommandProvider = new JsonCommandProvider(this.RepositoryProvider.DatabaseContext);
            var jsonInsert = new JsonInsert<TEntity>(tableCommandProvider, transaction);
            insertAction?.Invoke(jsonInsert);
            var insertedEntities = jsonInsert.ExecuteForResults(items);
            return this.EntityMapper.Map<List<TItem>>(insertedEntities);
        }
    }
}