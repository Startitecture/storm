// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeDependentEntityMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using Startitecture.Orm.Repository;
    using Startitecture.Orm.Testing.Model;

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