// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeModifiedByMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The fake modified by mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using AutoMapper;

    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The fake modified by mapping profile.
    /// </summary>
    public class FakeModifiedByMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeModifiedByMappingProfile"/> class.
        /// </summary>
        public FakeModifiedByMappingProfile()
        {
            this.CreateMap<ModifiedBy, MultiReferenceRow>()
                .ForMember(row => row.TransactionProvider, expression => expression.Ignore());

            this.CreateMap<MultiReferenceRow, ModifiedBy>();
        }
    }
}
