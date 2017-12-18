// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnifiedFieldMappingProfile.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The unified field mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
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
                .MapProperty(field => field.UnifiedValueType, row => row.UnifiedValueTypeId)
                .MapProperty(field => field.UnifiedFieldType, row => row.UnifiedFieldTypeId);
        }
    }
}
