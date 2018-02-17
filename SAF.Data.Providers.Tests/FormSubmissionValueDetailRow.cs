// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormSubmissionValueDetailRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Orm.Model;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Sql;

    /// <summary>
    /// The form submission value detail row.
    /// </summary>
    [TableName("[dbo].[FormSubmissionValue]")]
    [TableType("dbo.FormSubmissionValueDetailTableType")]
    public class FormSubmissionValueDetailRow : FormSubmissionValueRow
    {
        /// <summary>
        /// The form submission value relations.
        /// </summary>
        private static readonly IEnumerable<IEntityRelation> FormSubmissionValueDetailRelations =
            new SqlFromClause<FormSubmissionValueRow>()
                .InnerJoin(row => row.FormSubmissionValueId, row => row.UnifiedFieldValueId)
                .InnerJoin<UnifiedFieldValueRow, UnifiedFieldRow>(row => row.UnifiedFieldId, row => row.UnifiedFieldId)
                
                ////.LeftJoin<UnifiedFieldValueRow, UnifiedFieldAttachmentRow>(row => row.UnifiedFieldValueId, row => row.UnifiedFieldValueId)
                ////.LeftJoin<UnifiedFieldAttachmentRow, AttachmentRow>(row => row.UnifiedFieldAttachmentId, row => row.AttachmentId)
                ////.LeftJoin<AttachmentRow, AttachmentDocumentRow>(row => row.AttachmentId, row => row.AttachmentDocumentId)
                ////.LeftJoin<AttachmentDocumentRow, DocumentVersionRow>(row => row.DocumentVersionId, row => row.DocumentVersionId)
                ////.LeftJoin<DocumentVersionRow, DocumentRow>(row => row.DocumentId, row => row.DocumentId)
                .LeftJoin<UnifiedFieldValueRow, UnifiedIntegerValueRow>(row => row.UnifiedFieldValueId, row => row.UnifiedFieldValueId)
                .LeftJoin<UnifiedIntegerValueRow, ml_custom_field_valueRow>(row => row.IntegerValue, row => row.mlcfv_id)
                .LeftJoin<UnifiedFieldValueRow, UnifiedDateValueRow>(row => row.UnifiedFieldValueId, row => row.UnifiedFieldValueId)
                .LeftJoin<UnifiedFieldValueRow, UnifiedStringValueRow>(row => row.UnifiedFieldValueId, row => row.UnifiedFieldValueId)
                .LeftJoin<UnifiedFieldValueRow, UnifiedNumericValueRow>(row => row.UnifiedFieldValueId, row => row.UnifiedFieldValueId)
                .Relations;

        /////// <summary>
        /////// Gets or sets the UnifiedIntegerValueId.
        /////// </summary>
        ////[ResultColumn]
        ////[RelatedEntity(typeof(UnifiedIntegerValueRow))]
        ////public long UnifiedIntegerValueId { get; set; }

        /// <summary>
        /// Gets or sets the IntegerValue.
        /// </summary>
        [RelatedEntity(typeof(UnifiedIntegerValueRow))]
        public long? IntegerValue { get; set; }

        /// <summary>
        /// Gets or sets the identifier field value.
        /// </summary>
        [RelatedEntity(typeof(ml_custom_field_valueRow), PhysicalName = "mlcfv_value")]
        public string EnumeratedValueName { get; set; }

        /// <summary>
        /// Gets or sets the identifier field sort order.
        /// </summary>
        [RelatedEntity(typeof(ml_custom_field_valueRow), PhysicalName = "mlcfv_sort")]
        public int? EnumeratedValueSortOrder { get; set; }

        /////// <summary>
        /////// Gets or sets the UnifiedDateValueId.
        /////// </summary>
        ////[ResultColumn]
        ////[RelatedEntity(typeof(UnifiedDateValueRow))]
        ////public long UnifiedDateValueId { get; set; }

        /// <summary>
        /// Gets or sets the DateValue.
        /// </summary>
        [RelatedEntity(typeof(UnifiedDateValueRow))]
        public DateTimeOffset? DateValue { get; set; }

        /////// <summary>
        /////// Gets or sets the UnifiedStringValueId.
        /////// </summary>
        ////[ResultColumn]
        ////[RelatedEntity(typeof(UnifiedStringValueRow))]
        ////public long UnifiedStringValueId { get; set; }

        /// <summary>
        /// Gets or sets the StringValue.
        /// </summary>
        [RelatedEntity(typeof(UnifiedStringValueRow))]
        public string StringValue { get; set; }

        /////// <summary>
        /////// Gets or sets the UnifiedNumericValueId.
        /////// </summary>
        ////[ResultColumn]
        ////[RelatedEntity(typeof(UnifiedNumericValueRow))]
        ////public long UnifiedNumericValueId { get; set; }

        /// <summary>
        /// Gets or sets the NumericValue.
        /// </summary>
        [RelatedEntity(typeof(UnifiedNumericValueRow))]
        public double? NumericValue { get; set; }

        /// <summary>
        /// Gets the entity relations.
        /// </summary>
        public override IEnumerable<IEntityRelation> EntityRelations
        {
            get
            {
                return FormSubmissionValueDetailRelations;
            }
        }
    }
}