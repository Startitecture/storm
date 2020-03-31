// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeSubEntityMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using AutoMapper;

    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The fake sub entity mapping profile.
    /// </summary>
    public class FakeSubEntityMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeSubEntityMappingProfile"/> class.
        /// </summary>
        public FakeSubEntityMappingProfile()
        {
            this.CreateMap<SubEntity, SubRow>()
                .ForMember(row => row.TransactionProvider, expression => expression.Ignore());

            this.CreateMap<SubRow, SubEntity>();
        }
    }
}
