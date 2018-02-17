// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutPageSectionMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The layout page section mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using Startitecture.Repository.Mapping;

    /// <summary>
    /// The layout page section mapping profile.
    /// </summary>
    public class LayoutPageSectionMappingProfile : EntityMappingProfile<LayoutPageSection, LayoutPageSectionRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutPageSectionMappingProfile" /> class.
        /// </summary>
        public LayoutPageSectionMappingProfile()
        {
            this.SetPrimaryKey(section => section.LayoutPageSectionId, row => row.LayoutPageSectionId)
                .MapRelation(section => section.LayoutPage, row => row.LayoutPage, row => row.LayoutPageId)
                .MapRelation(section => section.LayoutSection, row => row.LayoutSection, row => row.LayoutSectionId);

            ////// Map from the entity to the table type.
            ////this.CreateMap<LayoutPageSection, LayoutPageSectionTableType>();
        }
    }
}