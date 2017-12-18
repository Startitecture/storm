// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormLayoutMappingProfile.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The form layout mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
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