// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeCreatedByMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The fake created by mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using AutoMapper;

    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The fake created by mapping profile.
    /// </summary>
    public class FakeCreatedByMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeCreatedByMappingProfile"/> class.
        /// </summary>
        public FakeCreatedByMappingProfile()
        {
            this.CreateMap<CreatedBy, MultiReferenceRow>().ForMember(row => row.TransactionProvider, expression => expression.Ignore());
            this.CreateMap<MultiReferenceRow, CreatedBy>();
        }
    }
}
