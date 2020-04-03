// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface to an entity repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
{
    using Startitecture.Orm.Query;

    /// <summary>
    /// Provides an interface to an entity repository.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of entity stored in the repository.
    /// </typeparam>
    public interface IEntityRepository<TEntity> : IReadOnlyRepository<TEntity>
    {
        /// <summary>
        /// Saves an item to the database.
        /// </summary>
        /// <param name="item">
        /// The entity to save.
        /// </param>
        /// <returns>
        /// The saved <typeparamref name="TEntity"/> instance.
        /// </returns>
        TEntity Save(TEntity item);

        /// <summary>
        /// Saves an item to the database.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item to save.
        /// </typeparam>
        /// <param name="item">
        /// The entity to save.
        /// </param>
        /// <returns>
        /// The saved <typeparamref name="TEntity"/> instance.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="item"/> is null.
        /// </exception>
        TEntity Save<TItem>(TItem item);

        /// <summary>
        /// Deletes a single entity.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item that can be mapped to a unique repository entity.
        /// </typeparam>
        /// <param name="example">
        /// The example entity.
        /// </param>
        /// <returns>
        /// The number of items affected as an <see cref="int"/>.
        /// </returns>
        int Delete<TItem>(TItem example);

        /// <summary>
        /// Deletes all entities matching the selection.
        /// </summary>
        /// <param name="selection">
        /// The selection.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that contains properties that can be mapped to an item selection for the repository entities.
        /// </typeparam>
        /// <returns>
        /// The number of items affected as an <see cref="int"/>.
        /// </returns>
        int Delete<TItem>(ItemSelection<TItem> selection);
    }
}