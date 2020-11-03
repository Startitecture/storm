// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldValueElementPgFlatRow.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.SqlTypes;

    using Startitecture.Orm.Schema;

    /// <summary>
    /// Represents a flattened field value element row.
    /// </summary>
    [Table("FieldValueElement", Schema = "dbo")]
    public class FieldValueElementPgFlatRow
    {
        /// <summary>
        /// Gets or sets the field value element ID.
        /// </summary>
        [Column(Order = 1)]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        [RelatedEntity(typeof(DateElementRow), PhysicalName = "Value")]
        public DateTimeOffset? DateElement { get; set; }

        /// <summary>
        /// Gets or sets the float element.
        /// </summary>
        [Column(Order = 5)]
        [RelatedEntity(typeof(FloatElementRow), PhysicalName = "Value")]
        public double? FloatElement { get; set; }

        /// <summary>
        /// Gets or sets the integer element.
        /// </summary>
        [Column(Order = 6)]
        [RelatedEntity(typeof(IntegerElementRow), PhysicalName = "Value")]
        public long? IntegerElement { get; set; }

        /// <summary>
        /// Gets or sets the money element.
        /// </summary>
        [Column(Order = 7)]
        [RelatedEntity(typeof(MoneyElementRow), PhysicalName = "Value")]
        public decimal? MoneyElement { get; set; }

        /// <summary>
        /// Gets or sets the text element.
        /// </summary>
        [Column(Order = 8)]
        [RelatedEntity(typeof(TextElementRow), PhysicalName = "Value")]
        public string TextElement { get; set; }
    }
}