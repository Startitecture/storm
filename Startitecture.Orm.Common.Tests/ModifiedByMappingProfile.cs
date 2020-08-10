// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModifiedByMappingProfile.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The fake modified by mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common.Tests
{
    using global::AutoMapper;

    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The fake modified by mapping profile.
    /// </summary>
    public class ModifiedByMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModifiedByMappingProfile"/> class.
        /// </summary>
        public ModifiedByMappingProfile()
        {
            this.CreateMap<ModifiedBy, MultiReferenceRow>();
            this.CreateMap<MultiReferenceRow, ModifiedBy>();
        }
    }
}
