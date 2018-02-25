// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentTypeMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using Startitecture.Orm.Testing.Model;
    using Startitecture.Orm.Testing.Model.DocumentEntities;

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