// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFormLayoutRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The FormLayoutRepository interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System.Collections.Generic;

    /// <summary>
    /// The FormLayoutRepository interface.
    /// </summary>
    public interface IFormLayoutRepository
    {
        /// <summary>
        /// Gets the form layouts for the specified form version.
        /// </summary>
        /// <param name="formVersion">
        /// The form version to get the layouts for.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="FormLayout"/> items for the specified <paramref name="formVersion"/>.
        /// </returns>
        IEnumerable<FormLayout> GetLayouts(FormVersion formVersion);

        /// <summary>
        /// Saves a layout in the repository.
        /// </summary>
        /// <param name="layout">
        /// The layout to save.
        /// </param>
        /// <returns>
        /// The saved <see cref="FormLayout"/>.
        /// </returns>
        FormLayout SaveLayout(FormLayout layout);

        /// <summary>
        /// Gets the <see cref="FormLayout"/> with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">
        /// The ID of the form layout to get.
        /// </param>
        /// <returns>
        /// The <see cref="FormLayout"/> with the associated <paramref name="id"/>, or null if the ID doesn't exist.
        /// </returns>
        FormLayout GetLayout(int id);
    }
}