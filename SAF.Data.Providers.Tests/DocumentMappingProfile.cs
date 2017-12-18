// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentMappingProfile.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    /// <summary>
    /// The document mapping profile.
    /// </summary>
    public class DocumentMappingProfile : EntityMappingProfile<Document, DocumentRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentMappingProfile" /> class.
        /// </summary>
        public DocumentMappingProfile()
        {
            this.SetPrimaryKey(document => document.DocumentId, row => row.DocumentId);
        }
    }
}