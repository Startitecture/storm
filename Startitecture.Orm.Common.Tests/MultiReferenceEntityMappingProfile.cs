// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiReferenceEntityMappingProfile.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The fake multi reference entity mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common.Tests
{
    using global::AutoMapper;

    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The fake multi reference entity mapping profile.
    /// </summary>
    public class MultiReferenceEntityMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiReferenceEntityMappingProfile"/> class.
        /// </summary>
        public MultiReferenceEntityMappingProfile()
        {
            this.CreateMap<MultiReferenceEntity, MultiReferenceRow>();
        }
    }
}
