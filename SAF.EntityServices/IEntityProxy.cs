// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityProxy.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.EntityServices
{
    using System.Collections.Generic;

    using SAF.ActionTracking;
    using SAF.Core;
    using SAF.Data;

    /// <summary>
    /// The EntityServiceProxy interface.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of entity managed by the service.
    /// </typeparam>
    public interface IEntityProxy<TEntity> : IActionEventProxy
        where TEntity : IValidatingEntity
    {
        /// <summary>
        /// Gets the entity mapper for the current entity proxy.
        /// </summary>
        IEntityMapper EntityMapper { get; }

        /// <summary>
        /// Determines whether the specified item is contained in the repository.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item that contains the properties of the entity.
        /// </typeparam>
        /// <param name="item">
        /// The item containing the properties to search for.
        /// </param>
        /// <returns>
        /// <c>true</c> if the item exists in the repository; otherwise, <c>false</c>.
        /// </returns>
        bool Contains<TItem>(TItem item);

        /// <summary>
        /// Selects a single item from the repository.
        /// </summary>
        /// <param name="item">
        /// An item that contains the unique ID or key of the item to retrieve.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that contains unique properties of the <typeparamref name="TEntity"/> type.
        /// </typeparam>
        /// <returns>
        /// A <see cref="TItem"/> that matches the unique properties of <paramref name="item"/>, or null if no match is found.
        /// </returns>
        TItem SelectItem<TItem>(TItem item);

        /// <summary>
        /// Selects a single item from the repository.
        /// </summary>
        /// <param name="item">
        /// An item that contains the unique ID or key of the item to retrieve.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that contains unique properties of the <typeparamref name="TEntity"/> type.
        /// </typeparam>
        /// <returns>
        /// The <see cref="TEntity"/> that matches the unique properties of <paramref name="item"/>, or null if no match is found.
        /// </returns>
        TEntity SelectEntity<TItem>(TItem item);

        /// <summary>
        /// Selects items matching the query from the repository.
        /// </summary>
        /// <param name="query">
        /// The query to execute.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that contains unique properties of the <typeparamref name="TEntity"/> type.
        /// </typeparam>
        /// <returns>
        /// A collection of items matching the query.
        /// </returns>
        IEnumerable<TItem> SelectItems<TItem>(IExampleQuery<TItem> query);

        /// <summary>
        /// Selects a page of items matching the query from the repository.
        /// </summary>
        /// <param name="query">
        /// The query to execute.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that contains unique properties of the <typeparamref name="TEntity"/> type.
        /// </typeparam>
        /// <returns>
        /// A page of items matching the query.
        /// </returns>
        Page<TItem> SelectItemPage<TItem>(IExampleQuery<TItem> query);

        /// <summary>
        /// Selects items matching the query from the repository.
        /// </summary>
        /// <param name="query">
        /// The query to execute.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that contains unique properties of the <typeparamref name="TEntity"/> type.
        /// </typeparam>
        /// <returns>
        /// A collection of items matching the query.
        /// </returns>
        IEnumerable<TEntity> SelectEntities<TItem>(IExampleQuery<TItem> query);

        /// <summary>
        /// Selects a page of items matching the query from the repository.
        /// </summary>
        /// <param name="query">
        /// The query to execute.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that contains unique properties of the <typeparamref name="TEntity"/> type.
        /// </typeparam>
        /// <returns>
        /// A page of items matching the query.
        /// </returns>
        Page<TEntity> SelectEntityPage<TItem>(IExampleQuery<TItem> query);

        /// <summary>
        /// Saves an item into the repository and returns the saved item.
        /// </summary>
        /// <param name="item">
        /// The item to save.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that represents the <typeparamref name="TEntity"/> to save.
        /// </typeparam>
        /// <returns>
        /// A <typeparamref name="TItem"/> that represents the saved <typeparamref name="TEntity"/> item.
        /// </returns>
        TItem SaveItem<TItem>(TItem item);

        /// <summary>
        /// Saves an item into the repository and returns the repository entity.
        /// </summary>
        /// <param name="item">
        /// The item to save.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that represents the <typeparamref name="TEntity"/> to save.
        /// </typeparam>
        /// <returns>
        /// The saved <typeparamref name="TEntity"/>.
        /// </returns>
        TEntity SaveEntity<TItem>(TItem item);

        /// <summary>
        /// Removes a selection of items from the repository.
        /// </summary>
        /// <param name="item">
        /// The item to remove.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that represents the <typeparamref name="TEntity"/> remove.
        /// </typeparam>
        /// <returns>
        /// <c>true</c> if the item was removed; otherwise, <c>false</c>.
        /// </returns>
        bool RemoveItem<TItem>(TItem item);

        /// <summary>
        /// Removes a selection of items from the repository.
        /// </summary>
        /// <param name="query">
        /// A query that defines the selection to remove.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that represents the <typeparamref name="TEntity"/> remove.
        /// </typeparam>
        /// <returns>
        /// The number of items affected by the removal.
        /// </returns>
        int RemoveSelection<TItem>(IExampleQuery<TItem> query);
    }
}