// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeDependentEntityMappingProfile.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    using SAF.Data;

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