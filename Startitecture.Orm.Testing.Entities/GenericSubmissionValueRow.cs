// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericSubmissionValueRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Entities
{
    /// <summary>
    /// The generic submission value row.
    /// </summary>
    public partial class GenericSubmissionValueRow
    {
        /// <summary>
        /// Gets or sets the generic submission.
        /// </summary>
        public GenericSubmissionRow GenericSubmission { get; set; } = new GenericSubmissionRow();

        /// <summary>
        /// Gets or sets the field.
        /// </summary>
        public FieldRow Field { get; set; } = new FieldRow();
    }
}