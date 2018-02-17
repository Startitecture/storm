// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormSubmissionRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using System.Collections.Generic;

    using SAF.Data.Providers.Tests.PM;

    using Startitecture.Orm.Model;
    using Startitecture.Orm.Query;

    /// <summary>
    /// The form submission row.
    /// </summary>
    public partial class FormSubmissionRow : ICompositeEntity
    {
        /// <summary>
        /// The process form entity relations.
        /// </summary>
        private static readonly IEnumerable<IEntityRelation> ProcessFormEntityRelations =
            new TransactSqlFromClause<FormSubmissionRow>().LeftJoin<ProcessFormSubmissionRow>(
                    row => row.FormSubmissionId,
                    row => row.ProcessFormSubmissionId)
                .LeftJoin<UnifiedFormSubmissionRow>(row => row.FormSubmissionId, row => row.UnifiedFormSubmissionId)
                .Relations;

        /// <summary>
        /// Gets or sets the process form ID.
        /// </summary>
        [RelatedEntity(typeof(ProcessFormSubmissionRow))]
        public int ProcessFormId { get; set; }

        /////// <summary>
        /////// Gets or sets the form version ID.
        /////// </summary>
        ////[ResultColumn]
        ////[RelatedEntity(typeof(UnifiedFormSubmissionRow))]
        ////public int FormVersionId { get; set; }

        /// <summary>
        /// Gets the entity relations.
        /// </summary>
        public IEnumerable<IEntityRelation> EntityRelations
        {
            get
            {
                return ProcessFormEntityRelations;
            }
        }
    }
}