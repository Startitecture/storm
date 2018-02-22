// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The form mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using Startitecture.Orm.Repository;

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