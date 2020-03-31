// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeSubSubEntityMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using AutoMapper;

    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The fake sub sub entity mapping profile.
    /// </summary>
    public class FakeSubSubEntityMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeSubSubEntityMappingProfile"/> class.
        /// </summary>
        public FakeSubSubEntityMappingProfile()
        {
            this.CreateMap<SubSubEntity, SubSubRow>()
                .ForMember(row => row.TransactionProvider, expression => expression.Ignore());

            this.CreateMap<SubSubRow, SubSubEntity>();
        }
    }
}
