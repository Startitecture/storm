// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILayoutSectionRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The LayoutSectionRepository interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The LayoutSectionRepository interface.
    /// </summary>
    public interface ILayoutSectionRepository
    {
        /// <summary>
        /// Gets the layout section with the specified ID along with its field placements.
        /// </summary>
        /// <param name="id">
        /// The ID of the layout section to get.
        /// </param>
        /// <returns>
        /// The <see cref="LayoutSection"/> associated with the <paramref name="id"/>, or null if no section is found.
        /// </returns>
        LayoutSection GetSection(int id);

        /// <summary>
        /// Gets the layout section with the specified identifier along with its field placements.
        /// </summary>
        /// <param name="identifier">
        /// The identifier of the layout section to get.
        /// </param>
        /// <returns>
        /// The <see cref="LayoutSection"/> associated with the <paramref name="identifier"/>, or null if no section is found.
        /// </returns>
        LayoutSection GetSection(Guid identifier);

        /// <summary>
        /// Selects all <see cref="LayoutSection"/> items in the repository.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="LayoutSection"/> items.
        /// </returns>
        IEnumerable<LayoutSection> SelectAll();

        /// <summary>
        /// Saves a <see cref="LayoutSection"/> to a repository without updating field placements.
        /// </summary>
        /// <param name="section">
        /// The layout section to save.
        /// </param>
        /// <returns>
        /// The saved <see cref="LayoutSection"/>.
        /// </returns>
        LayoutSection Save(LayoutSection section);

        /// <summary>
        /// Saves a <see cref="LayoutSection"/> to a repository along with field placements.
        /// </summary>
        /// <param name="section">
        /// The layout section to save.
        /// </param>
        /// <returns>
        /// The saved <see cref="LayoutSection"/>.
        /// </returns>
        LayoutSection SaveWithPlacements(LayoutSection section);
    }
}