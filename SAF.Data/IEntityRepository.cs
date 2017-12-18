// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityRepository.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface to an entity repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data
{
    using SAF.Core;

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
        /// Saves an item to the database along with the children of the entity.
        /// </summary>
        /// <param name="item">
        /// The entity to save.
        /// </param>
        /// <returns>
        /// The saved <typeparamref name="TEntity"/> instance.
        /// </returns>
        TEntity SaveWithChildren(TEntity item);

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
        TEntity Save<TItem>(TItem item);

        /// <summary>
        /// Saves an entity to the database.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item to retrieve.
        /// </typeparam>
        /// <param name="entity">
        /// The entity to save.
        /// </param>
        /// <returns>
        /// The saved <typeparamref name="TItem"/> instance.
        /// </returns>
        TItem SaveAs<TItem>(TEntity entity);

        /// <summary>
        /// Deletes a selection of items.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item that contains the example properties.
        /// </typeparam>
        /// <param name="selection">
        /// The selection criteria.
        /// </param>
        /// <returns>
        /// The number of items affected as an <see cref="int"/>.
        /// </returns>
        int DeleteSelection<TItem>(IExampleQuery<TItem> selection);

        /// <summary>
        /// Deletes a single item.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item that contains the example properties.
        /// </typeparam>
        /// <param name="example">
        /// The example entity.
        /// </param>
        /// <returns>
        /// The number of items affected as an <see cref="int"/>.
        /// </returns>
        int Delete<TItem>(TItem example);

        /// <summary>
        /// Deletes a single item with its child items.
        /// </summary>
        /// <param name="entity">
        /// The entity to delete.
        /// </param>
        /// <returns>
        /// The number of non-child items affected as an <see cref="int"/>.
        /// </returns>
        int DeleteWithChildren(TEntity entity);
    }
}