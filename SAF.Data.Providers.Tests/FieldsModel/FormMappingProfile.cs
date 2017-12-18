// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormMappingProfile.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The form mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    /// <summary>
    /// The form mapping profile.
    /// </summary>
    public class FormMappingProfile : EntityMappingProfile<Form, FormRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormMappingProfile"/> class.
        /// </summary>
        public FormMappingProfile()
        {
            this.SetPrimaryKey(form => form.FormId, row => row.FormId)
                .SetUniqueKey(row => row.Name);
        }
    }
}