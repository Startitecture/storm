// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Utility.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    using System.Diagnostics.CodeAnalysis;

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
        /// The <see cref="Startitecture.Orm.Common.IEntityMapper" />.
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