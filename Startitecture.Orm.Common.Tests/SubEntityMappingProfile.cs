// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubEntityMappingProfile.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The fake sub entity mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common.Tests
{
    using global::AutoMapper;

    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The fake sub entity mapping profile.
    /// </summary>
    public class SubEntityMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubEntityMappingProfile"/> class.
        /// </summary>
        public SubEntityMappingProfile()
        {
            this.CreateMap<SubEntity, SubRow>();
            this.CreateMap<SubRow, SubEntity>();
        }
    }
}
