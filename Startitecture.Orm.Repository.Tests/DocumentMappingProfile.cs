// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using Startitecture.Orm.Testing.Model;
    using Startitecture.Orm.Testing.Model.DocumentEntities;

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