// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentDto.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model.Contracts
{
    /// <summary>
    /// The document DTO.
    /// </summary>
    public class DocumentDto
    {
        /// <summary>
        /// Gets or sets the document ID.
        /// </summary>
        public int DocumentId { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the version number.
        /// </summary>
        public int VersionNumber { get; set; }
    }
}