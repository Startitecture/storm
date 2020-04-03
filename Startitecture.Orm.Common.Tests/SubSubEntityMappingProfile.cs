// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubSubEntityMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common.Tests
{
    using AutoMapper;

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
            this.CreateMap<SubSubEntity, SubSubRow>()
                .ForMember(row => row.TransactionProvider, expression => expression.Ignore());

            this.CreateMap<SubSubRow, SubSubEntity>();
        }
    }
}
