// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutPageMappingProfile.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The layout page mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    /// <summary>
    /// The layout page mapping profile.
    /// </summary>
    public class LayoutPageMappingProfile : EntityMappingProfile<LayoutPage, LayoutPageRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutPageMappingProfile"/> class.
        /// </summary>
        public LayoutPageMappingProfile()
        {
            this.SetPrimaryKey(page => page.LayoutPageId, row => row.LayoutPageId)
                .MapRelation(page => page.FormLayout, row => row.FormLayout, row => row.FormLayoutId);
        }
    }
}