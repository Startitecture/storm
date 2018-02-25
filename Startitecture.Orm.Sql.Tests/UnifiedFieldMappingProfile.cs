// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnifiedFieldMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The unified field mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql.Tests
{
    using Startitecture.Orm.Repository;
    using Startitecture.Orm.Testing.Model;
    using Startitecture.Orm.Testing.Model.FieldEntities;

    using UnifiedFieldRow = Startitecture.Orm.Testing.Model.DocumentEntities.UnifiedFieldRow;

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
