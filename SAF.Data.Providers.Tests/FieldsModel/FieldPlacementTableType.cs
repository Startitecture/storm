// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldPlacementTableType.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The field placement table type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    /// <summary>
    /// The field placement table type.
    /// </summary>
    [TableType("[dbo].[FieldPlacementTableType]")]
    [TableName("[dbo].[FieldPlacement]")]
    [PrimaryKey("FieldPlacementId", AutoIncrement = true)]
    public class FieldPlacementTableType
    {
        /// <summary>
        /// Gets or sets the FieldPlacementId.
        /// </summary>
        [Column]
        public int FieldPlacementId { get; set; }

        /// <summary>
        /// Gets or sets the LayoutSectionId.
        /// </summary>
        [Column]
        public int LayoutSectionId { get; set; }

        /// <summary>
        /// Gets or sets the UnifiedFieldId.
        /// </summary>
        [Column]
        public int UnifiedFieldId { get; set; }

        /// <summary>
        /// Gets or sets the Order.
        /// </summary>
        [Column]
        public short Order { get; set; }

        /// <summary>
        /// Gets or sets the CSS style.
        /// </summary>
        [Column]
        public string CssStyle { get; set; }
    }
}