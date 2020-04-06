// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldValueElementTableTypeRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Entities.TableTypes
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    using Startitecture.Orm.Schema;

    /// <summary>
    /// The field value element table type row.
    /// </summary>
    [TableType("FieldValueElementTableType")]
    public class FieldValueElementTableTypeRow
    {
        /// <summary>
        /// Gets or sets the field value element ID.
        /// </summary>
        [Column(Order = 1)]
        public long? FieldValueElementId { get; set; }

        /// <summary>
        /// Gets or sets the field value ID.
        /// </summary>
        [Column(Order = 2)]
        public long FieldValueId { get; set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        [Column(Order = 3)]
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the date element.
        /// </summary>
        [Column(Order = 4)]
        public DateTimeOffset? DateElement { get; set; }

        /// <summary>
        /// Gets or sets the float element.
        /// </summary>
        [Column(Order = 5)]
        public double? FloatElement { get; set; }

        /// <summary>
        /// Gets or sets the integer element.
        /// </summary>
        [Column(Order = 6)]
        public long? IntegerElement { get; set; }

        /// <summary>
        /// Gets or sets the money element.
        /// </summary>
        [Column(Order = 7)]
        public decimal? MoneyElement { get; set; }

        /// <summary>
        /// Gets or sets the text element.
        /// </summary>
        [Column(Order = 8)]
        public string TextElement { get; set; }
    }
}