// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeMultiReferenceEntityMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using Startitecture.Orm.Repository;
    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The fake multi reference entity mapping profile.
    /// </summary>
    public class FakeMultiReferenceEntityMappingProfile : EntityMappingProfile<FakeMultiReferenceEntity, FakeMultiReferenceRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeMultiReferenceEntityMappingProfile"/> class.
        /// </summary>
        public FakeMultiReferenceEntityMappingProfile()
        {
            this.SetPrimaryKey(entity => entity.FakeMultiReferenceEntityId, row => row.FakeMultiReferenceEntityId)
                .WriteOnce(row => row.Description, row => row.UniqueName);
        }
    }
}
