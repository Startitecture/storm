// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutPageSectionTableType.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The layout page section table type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System.ComponentModel.DataAnnotations.Schema;

    using Startitecture.Orm.Schema;

    /// <summary>
    /// The layout page section table type.
    /// </summary>
    [TableType("[dbo].[LayoutPageSectionTableType]")]
    public class LayoutPageSectionTableType
    {
        /// <summary>
        /// Gets or sets the LayoutPageSectionId.
        /// </summary>
        [Column]
        public int LayoutPageSectionId { get; set; }

        /// <summary>
        /// Gets or sets the LayoutPageId.
        /// </summary>
        [Column]
        public int LayoutPageId { get; set; }

        /// <summary>
        /// Gets or sets the LayoutSectionId.
        /// </summary>
        [Column]
        public int LayoutSectionId { get; set; }

        /// <summary>
        /// Gets or sets the Order.
        /// </summary>
        [Column]
        public short Order { get; set; }
    }
}