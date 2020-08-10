// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubSubEntityMappingProfile.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The fake sub sub entity mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common.Tests
{
    using global::AutoMapper;

    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The fake sub sub entity mapping profile.
    /// </summary>
    public class SubSubEntityMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubSubEntityMappingProfile"/> class.
        /// </summary>
        public SubSubEntityMappingProfile()
        {
            this.CreateMap<SubSubEntity, SubSubRow>();
            this.CreateMap<SubSubRow, SubSubEntity>();
        }
    }
}
