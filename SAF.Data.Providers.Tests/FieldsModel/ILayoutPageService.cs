// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILayoutPageService.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The LayoutPageService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System.Collections.Generic;

    /// <summary>
    /// The LayoutPageService interface.
    /// </summary>
    public interface ILayoutPageService
    {
        /// <summary>
        /// Gets the page with the specified <paramref name="id"/>, along with its child objects.
        /// </summary>
        /// <param name="id">
        /// The ID of the page to get.
        /// </param>
        /// <returns>
        /// The <see cref="LayoutPage"/> with the specified <paramref name="id"/>, or null if no match is found.
        /// </returns>
        LayoutPage GetPage(int id);

        /// <summary>
        /// Gets the layout pages for the specified <paramref name="layout"/>.
        /// </summary>
        /// <param name="layout">
        /// The form layout to get the layout pages for.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="LayoutPage"/> items for the specified <paramref name="layout"/>.
        /// </returns>
        IEnumerable<LayoutPage> GetPages(FormLayout layout);

        /// <summary>
        /// Adds a page to the <paramref name="layout"/>.
        /// </summary>
        /// <param name="layout">
        /// The layout to add the page to.
        /// </param>
        /// <param name="name">
        /// The name of the page.
        /// </param>
        /// <returns>
        /// The newly added <see cref="LayoutPage"/>.
        /// </returns>
        LayoutPage AddPage(FormLayout layout, string name);

        /// <summary>
        /// Saves a <see cref="LayoutPage"/> to the repository.
        /// </summary>
        /// <param name="page">
        /// The layout page to save.
        /// </param>
        /// <returns>
        /// The saved <see cref="LayoutPage"/>.
        /// </returns>
        LayoutPage SavePage(LayoutPage page);
    }
}