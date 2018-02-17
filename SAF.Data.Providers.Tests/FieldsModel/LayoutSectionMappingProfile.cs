// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutSectionMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The layout section mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using Startitecture.Repository.Mapping;

    /// <summary>
    /// The layout section mapping profile.
    /// </summary>
    public class LayoutSectionMappingProfile : EntityMappingProfile<LayoutSection, LayoutSectionRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutSectionMappingProfile"/> class.
        /// </summary>
        public LayoutSectionMappingProfile()
        {
            this.SetPrimaryKey(section => section.LayoutSectionId, row => row.LayoutSectionId)
                .SetUniqueKey(section => section.InstanceIdentifier);
        }
    }
}