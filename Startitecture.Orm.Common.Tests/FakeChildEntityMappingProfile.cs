// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeChildEntityMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Defines the FakeChildEntityMappingProfile type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common.Tests
{
    using global::AutoMapper;

    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The fake child entity mapping profile.
    /// </summary>
    public class FakeChildEntityMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeChildEntityMappingProfile"/> class.
        /// </summary>
        public FakeChildEntityMappingProfile()
        {
            this.CreateMap<ChildEntity, ChildRaisedRow>()
                .ForMember(row => row.TransactionProvider, expression => expression.Ignore());

            // TODO: Only filled if the cache is set. Should we have it do a unique select if not found?
            this.CreateMap<ChildRaisedRow, ChildEntity>()
                .ForMember(
                    entity => entity.Parent,
                    expression => expression.MapFrom(
                        row => row.TransactionProvider.DependencyContainer.GetDependency<ChildRaisedRow>(
                            row.ParentFakeChildEntityId.GetValueOrDefault())));
        }
    }
}
