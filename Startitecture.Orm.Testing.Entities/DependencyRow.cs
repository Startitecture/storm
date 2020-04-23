// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Startitecture.Orm.Mapper;

    /// <summary>
    /// The fake dependency row.
    /// </summary>
    [Table("DependencyEntity", Schema = "dbo")]
    public class DependencyRow : EntityBase
    {
        /// <summary>
        /// Gets or sets the fake dependency entity id.
        /// </summary>
        [Column]
        [Key]
        public long FakeDependencyEntityId { get; set; }

        /// <summary>
        /// Gets or sets the fake complex entity id.
        /// </summary>
        [Column]
        public int ComplexEntityId { get; set; }

        /// <summary>
        /// Gets or sets the unique name.
        /// </summary>
        [Column]
        public string UniqueName { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [Column]
        public string Description { get; set; }
    }
}
