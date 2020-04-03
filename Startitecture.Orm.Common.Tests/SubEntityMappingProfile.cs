// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubEntityMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common.Tests
{
    using AutoMapper;

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
            this.CreateMap<SubEntity, SubRow>()
                .ForMember(row => row.TransactionProvider, expression => expression.Ignore());

            this.CreateMap<SubRow, SubEntity>();
        }
    }
}
