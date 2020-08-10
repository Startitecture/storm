// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeRelatedRow.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The fake related row.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// The fake related row.
    /// </summary>
    [Table("Related", Schema = "someschema")]
    public class FakeRelatedRow
    {
        /// <summary>
        /// Gets or sets the related id.
        /// </summary>
        [Column]
        [Key]
        public int RelatedId { get; set; }

        /// <summary>
        /// Gets or sets the fake data id.
        /// </summary>
        [Column]
        public int FakeDataId { get; set; }

        /// <summary>
        /// Gets or sets the related property.
        /// </summary>
        [Column]
        public string RelatedProperty { get; set; }
    }
}