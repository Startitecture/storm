// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILayoutSectionService.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The LayoutSectionService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    /// <summary>
    /// The LayoutSectionService interface.
    /// </summary>
    public interface ILayoutSectionService
    {
        /// <summary>
        ///     Gets all the layout sections in the repository.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> of <see cref="LayoutSection" /> items.
        /// </returns>
        IEnumerable<LayoutSection> SelectAllSections();

        /// <summary>
        /// Gets the <see cref="LayoutSection"/> associated with the specified <paramref name="id"/>;
        /// </summary>
        /// <param name="id">
        /// The ID of the layout section to get.
        /// </param>
        /// <returns>
        /// The <see cref="LayoutSection"/> with the associated <paramref name="id"/>, or null if no section is found.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="id"/> is less than one.
        /// </exception>
        LayoutSection GetSection(int id);

        /// <summary>
        /// Gets the <see cref="LayoutSection"/> associated with the specified <paramref name="identifier"/>;
        /// </summary>
        /// <param name="identifier">
        /// The ID of the layout section to get.
        /// </param>
        /// <returns>
        /// The <see cref="LayoutSection"/> with the associated <paramref name="identifier"/>, or null if no section is
        ///     found.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="identifier"/> is less than one.
        /// </exception>
        LayoutSection GetSection(Guid identifier);

        /// <summary>
        /// Saves the <paramref name="section"/> to a repository.
        /// </summary>
        /// <param name="section">
        /// The section to save.
        /// </param>
        /// <returns>
        /// The saved <see cref="LayoutSection"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="section"/> is null.
        /// </exception>
        LayoutSection SaveSection([NotNull] LayoutSection section);
    }
}