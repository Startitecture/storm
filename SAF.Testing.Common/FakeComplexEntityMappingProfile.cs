// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeComplexEntityMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    using SAF.Data;

    using Startitecture.Repository.Mapping;

    /// <summary>
    /// The fake entity mapping profile.
    /// </summary>
    public class FakeComplexEntityMappingProfile : EntityMappingProfile<FakeComplexEntity, FakeComplexRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeComplexEntityMappingProfile"/> class.
        /// </summary>
        public FakeComplexEntityMappingProfile()
        {
            this.SetPrimaryKey(entity => entity.FakeComplexEntityId, row => row.FakeComplexEntityId)
                .SetUniqueKey(row => row.UniqueName)
                .MapProperty(entity => entity.FakeEnumeration, row => row.FakeEnumerationId)
                .MapProperty(entity => entity.FakeOtherEnumeration, row => row.FakeOtherEnumerationId);

            this.CreateRelatedEntityProfile(entity => entity.FakeSubEntity, row => row.FakeSubEntityId)
                .MapRelatedEntity(entity => entity.FakeSubSubEntity, row => row.FakeSubSubEntityId)
                .MapEntityProperty(entity => entity.UniqueName, row => row.FakeSubEntityUniqueName)
                .MapEntityProperty(entity => entity.Description, row => row.FakeSubEntityDescription)
                .MapEntityProperty(entity => entity.UniqueOtherId, row => row.FakeSubEntityUniqueOtherId);

            this.CreateRelatedEntityProfile(entity => entity.FakeSubSubEntity, row => row.FakeSubSubEntityId)
                .MapEntityProperty(entity => entity.UniqueName, row => row.FakeSubSubEntityUniqueName)
                .MapEntityProperty(entity => entity.Description, row => row.FakeSubSubEntityDescription);

            this.CreateRelatedEntityProfile(entity => entity.FakeDependentEntity, row => row.FakeDependentEntityId)
                .MapEntityProperty(entity => entity.DependentIntegerValue, row => row.FakeDependentEntityDependentIntegerValue)
                .MapEntityProperty(entity => entity.DependentTimeValue, row => row.FakeDependentEntityDependentTimeValue);

            // Specify the entity key since it isn't the same property name.
            this.CreateRelatedEntityProfile(
                    entity => entity.CreatedBy,
                    row => row.CreatedByFakeMultiReferenceEntityId,
                    by => by.FakeMultiReferenceEntityId)
                .MapEntityProperty(by => by.UniqueName, row => row.CreatedByUniqueName)
                .MapEntityProperty(by => by.Description, row => row.CreatedByDescription);

            // Specify the entity key since it isn't the same property name.
            this.CreateRelatedEntityProfile(
                    entity => entity.ModifiedBy,
                    row => row.ModifiedByFakeMultiReferenceEntityId,
                    by => by.FakeMultiReferenceEntityId)
                .MapEntityProperty(by => by.UniqueName, row => row.ModifiedByUniqueName)
                .MapEntityProperty(by => by.Description, row => row.ModifiedByDescription);
        }
    }
}
