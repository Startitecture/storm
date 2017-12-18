// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeMultiReferenceEntityMappingProfile.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    using SAF.Data;

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
