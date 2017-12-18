// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeModifiedByMappingProfile.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The fake modified by mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    using SAF.Data;

    /// <summary>
    /// The fake modified by mapping profile.
    /// </summary>
    public class FakeModifiedByMappingProfile : EntityMappingProfile<FakeModifiedBy, FakeMultiReferenceRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeModifiedByMappingProfile"/> class.
        /// </summary>
        public FakeModifiedByMappingProfile()
        {
            this.SetPrimaryKey(by => by.FakeMultiReferenceEntityId, row => row.FakeMultiReferenceEntityId).SetUniqueKey(row => row.UniqueName);
        }
    }
}
