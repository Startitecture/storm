// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFormLayoutService.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The FormLayoutService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// The FormLayoutService interface.
    /// </summary>
    public interface IFormLayoutService
    {
        /// <summary>
        /// Gets the form layout with child objects loaded.
        /// </summary>
        /// <param name="id">
        /// The ID of the layout.
        /// </param>
        /// <returns>
        /// The <see cref="FormLayout"/> associated with the specified <paramref name="id"/>, or null if no match is found.
        /// </returns>
        FormLayout GetLayout(int id);

        /// <summary>
        /// Gets the layouts for the specified form version. Child objects are not loaded.
        /// </summary>
        /// <param name="version">
        /// The form version to get the layouts for.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="FormLayout"/> items for the specified <paramref name="version"/>.
        /// </returns>
        IEnumerable<FormLayout> GetLayouts(FormVersion version);

        /// <summary>
        /// Adds a layout to the specified form version.
        /// </summary>
        /// <param name="version">
        /// The version to add the layout to.
        /// </param>
        /// <param name="name">
        /// The name of the layout.
        /// </param>
        /// <returns>
        /// The newly created <see cref="FormLayout"/>.
        /// </returns>
        FormLayout AddLayout(FormVersion version, string name);

        /// <summary>
        /// Saves a form layout into the repository.
        /// </summary>
        /// <param name="layout">
        /// The form layout to save.
        /// </param>
        /// <returns>
        /// The saved <see cref="FormLayout"/>.
        /// </returns>
        FormLayout SaveLayout(FormLayout layout);
    }
}