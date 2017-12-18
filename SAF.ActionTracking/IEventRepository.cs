// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEventRepository.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.ActionTracking
{
    using System;

    /// <summary>
    /// An interface for event repositories.
    /// </summary>
    public interface IEventRepository
    {
        /// <summary>
        /// Gets or sets the connection string. 
        /// </summary>
        string Connection { get; set; }

        /// <summary>
        /// Saves an item to the database.
        /// </summary>
        /// <param name="item">
        /// The entity to save.
        /// </param>
        /// <returns>
        /// Returns the <see cref="ActionEvent"/> specified in <paramref name="item"/>.
        /// </returns>
        ActionEvent Save(ActionEvent item);

        /// <summary>
        /// Gets the ID for the specified action identifier.
        /// </summary>
        /// <param name="actionIdentifier">
        /// The action identifier.
        /// </param>
        /// <returns>
        /// The action, or null if no ID is found.
        /// </returns>
        ActionEvent GetById(Guid actionIdentifier);
    }
}
