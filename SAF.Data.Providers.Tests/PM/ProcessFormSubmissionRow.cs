// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessFormSubmissionRow.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.PM
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The process form submission row.
    /// </summary>
    public partial class ProcessFormSubmissionRow : ICompositeEntity
    {
        /// <summary>
        /// The process form submission entity relations.
        /// </summary>
        private static readonly IEnumerable<IEntityRelation> ProcessFormSubmissionEntityRelations =
            new TransactSqlFromClause<ProcessFormSubmissionRow>().InnerJoin<FormSubmissionRow>(
                row => row.ProcessFormSubmissionId,
                row => row.FormSubmissionId).Relations;

        /// <summary>
        /// Gets or sets the FormSubmissionId.
        /// </summary>
        [RelatedEntity(typeof(FormSubmissionRow))]
        public long FormSubmissionId { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        [RelatedEntity(typeof(FormSubmissionRow))]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the SubmittedByPersonId.
        /// </summary>
        [RelatedEntity(typeof(FormSubmissionRow))]
        public int SubmittedByPersonId { get; set; }

        /// <summary>
        /// Gets or sets the SubmittedTime.
        /// </summary>
        [RelatedEntity(typeof(FormSubmissionRow))]
        public DateTimeOffset SubmittedTime { get; set; }

        /// <summary>
        /// Gets the entity relations.
        /// </summary>
        public IEnumerable<IEntityRelation> EntityRelations
        {
            get
            {
                return ProcessFormSubmissionEntityRelations;
            }
        }
    }
}