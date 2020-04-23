// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChildRaisedRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChildRaisedRow type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The fake raised child row.
    /// </summary>
    [Table("ChildEntity")]
    public class ChildRaisedRow : EntityBase
    {
        /// <summary>
        /// Gets or sets the fake child entity id.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(Order = 1)]
        public int FakeChildEntityId { get; set; }

        /// <summary>
        /// Gets or sets the fake complex entity id.
        /// </summary>
        [Column(Order = 2)]
        public int ComplexEntityId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Column(Order = 3)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the some value.
        /// </summary>
        [Column(Order = 4)]
        public int SomeValue { get; set; }

        /// <summary>
        /// Gets or sets the fake complex entity.
        /// </summary>
        [Relation]
        public ComplexRaisedRow ComplexEntity { get; set; }

        /// <summary>
        /// Gets or sets the parent fake child entity id.
        /// </summary>
        /// <remarks>
        /// For unmapped entities we still need to use the related entity approach.
        /// </remarks>
        [RelatedEntity(typeof(SubChildRow))]
        public int? ParentFakeChildEntityId { get; set; }
    }
}
