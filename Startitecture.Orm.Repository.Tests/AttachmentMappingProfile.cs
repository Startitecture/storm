// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttachmentMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using Startitecture.Orm.Testing.Model;
    using Startitecture.Orm.Testing.Model.DocumentEntities;

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