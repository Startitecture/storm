// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDocumentVersionService.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using System.Collections.Generic;

    using JetBrains.Annotations;

    /// <summary>
    /// The DocumentVersionService interface.
    /// </summary>
    public interface IDocumentVersionService
    {
        /// <summary>
        /// Gets all the versions for the specified document.
        /// </summary>
        /// <param name="document">
        /// The document to get the versions for.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="DocumentVersion"/> items associated with the specified
        /// <paramref name="document"/>.
        /// </returns>
        IEnumerable<DocumentVersion> GetAllVersions([NotNull] Document document);
    }
}