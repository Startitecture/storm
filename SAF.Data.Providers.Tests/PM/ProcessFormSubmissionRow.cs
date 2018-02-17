// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessFormSubmissionRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.PM
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Orm.Model;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Sql;

    /// <summary>
    /// The process form submission row.
    /// </summary>
    public partial class ProcessFormSubmissionRow : ICompositeEntity
    {
        /// <summary>
        /// The process form submission entity relations.
        /// </summary>
        private static readonly IEnumerable<IEntityRelation> ProcessFormSubmissionEntityRelations =
            new SqlFromClause<ProcessFormSubmissionRow>().InnerJoin<FormSubmissionRow>(
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