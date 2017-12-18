// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeCreatedByMappingProfile.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The fake created by mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    using SAF.Data;

    /// <summary>
    /// The fake created by mapping profile.
    /// </summary>
    public class FakeCreatedByMappingProfile : EntityMappingProfile<FakeCreatedBy, FakeMultiReferenceRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeCreatedByMappingProfile"/> class.
        /// </summary>
        public FakeCreatedByMappingProfile()
        {
            this.SetPrimaryKey(by => by.FakeMultiReferenceEntityId, row => row.FakeMultiReferenceEntityId).SetUniqueKey(row => row.UniqueName);
        }
    }
}
