// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeRaisedComplexEntityMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The fake complex raised entity profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    using Startitecture.Repository.Mapping;

    /// <summary>
    /// The fake complex raised entity profile.
    /// </summary>
    public class FakeRaisedComplexEntityMappingProfile : EntityMappingProfile<FakeComplexEntity, FakeRaisedComplexRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeRaisedComplexEntityMappingProfile"/> class.
        /// </summary>
        public FakeRaisedComplexEntityMappingProfile()
        {
            this.SetPrimaryKey(entity => entity.FakeComplexEntityId, row => row.FakeComplexEntityId)
                .SetUniqueKey(row => row.UniqueName)
                .MapRelation(entity => entity.FakeSubEntity, row => row.FakeSubEntity, row => row.FakeSubEntityId)
                .MapRelation(entity => entity.CreatedBy, row => row.CreatedBy, row => row.FakeMultiReferenceEntityId)
                .MapRelation(entity => entity.ModifiedBy, row => row.ModifiedBy, row => row.FakeMultiReferenceEntityId)
                .MapRelation(entity => entity.FakeDependentEntity, row => row.FakeDependentEntity, row => row.FakeDependentEntityId)
                .MapProperty(entity => entity.FakeEnumeration, row => row.FakeEnumerationId)
                .MapProperty(entity => entity.FakeOtherEnumeration, row => row.FakeOtherEnumerationId);
        }
    }
}