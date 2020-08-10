// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreatedByMappingProfile.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The fake created by mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common.Tests
{
    using global::AutoMapper;

    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The fake created by mapping profile.
    /// </summary>
    public class CreatedByMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatedByMappingProfile"/> class.
        /// </summary>
        public CreatedByMappingProfile()
        {
            this.CreateMap<CreatedBy, MultiReferenceRow>();
            this.CreateMap<MultiReferenceRow, CreatedBy>();
        }
    }
}
