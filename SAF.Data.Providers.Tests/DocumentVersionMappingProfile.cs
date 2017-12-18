// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentVersionMappingProfile.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    /// <summary>
    /// The document version mapping profile.
    /// </summary>
    public class DocumentVersionMappingProfile : EntityMappingProfile<DocumentVersion, DocumentVersionRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentVersionMappingProfile" /> class.
        /// </summary>
        public DocumentVersionMappingProfile()
        {
            this.SetPrimaryKey(version => version.DocumentVersionId, row => row.DocumentVersionId)
                .MapRelation(version => version.Document, row => row.Document, row => row.DocumentId);
        }
    }
}