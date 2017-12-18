// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentTypeMappingProfile.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    /// <summary>
    /// The document type mapping profile.
    /// </summary>
    public class DocumentTypeMappingProfile : EntityMappingProfile<DocumentType, DocumentTypeRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentTypeMappingProfile" /> class.
        /// </summary>
        public DocumentTypeMappingProfile()
        {
            this.SetPrimaryKey(type => type.DocumentTypeId, row => row.DocumentTypeId);
        }
    }
}