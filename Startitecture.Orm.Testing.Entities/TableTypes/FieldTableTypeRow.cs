// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldTableTypeRow.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Represents a user-defined table type for the FieldRow entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Entities.TableTypes
{
    using System.ComponentModel.DataAnnotations.Schema;

    using Startitecture.Orm.Schema;

    /// <summary>
    /// Represents a user-defined table type for the <see cref="FieldRow"/> entity.
    /// </summary>
    [TableType("FieldTableType")]
    [Table("Field", Schema = "dbo")]
    public class FieldTableTypeRow
    {
        /// <summary>
        /// Gets or sets the field ID.
        /// </summary>
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? FieldId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Column(Order = 2)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [Column(Order = 3)]
        public string Description { get; set; }
    }
}