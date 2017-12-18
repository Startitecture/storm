// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldPlacementMappingProfile.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The field placement mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    /// <summary>
    /// The field placement mapping profile.
    /// </summary>
    public class FieldPlacementMappingProfile : EntityMappingProfile<FieldPlacement, FieldPlacementRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldPlacementMappingProfile" /> class.
        /// </summary>
        public FieldPlacementMappingProfile()
        {
            this.SetPrimaryKey(placement => placement.FieldPlacementId, row => row.FieldPlacementId)
                .MapRelation(placement => placement.LayoutSection, row => row.LayoutSection, row => row.LayoutSectionId)
                .MapRelation(placement => placement.UnifiedField, row => row.UnifiedField, row => row.UnifiedFieldId);

            this.CreateMap<FieldPlacement, FieldPlacementTableType>();
        }
    }
}