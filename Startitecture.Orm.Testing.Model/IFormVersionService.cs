// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFormVersionService.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The FormVersionService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// The FormVersionService interface.
    /// </summary>
    public interface IFormVersionService
    {
        /// <summary>
        /// Gets the form versions for the specified <paramref name="form"/>.
        /// </summary>
        /// <param name="form">
        /// The form to load the versions for.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="FormVersion"/> items for the specified <paramref name="form"/>.
        /// </returns>
        IEnumerable<FormVersion> GetVersions(Form form);

        /// <summary>
        /// Saves the form version in a repository.
        /// </summary>
        /// <param name="version">
        /// The form version to save.
        /// </param>
        /// <returns>
        /// The saved <see cref="FormVersion"/>.
        /// </returns>
        FormVersion SaveVersion(FormVersion version);
    }
}