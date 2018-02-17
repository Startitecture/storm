// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeCreatedByMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The fake created by mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    using SAF.Data;

    using Startitecture.Repository.Mapping;

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
