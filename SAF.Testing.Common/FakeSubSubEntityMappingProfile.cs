// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeSubSubEntityMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    using SAF.Data;

    using Startitecture.Repository.Mapping;

    /// <summary>
    /// The fake sub sub entity mapping profile.
    /// </summary>
    public class FakeSubSubEntityMappingProfile : EntityMappingProfile<FakeSubSubEntity, FakeSubSubRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeSubSubEntityMappingProfile"/> class.
        /// </summary>
        public FakeSubSubEntityMappingProfile()
        {
            this.SetPrimaryKey(entity => entity.FakeSubSubEntityId, row => row.FakeSubSubEntityId)
                .SetUniqueKey(row => row.UniqueName);
        }
    }
}
