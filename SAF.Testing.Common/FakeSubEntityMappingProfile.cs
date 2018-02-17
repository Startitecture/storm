﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeSubEntityMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    using Startitecture.Repository.Mapping;

    /// <summary>
    /// The fake sub entity mapping profile.
    /// </summary>
    public class FakeSubEntityMappingProfile : EntityMappingProfile<FakeSubEntity, FakeFlatSubRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeSubEntityMappingProfile"/> class.
        /// </summary>
        public FakeSubEntityMappingProfile()
        {
            this.SetPrimaryKey(entity => entity.FakeSubEntityId, row => row.FakeSubEntityId);
            this.CreateRelatedEntityProfile(entity => entity.FakeSubSubEntity, row => row.FakeSubSubEntityId)
                .MapEntityProperty(entity => entity.Description, row => row.FakeSubSubEntityDescription)
                .MapEntityProperty(entity => entity.UniqueName, row => row.FakeSubSubEntityUniqueName);
        }
    }
}
