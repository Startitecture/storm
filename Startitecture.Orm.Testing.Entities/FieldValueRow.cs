// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldValueRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Entities
{
    using Startitecture.Orm.Model;

    /// <summary>
    /// The field value row.
    /// </summary>
    public partial class FieldValueRow
    {
        /// <summary>
        /// Gets or sets the field.
        /// </summary>
        [Relation]
        public FieldRow Field { get; set; }

        /// <summary>
        /// Gets or sets the last modified by domain identity.
        /// </summary>
        [Relation]
        public DomainIdentityRow LastModifiedBy { get; set; }
    }
}