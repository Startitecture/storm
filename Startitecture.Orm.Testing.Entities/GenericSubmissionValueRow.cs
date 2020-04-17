﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericSubmissionValueRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Entities
{
    using Startitecture.Orm.Model;

    /// <summary>
    /// The generic submission value row.
    /// </summary>
    public partial class GenericSubmissionValueRow
    {
        /// <summary>
        /// Gets or sets the field.
        /// </summary>
        [Relation]
        public FieldValueRow FieldValue { get; set; } = new FieldValueRow();
    }
}