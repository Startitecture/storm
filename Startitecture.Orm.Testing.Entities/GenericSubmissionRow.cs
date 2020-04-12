// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericSubmissionRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Entities
{
    using Startitecture.Orm.Model;

    /// <summary>
    /// The generic submission row.
    /// </summary>
    public partial class GenericSubmissionRow
    {
        /// <summary>
        /// Gets or sets the submitted by.
        /// </summary>
        [Relation]
        public DomainIdentityRow SubmittedBy { get; set; }
    }
}