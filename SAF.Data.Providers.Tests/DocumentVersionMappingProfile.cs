// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentVersionMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using Startitecture.Orm.Repository;
    using Startitecture.Orm.Testing.Model;

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