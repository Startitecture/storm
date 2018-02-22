// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormLayoutMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The form layout mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using Startitecture.Orm.Repository;

    /// <summary>
    /// The form layout mapping profile.
    /// </summary>
    public class FormLayoutMappingProfile : EntityMappingProfile<FormLayout, FormLayoutRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormLayoutMappingProfile"/> class.
        /// </summary>
        public FormLayoutMappingProfile()
        {
            this.SetPrimaryKey(layout => layout.FormLayoutId, row => row.FormLayoutId);
        }
    }
}