// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFormVersionRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The FormVersionRepository interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// The FormVersionRepository interface.
    /// </summary>
    public interface IFormVersionRepository
    {
        /// <summary>
        /// Gets the versions for the specified form.
        /// </summary>
        /// <param name="form">
        /// The form to get the versions for.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="FormVersion"/> items.
        /// </returns>
        IEnumerable<FormVersion> GetVersions(Form form);

        /// <summary>
        /// Saves a form version to the repository.
        /// </summary>
        /// <param name="formVersion">
        /// The form version to save.
        /// </param>
        /// <returns>
        /// The saved <see cref="FormVersion"/>.
        /// </returns>
        FormVersion Save(FormVersion formVersion);

        /// <summary>
        /// Gets the form version with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">
        /// The ID of the form version to get.
        /// </param>
        /// <returns>
        /// The <see cref="FormVersion"/> with the specified <paramref name="id"/>, or null if no version is found.
        /// </returns>
        FormVersion GetVersion(int id);
    }
}