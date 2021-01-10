// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityRepository.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface to an entity repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Startitecture.Orm.Model;

    /// <summary>
    /// Provides an interface to an entity repository.
    /// </summary>
    /// <typeparam name="TModel">
    /// The type of model managed by the repository.
    /// </typeparam>
    public interface IEntityRepository<TModel> : IReadOnlyRepository<TModel>
    {
        /// <summary>
        /// Saves a domain model to the database.
        /// </summary>
        /// <param name="model">
        /// The domain model to save.
        /// </param>
        /// <returns>
        /// The saved <typeparamref name="TModel"/> instance.
        /// </returns>
        TModel Save(TModel model);

        /// <summary>
        /// Saves a domain model to the database.
        /// </summary>
        /// <param name="model">
        /// The domain model to save.
        /// </param>
        /// <returns>
        /// The saved <typeparamref name="TModel"/> instance.
        /// </returns>
        Task<TModel> SaveAsync(TModel model);

        /// <summary>
        /// Updates a set of entities in the repository.
        /// </summary>
        /// <param name="updateSet">
        /// The update set that defines the entities and entity attributes to update.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of entity to update.
        /// </typeparam>
        /// <returns>
        /// The number of affected entities as an <see cref="int"/>.
        /// </returns>
        /// <remarks>
        /// The number of affected entities can include rows affected by triggers on the target table.
        /// </remarks>
        int Update<TItem>(UpdateSet<TItem> updateSet);

        /// <summary>
        /// Updates a set of entities in the repository.
        /// </summary>
        /// <param name="updateSet">
        /// The update set that defines the entities and entity attributes to update.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of entity to update.
        /// </typeparam>
        /// <returns>
        /// The number of affected entities as an <see cref="int"/>.
        /// </returns>
        /// <remarks>
        /// The number of affected entities can include rows affected by triggers on the target table.
        /// </remarks>
        Task<int> UpdateAsync<TItem>(UpdateSet<TItem> updateSet);

        /// <summary>
        /// Updates a selection of entities in the repository.
        /// </summary>
        /// <typeparam name="TKey">
        /// The type of the key that identifies the entity to update.
        /// </typeparam>
        /// <typeparam name="TItem">
        /// The type of entity in the repository.
        /// </typeparam>
        /// <param name="key">
        /// The key that identifies the entity.
        /// </param>
        /// <param name="source">
        /// The item that contains the update.
        /// </param>
        /// <param name="setExpressions">
        /// A optional set of expressions that explicitly select the columns to update. If empty, all non-key columns are updated.
        /// </param>
        void UpdateSingle<TKey, TItem>(TKey key, TItem source, params Expression<Func<TItem, object>>[] setExpressions);

        /// <summary>
        /// Updates a selection of entities in the repository.
        /// </summary>
        /// <typeparam name="TKey">
        /// The type of the key that identifies the entity to update.
        /// </typeparam>
        /// <typeparam name="TItem">
        /// The type of entity in the repository.
        /// </typeparam>
        /// <param name="key">
        /// The key that identifies the entity.
        /// </param>
        /// <param name="source">
        /// The item that contains the update.
        /// </param>
        /// <param name="setExpressions">
        /// A optional set of expressions that explicitly select the columns to update. If empty, all non-key columns are updated.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> that is performing the update.
        /// </returns>
        Task UpdateSingleAsync<TKey, TItem>(TKey key, TItem source, params Expression<Func<TItem, object>>[] setExpressions);

        /// <summary>
        /// Deletes a single item in the database.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item that can be mapped to a unique repository entity.
        /// </typeparam>
        /// <param name="example">
        /// The example item.
        /// </param>
        /// <returns>
        /// The number of items affected as an <see cref="int"/>.
        /// </returns>
        int Delete<TItem>(TItem example);

        /// <summary>
        /// Deletes a single item in the database.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item that can be mapped to a unique repository entity.
        /// </typeparam>
        /// <param name="example">
        /// The example item.
        /// </param>
        /// <returns>
        /// The number of items affected as an <see cref="int"/>.
        /// </returns>
        Task<int> DeleteAsync<TItem>(TItem example);

        /// <summary>
        /// Delete entities matching the set of <typeparamref name="TModel"/> models in <paramref name="defineSet"/>.
        /// </summary>
        /// <param name="defineSet">
        /// Define the set of entities to delete.
        /// </param>
        /// <returns>
        /// The number of items affected as an <see cref="int"/>.
        /// </returns>
        int DeleteEntities(Action<EntitySet<TModel>> defineSet);

        /// <summary>
        /// Delete entities matching the set of <typeparamref name="TModel"/> models in <paramref name="defineSet"/>.
        /// </summary>
        /// <param name="defineSet">
        /// Define the set of entities to delete.
        /// </param>
        /// <returns>
        /// The number of items affected as an <see cref="int"/>.
        /// </returns>
        Task<int> DeleteEntitiesAsync(Action<EntitySet<TModel>> defineSet);

        /// <summary>
        /// Deletes all entities matching the entity set.
        /// </summary>
        /// <param name="entitySet">
        /// The entity set to delete.
        /// </param>
        /// <returns>
        /// The number of items affected as an <see cref="int"/>.
        /// </returns>
        int DeleteSelection(IEntitySet entitySet);

        /// <summary>
        /// Deletes all entities matching the entity set.
        /// </summary>
        /// <param name="entitySet">
        /// The entity set to delete.
        /// </param>
        /// <returns>
        /// The number of items affected as an <see cref="int"/>.
        /// </returns>
        Task<int> DeleteSelectionAsync(IEntitySet entitySet);
    }
}