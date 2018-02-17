// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeDependentEntityMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    using SAF.Data;

    using Startitecture.Repository.Mapping;

    /// <summary>
    /// The fake dependent entity mapping profile.
    /// </summary>
    public class FakeDependentEntityMappingProfile : EntityMappingProfile<FakeDependentEntity, FakeDependentRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeDependentEntityMappingProfile"/> class.
        /// </summary>
        public FakeDependentEntityMappingProfile()
        {
            this.SetPrimaryKey(entity => entity.FakeDependentEntityId, row => row.FakeDependentEntityId)
                .SetDependencyKey<FakeComplexRow, int>(row => row.FakeComplexEntityId, row => row.FakeDependentEntityId);
        }
    }
}