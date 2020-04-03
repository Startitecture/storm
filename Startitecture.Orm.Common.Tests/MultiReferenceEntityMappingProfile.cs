// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiReferenceEntityMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common.Tests
{
    using AutoMapper;

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
            this.CreateMap<MultiReferenceEntity, MultiReferenceRow>()
                .ForMember(row => row.TransactionProvider, expression => expression.Ignore());
        }
    }
}
