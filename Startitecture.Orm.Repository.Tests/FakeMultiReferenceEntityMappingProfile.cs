// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeMultiReferenceEntityMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using AutoMapper;

    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The fake multi reference entity mapping profile.
    /// </summary>
    public class FakeMultiReferenceEntityMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeMultiReferenceEntityMappingProfile"/> class.
        /// </summary>
        public FakeMultiReferenceEntityMappingProfile()
        {
            this.CreateMap<MultiReferenceEntity, MultiReferenceRow>()
                .ForMember(row => row.TransactionProvider, expression => expression.Ignore());
        }
    }
}
