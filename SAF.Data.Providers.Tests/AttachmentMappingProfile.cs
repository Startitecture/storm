// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttachmentMappingProfile.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    /// <summary>
    /// The attachment mapping profile.
    /// </summary>
    public class AttachmentMappingProfile : EntityMappingProfile<Attachment, AttachmentRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttachmentMappingProfile" /> class.
        /// </summary>
        public AttachmentMappingProfile()
        {
            this.SetPrimaryKey(attachment => attachment.AttachmentId, row => row.AttachmentId)
                .MapRelation(attachment => attachment.DocumentType, row => row.DocumentType, row => row.DocumentTypeId)
                .WriteOnce(row => row.CreatedBy, row => row.CreatedTime);
        }
    }
}