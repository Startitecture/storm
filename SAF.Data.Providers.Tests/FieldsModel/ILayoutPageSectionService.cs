// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILayoutPageSectionService.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The LayoutPageSectionService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System.Collections.Generic;

    /// <summary>
    /// The LayoutPageSectionService interface.
    /// </summary>
    public interface ILayoutPageSectionService
    {
        /// <summary>
        /// Gets the page sections for the specified <paramref name="page"/>.
        /// </summary>
        /// <param name="page">
        /// The layout page to get the page sections for.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="LayoutPageSection"/> items for the specified <paramref name="page"/>.
        /// </returns>
        IEnumerable<LayoutPageSection> GetSections(LayoutPage page);

        /// <summary>
        /// Gets the page sections for the specified <paramref name="layout"/>.
        /// </summary>
        /// <param name="layout">
        /// The form layout to get the page sections for.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="LayoutPageSection"/> items for the specified <paramref name="layout"/>.
        /// </returns>
        IEnumerable<LayoutPageSection> GetSections(FormLayout layout);

        /// <summary>
        /// Gets the <see cref="LayoutPageSection"/> with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">
        /// The ID of the section to get.
        /// </param>
        /// <returns>
        /// The <see cref="LayoutPageSection"/> associated with the specified <paramref name="id"/>, or null if no section is
        /// found.
        /// </returns>
        LayoutPageSection GetSection(int id);
    }
}