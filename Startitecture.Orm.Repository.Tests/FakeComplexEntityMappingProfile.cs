// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeComplexEntityMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using AutoMapper;

    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The fake entity mapping profile.
    /// </summary>
    public class FakeComplexEntityMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeComplexEntityMappingProfile"/> class.
        /// </summary>
        public FakeComplexEntityMappingProfile()
        {
            this.CreateMap<ComplexEntity, ComplexRaisedRow>()
                .ForMember(row => row.TransactionProvider, expression => expression.Ignore())
                .ForMember(row => row.FakeEnumerationId, expression => expression.MapFrom(entity => entity.FakeEnumeration))
                .ForMember(row => row.FakeOtherEnumerationId, expression => expression.MapFrom(entity => entity.FakeOtherEnumeration));

            this.CreateMap<ComplexRaisedRow, ComplexEntity>()
                .ForMember(entity => entity.FakeEnumeration, expression => expression.MapFrom(row => row.FakeEnumerationId))
                .ForMember(entity => entity.FakeOtherEnumeration, expression => expression.MapFrom(row => row.FakeOtherEnumerationId));
        }
    }
}
