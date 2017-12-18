// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttachmentDocumentMappingProfile.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    /// <summary>
    /// The attachment document mapping profile.
    /// </summary>
    public class AttachmentDocumentMappingProfile : EntityMappingProfile<AttachmentDocument, AttachmentDocumentRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttachmentDocumentMappingProfile" /> class.
        /// </summary>
        public AttachmentDocumentMappingProfile()
        {
            this.SetPrimaryKey(document => document.AttachmentId, row => row.AttachmentDocumentId)
                .MapRelation(document => document.DocumentVersion, row => row.DocumentVersion, row => row.DocumentVersionId)
                .MapRelation(document => document.DocumentType, row => row.DocumentType, row => row.DocumentTypeId);
        }
    }
}