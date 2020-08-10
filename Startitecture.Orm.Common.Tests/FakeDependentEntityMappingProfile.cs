// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeDependentEntityMappingProfile.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The fake dependent entity mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common.Tests
{
    using global::AutoMapper;

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
            this.CreateMap<DependentEntity, DependentRow>();
            this.CreateMap<DependentRow, DependentEntity>();
        }
    }
}