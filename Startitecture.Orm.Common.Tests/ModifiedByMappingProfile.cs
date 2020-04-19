// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModifiedByMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
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
            this.CreateMap<ModifiedBy, MultiReferenceRow>()
                .ForMember(row => row.TransactionProvider, expression => expression.Ignore());

            this.CreateMap<MultiReferenceRow, ModifiedBy>();
        }
    }
}
