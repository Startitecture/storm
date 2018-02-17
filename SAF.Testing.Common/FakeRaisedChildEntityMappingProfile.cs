// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeRaisedChildEntityMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    using Startitecture.Repository.Mapping;

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