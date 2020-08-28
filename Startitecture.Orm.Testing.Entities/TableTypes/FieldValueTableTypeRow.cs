// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldValueTableTypeRow.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Represents a user-defined table type for the FieldValueRow entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Entities.TableTypes
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    using Startitecture.Orm.Schema;

    /// <summary>
    /// Represents a user-defined table type for the <see cref="FieldValueRow"/> entity.
    /// </summary>
    [TableType("FieldValueTableType")]
    [Table("FieldValue", Schema = "dbo")]
    public class FieldValueTableTypeRow
    {
        /// <summary>
        /// Gets or sets the field value id.
        /// </summary>
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long? FieldValueId { get; set; }

        /// <summary>
        /// Gets or sets the field id.
        /// </summary>
        [Column(Order = 2)]
        public int FieldId { get; set; }

        /// <summary>
        /// Gets or sets the last modified by domain identifier id.
        /// </summary>
        [Column(Order = 3)]
        public int LastModifiedByDomainIdentifierId { get; set; }

        /// <summary>
        /// Gets or sets the last modified time.
        /// </summary>
        [Column(Order = 4)]
        public DateTimeOffset LastModifiedTime { get; set; }
    }
}