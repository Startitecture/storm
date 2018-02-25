// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILayoutPageRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The LayoutPageRepository interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// The LayoutPageRepository interface.
    /// </summary>
    public interface ILayoutPageRepository
    {
        /// <summary>
        /// Gets the <see cref="LayoutPage"/> associated with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">
        /// The ID of the <see cref="LayoutPage"/> to get.
        /// </param>
        /// <returns>
        /// The <see cref="LayoutPage"/> associated with the <paramref name="id"/>, or null if no page is found.
        /// </returns>
        LayoutPage GetPage(int id);

        /// <summary>
        /// Gets the layout pages for the specified form layout.
        /// </summary>
        /// <param name="layout">
        /// The form layout to get the pages for.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="LayoutPage"/> items associated with the <paramref name="layout"/>.
        /// </returns>
        IEnumerable<LayoutPage> GetPages(FormLayout layout);

        /// <summary>
        /// Saves a <see cref="LayoutPage"/> to the repository.
        /// </summary>
        /// <param name="page">
        /// The layout page to save.
        /// </param>
        /// <returns>
        /// The saved <see cref="LayoutPage"/>.
        /// </returns>
        LayoutPage Save(LayoutPage page);
    }
}