// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentTypeMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using Startitecture.Orm.Repository;
    using Startitecture.Orm.Testing.Model;

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