// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeRaisedSubEntityMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The fake sub raised entity mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    using Startitecture.Repository.Mapping;

    /// <summary>
    /// The fake sub raised entity mapping profile.
    /// </summary>
    public class FakeRaisedSubEntityMappingProfile : EntityMappingProfile<FakeSubEntity, FakeRaisedSubRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeRaisedSubEntityMappingProfile"/> class.
        /// </summary>
        public FakeRaisedSubEntityMappingProfile()
        {
            this.SetPrimaryKey(entity => entity.FakeSubEntityId, row => row.FakeSubEntityId)
                .SetUniqueKey(row => row.UniqueName)
                .MapRelation(entity => entity.FakeSubSubEntity, row => row.FakeSubSubEntity, row => row.FakeSubSubEntityId);
        }
    }
}