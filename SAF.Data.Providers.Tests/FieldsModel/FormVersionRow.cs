// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormVersionRow.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The form version row.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The form version row.
    /// </summary>
    public partial class FormVersionRow : ICompositeEntity
    {
        /// <summary>
        /// The form version relations.
        /// </summary>
        private static readonly Lazy<IEnumerable<IEntityRelation>> FormVersionRelations =
            new Lazy<IEnumerable<IEntityRelation>>(
                () =>
                    new TransactSqlFromClause<FormVersionRow>().InnerJoin(row => row.FormId, row => row.Form.FormId)
                        .InnerJoin(row => row.CreatedByPersonId, row => row.CreatedBy.PersonId)
                        .InnerJoin(row => row.LastModifiedByPersonId, row => row.LastModifiedBy.PersonId)
                        .Relations);

        /// <summary>
        /// Gets or sets the form.
        /// </summary>
        [Relation]
        public FormRow Form { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        [Relation]
        public PersonRow CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the last modified by.
        /// </summary>
        [Relation]
        public PersonRow LastModifiedBy { get; set; }

        /// <summary>Gets the entity relations.</summary>
        public IEnumerable<IEntityRelation> EntityRelations
        {
            get
            {
                return FormVersionRelations.Value;
            }
        }
    }
}
