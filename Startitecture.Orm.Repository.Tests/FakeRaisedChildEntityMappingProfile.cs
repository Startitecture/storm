// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeRaisedChildEntityMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using Startitecture.Orm.Repository;
    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The fake raised child entity mapping profile.
    /// </summary>
    public class FakeRaisedChildEntityMappingProfile : EntityMappingProfile<FakeChildEntity, FakeRaisedChildRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeRaisedChildEntityMappingProfile"/> class.
        /// </summary>
        public FakeRaisedChildEntityMappingProfile()
        {
            this.SetPrimaryKey(entity => entity.FakeChildEntityId, row => row.FakeChildEntityId)
                .MapRelation(entity => entity.FakeComplexEntity, row => row.FakeComplexEntity, row => row.FakeComplexEntityId)
                .ResolveUnmappedEntity(entity => entity.Parent, row => row.ParentFakeChildEntityId);
        }
    }
}