// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnifiedFieldMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The unified field mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using Startitecture.Repository.Mapping;

    /// <summary>
    /// The unified field mapping profile.
    /// </summary>
    public class UnifiedFieldMappingProfile : EntityMappingProfile<UnifiedField, UnifiedFieldRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnifiedFieldMappingProfile"/> class.
        /// </summary>
        public UnifiedFieldMappingProfile()
        {
            this.SetPrimaryKey(field => field.UnifiedFieldId, row => row.UnifiedFieldId)
                .MapProperty(field => field.UnifiedFieldType, row => row.UnifiedFieldTypeId)
                .MapProperty(field => field.UnifiedValueType, row => row.UnifiedValueTypeId);
        }
    }
}