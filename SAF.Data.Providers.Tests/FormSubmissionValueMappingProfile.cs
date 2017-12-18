// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormSubmissionValueMappingProfile.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The form submission value mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    /// <summary>
    /// The form submission value mapping profile.
    /// </summary>
    public class FormSubmissionValueMappingProfile : EntityMappingProfile<FormSubmissionValue, FormSubmissionValueRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormSubmissionValueMappingProfile"/> class.
        /// </summary>
        public FormSubmissionValueMappingProfile()
        {
            // Because FormSubmissionValue is inherited from UnifiedFieldValue, we use the superclass ID (which is settable).
            this.SetPrimaryKey(dto => dto.UnifiedFieldValueId, row => row.FormSubmissionValueId)
                .MapProperty(value => value.UnifiedFieldType, row => row.UnifiedFieldTypeId)
                .MapProperty(value => value.UnifiedValueType, row => row.UnifiedValueTypeId)
                .ResolveUnmappedEntity(value => value.LastModifiedPerson, row => row.LastModifiedPersonId);

            this.CreateRelatedEntityProfile(dto => dto.UnifiedField, row => row.UnifiedFieldId)
                .MapEntityProperty(dto => dto.Caption, row => row.UnifiedFieldCaption)
                .MapEntityProperty(dto => dto.Name, row => row.UnifiedFieldName)
                .MapEntityProperty(dto => dto.Label, row => row.UnifiedFieldLabel)
                .MapEntityProperty(field => field.UnifiedFieldType, row => row.UnifiedFieldTypeId)
                .MapEntityProperty(field => field.UnifiedValueType, row => row.UnifiedValueTypeId)
                .Ignore(field => field.SystemFieldSourceId, field => field.CustomFieldId);
        }
    }
}