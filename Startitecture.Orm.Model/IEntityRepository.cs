// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface to an entity repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
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
        /// Deletes all entities matching the selection.
        /// </summary>
        /// <param name="selection">
        /// The selection to delete.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that contains properties that can be mapped to an item selection for the repository entities.
        /// </typeparam>
        /// <returns>
        /// The number of items affected as an <see cref="int"/>.
        /// </returns>
        int Delete<TItem>(EntitySelection<TItem> selection);
    }
}