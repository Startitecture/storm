// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeChildEntityMappingProfile.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Defines the FakeChildEntityMappingProfile type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.Testing.Common
{
    using SAF.Data;

    /// <summary>
    /// The fake child entity mapping profile.
    /// </summary>
    public class FakeChildEntityMappingProfile : EntityMappingProfile<FakeChildEntity, FakeChildRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeChildEntityMappingProfile"/> class.
        /// </summary>
        public FakeChildEntityMappingProfile()
        {
            // Setting the primary key creates a link between the data item and its key and the entity's key. This can be done when the
            // entity's key is nullable but the data item's key is not as long as they are the same base type.
            this.SetPrimaryKey(entity => entity.FakeChildEntityId, row => row.FakeChildEntityId)
                
                // These enumerations are referenced from the related entity but still have to be mapped to the row.
                .MapProperty(entity => entity.FakeEnumeration, row => row.FakeEnumerationId)
                .MapProperty(entity => entity.FakeOtherEnumeration, row => row.FakeOtherEnumerationId)

                // This entity (a Parent which can be recursive) cannot be mapped from the row itself.
                .ResolveUnmappedEntity(entity => entity.Parent, row => row.ParentFakeChildEntityId);

            // Creating a dependency profile handles the first-level dependencies of the entity.
            this.CreateRelatedEntityProfile(entity => entity.FakeComplexEntity, row => row.FakeComplexEntityId)

                // The first-level dependencies of this dependency must also be addressed.
                .MapRelatedEntity(entity => entity.FakeSubEntity, row => row.FakeSubEntityId)
                .MapRelatedEntity(entity => entity.FakeDependentEntity, row => row.FakeDependentEntityId)
                .MapRelatedEntity(entity => entity.CreatedBy, row => row.CreatedByFakeMultiReferenceEntityId)
                .MapRelatedEntity(entity => entity.ModifiedBy, row => row.ModifiedByFakeMultiReferenceEntityId)

                // All non-key, non-dependency and non-enumeration properties of dependencies have to be mapped by convention.
                .MapEntityProperty(entity => entity.Description, row => row.FakeComplexEntityDescription)
                .MapEntityProperty(entity => entity.ModifiedTime, row => row.FakeComplexEntityModifiedTime)

                // All row columns which do not match entity property names must be mapped specifically. 
                .MapEntityProperty(entity => entity.UniqueName, row => row.FakeComplexEntityUniqueName)
                .MapEntityProperty(entity => entity.CreationTime, row => row.FakeComplexEntityCreationTime)
                .MapEntityProperty(entity => entity.FakeEnumeration, row => row.FakeEnumerationId)
                .MapEntityProperty(entity => entity.FakeOtherEnumeration, row => row.FakeOtherEnumerationId);

            // Maps directly from the source for the complex object and by convention for the destination names.
            this.CreateRelatedEntityProfile(entity => entity.FakeSubEntity, row => row.FakeSubEntityId)
                .MapRelatedEntity(dest => dest.FakeSubSubEntity, row => row.FakeSubSubEntityId)
                .MapEntityProperty(entity => entity.UniqueOtherId, row => row.FakeSubEntityUniqueOtherId)

                // MapByConvention can map multiple properteis but only if they are of the same type.
                .MapEntityProperty(entity => entity.Description, row => row.FakeSubEntityDescription)
                .MapEntityProperty(entity => entity.UniqueName, row => row.FakeSubEntityUniqueName);

            // Maps the two string elements by convention to complete the previous mapping.
            this.CreateRelatedEntityProfile(entity => entity.FakeSubSubEntity, row => row.FakeSubSubEntityId)
                .MapEntityProperty(entity => entity.Description, row => row.FakeSubSubEntityDescription)
                .MapEntityProperty(entity => entity.UniqueName, row => row.FakeSubSubEntityUniqueName);

            // Use sourceCanMap = false to prevent a recursive mapping.
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
