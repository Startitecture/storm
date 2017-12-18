// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttachmentDocumentRow.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The raised attachment document row.
    /// </summary>
    public partial class AttachmentDocumentRow : ICompositeEntity
    {
        /// <summary>
        /// The attachment document relations.
        /// </summary>
        private static readonly Lazy<IEnumerable<IEntityRelation>> AttachmentDocumentRelations =
            new Lazy<IEnumerable<IEntityRelation>>(
                () => new TransactSqlFromClause<AttachmentDocumentRow>()
                    .InnerJoin(row => row.AttachmentDocumentId, row => row.AttachmentId)
                    .InnerJoin(row => row.DocumentTypeId, row => row.DocumentType.DocumentTypeId)
                    .InnerJoin(row => row.DocumentVersionId, row => row.DocumentVersion.DocumentVersionId)
                    .InnerJoin(row => row.DocumentVersion.DocumentId, row => row.DocumentVersion.Document.DocumentId)
                    .Relations);

        /// <summary>
        /// Gets or sets the attachment id.
        /// </summary>
        [RelatedEntity(typeof(AttachmentRow))]
        public long AttachmentId { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        [RelatedEntity(typeof(AttachmentRow))]
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the document type id.
        /// </summary>
        [RelatedEntity(typeof(AttachmentRow))]
        public int DocumentTypeId { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        [RelatedEntity(typeof(AttachmentRow))]
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        [RelatedEntity(typeof(AttachmentRow))]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the created time.
        /// </summary>
        [RelatedEntity(typeof(AttachmentRow))]
        public DateTimeOffset CreatedTime { get; set; }

        /// <summary>
        /// Gets or sets the last modified by.
        /// </summary>
        [RelatedEntity(typeof(AttachmentRow))]
        public string LastModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the last modified time.
        /// </summary>
        [RelatedEntity(typeof(AttachmentRow))]
        public DateTimeOffset LastModifiedTime { get; set; }

        /// <summary>
        /// Gets or sets the document version.
        /// </summary>
        [Relation]
        public DocumentVersionRow DocumentVersion { get; set; }

        /// <summary>
        /// Gets or sets the document type.
        /// </summary>
        [Relation]
        public DocumentTypeRow DocumentType { get; set; }

        /// <summary>
        /// Gets the entity relations.
        /// </summary>
        public IEnumerable<IEntityRelation> EntityRelations => AttachmentDocumentRelations.Value;
    }
}