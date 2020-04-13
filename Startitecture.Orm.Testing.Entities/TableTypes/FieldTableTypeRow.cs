// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldTableTypeRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Entities.TableTypes
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Startitecture.Orm.Schema;

    /// <summary>
    /// The field data type row.
    /// </summary>
    [TableType("FieldTableType")]
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