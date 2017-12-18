// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILayoutPageSectionRepository.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The LayoutPageSectionRepository interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System.Collections.Generic;

    /// <summary>
    /// The LayoutPageSectionRepository interface.
    /// </summary>
    public interface ILayoutPageSectionRepository
    {
        /// <summary>
        /// Gets the <see cref="LayoutPageSection"/> items for the specified <paramref name="page"/>.
        /// </summary>
        /// <param name="page">
        /// The layout to get the layout page sections for.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="LayoutPageSection"/> items for the specified <paramref name="page"/>.
        /// </returns>
        IEnumerable<LayoutPageSection> GetSections(LayoutPage page);

        /// <summary>
        /// Gets the <see cref="LayoutPageSection"/> items for the specified <paramref name="layout"/>.
        /// </summary>
        /// <param name="layout">
        /// The layout to get the layout page sections for.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="LayoutPageSection"/> items for the specified <paramref name="layout"/>
        /// .
        /// </returns>
        IEnumerable<LayoutPageSection> GetSections(FormLayout layout);

        /// <summary>
        /// Gets the <see cref="LayoutPageSection"/> with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">
        /// The ID of the layout page section to get.
        /// </param>
        /// <returns>
        /// The <see cref="LayoutPageSection"/> with the specified <paramref name="id"/>, or null if no section is found.
        /// </returns>
        LayoutPageSection GetSection(int id);
    }
}