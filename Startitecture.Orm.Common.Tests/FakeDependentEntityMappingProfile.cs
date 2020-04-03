// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeDependentEntityMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common.Tests
{
    using AutoMapper;

    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The fake dependent entity mapping profile.
    /// </summary>
    public class FakeDependentEntityMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeDependentEntityMappingProfile"/> class.
        /// </summary>
        public FakeDependentEntityMappingProfile()
        {
            this.CreateMap<DependentEntity, DependentRow>()
                .ForMember(row => row.TransactionProvider, expression => expression.Ignore());

            this.CreateMap<DependentRow, DependentEntity>();
        }
    }
}