// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormSubmissionValueRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model.DocumentEntities
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Orm.Model;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Sql;

    /// <summary>
    /// The form submission value row.
    /// </summary>
    [TableType("dbo.FormSubmissionValueTableType")]
    public partial class FormSubmissionValueRow : ICompositeEntity
    {
        /// <summary>
        /// The form submission value relations.
        /// </summary>
        private static readonly IEnumerable<IEntityRelation> FormSubmissionValueRelations =
            new SqlFromClause<FormSubmissionValueRow>().InnerJoin(
                row => row.FormSubmissionValueId,
                row => row.UnifiedFieldValueId).InnerJoin<UnifiedFieldRow>(row => row.UnifiedFieldId, row => row.UnifiedFieldId).Relations;

        /// <summary>
        /// Gets or sets the UnifiedFieldTypeId.
        /// </summary>
        [RelatedEntity(typeof(UnifiedFieldRow))]
        public int UnifiedFieldTypeId { get; set; }

        /// <summary>
        /// Gets or sets the UnifiedValueTypeId.
        /// </summary>
        [RelatedEntity(typeof(UnifiedFieldRow))]
        public int UnifiedValueTypeId { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        [RelatedEntity(typeof(UnifiedFieldRow), true)]
        public string UnifiedFieldName { get; set; }

        /// <summary>
        /// Gets or sets the Caption.
        /// </summary>
        [RelatedEntity(typeof(UnifiedFieldRow), true)]
        public string UnifiedFieldCaption { get; set; }

        /// <summary>
        /// Gets or sets the Label.
        /// </summary>
        [RelatedEntity(typeof(UnifiedFieldRow), true)]
        public string UnifiedFieldLabel { get; set; }

        /// <summary>
        /// Gets or sets the UnifiedFieldValueId.
        /// </summary>
        [RelatedEntity(typeof(FieldEntities.UnifiedFieldValueRow))]
        public long UnifiedFieldValueId { get; set; }

        /// <summary>
        /// Gets or sets the UnifiedFieldId.
        /// </summary>
        [RelatedEntity(typeof(FieldEntities.UnifiedFieldValueRow))]
        public int UnifiedFieldId { get; set; }

        /// <summary>
        /// Gets or sets the LastModifiedPersonId.
        /// </summary>
        [RelatedEntity(typeof(FieldEntities.UnifiedFieldValueRow))]
        public int LastModifiedPersonId { get; set; }

        /// <summary>
        /// Gets or sets the LastModifiedTime.
        /// </summary>
        [RelatedEntity(typeof(FieldEntities.UnifiedFieldValueRow))]
        public DateTimeOffset LastModifiedTime { get; set; }

        /// <summary>
        /// Gets the entity relations.
        /// </summary>
        public virtual IEnumerable<IEntityRelation> EntityRelations
        {
            get
            {
                return FormSubmissionValueRelations;
            }
        }
    }
}