// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutSectionMappingProfile.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The layout section mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
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