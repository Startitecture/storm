// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentMappingProfile.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common.Tests
{
    using global::AutoMapper;

    using Startitecture.Orm.Testing.Model;
    using Startitecture.Orm.Testing.Model.Entities;

    /// <summary>
    /// The document mapping profile.
    /// </summary>
    public class DocumentMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentMappingProfile"/> class.
        /// </summary>
        public DocumentMappingProfile()
        {
            this.CreateMap<DocumentRow, Document>();
        }
    }
}