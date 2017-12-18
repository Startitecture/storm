// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Utility.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    using System.Diagnostics.CodeAnalysis;

    using SAF.Data;

    using Startitecture.Repository.Mapping;

    /// <summary>
    /// The utility.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class Utility
    {
        /// <summary>
        /// The create entity mapper.
        /// </summary>
        /// <returns>
        /// The <see cref="IEntityMapper" />.
        /// </returns>
        public static AutoMapperEntityMapper CreateEntityMapper()
        {
            var autoMapperEntityMapper = new AutoMapperEntityMapper();
            autoMapperEntityMapper.Initialize(
                configuration =>
                    {
                        configuration.AddProfile<FakeSubSubEntityMappingProfile>();
                        configuration.AddProfile<FakeMultiReferenceEntityMappingProfile>();
                        configuration.AddProfile<FakeCreatedByMappingProfile>();
                        configuration.AddProfile<FakeModifiedByMappingProfile>();
                        configuration.AddProfile<FakeSubEntityMappingProfile>();
                        configuration.AddProfile<FakeRaisedSubEntityMappingProfile>();
                        configuration.AddProfile<FakeChildEntityMappingProfile>();
                        configuration.AddProfile<FakeComplexEntityMappingProfile>();
                        configuration.AddProfile<FakeRaisedComplexEntityMappingProfile>();
                        configuration.AddProfile<FakeDependentEntityMappingProfile>();
                    });

            return autoMapperEntityMapper;
        }
    }
}