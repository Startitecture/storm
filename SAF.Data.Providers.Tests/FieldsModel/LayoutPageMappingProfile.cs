// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutPageMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The layout page mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using Startitecture.Orm.Repository;

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