// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeModifiedByMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The fake modified by mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using Startitecture.Orm.Repository;
    using Startitecture.Orm.Testing.Model;

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
