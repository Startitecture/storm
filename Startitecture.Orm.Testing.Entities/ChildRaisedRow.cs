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
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The fake raised child row.
    /// </summary>
    [Table("ChildEntity")]
    public class ChildRaisedRow : TransactionItemBase
    {
        /// <summary>
        /// The fake child relations.
        /// </summary>
        private static readonly IEnumerable<IEntityRelation> FakeChildRelations =
            new EntityRelationSet<ChildRaisedRow>()
                .InnerJoin(
                    row => row.FakeComplexEntityId,
                    row => row.ComplexEntity.FakeComplexEntityId)
                .InnerJoin(row => row.ComplexEntity.FakeSubEntityId, row => row.ComplexEntity.SubEntity.FakeSubEntityId)
                .InnerJoin(
                    row => row.ComplexEntity.SubEntity.FakeSubSubEntityId,
                    row => row.ComplexEntity.SubEntity.SubSubEntity.FakeSubSubEntityId)
                .InnerJoin(
                    row => row.ComplexEntity.CreatedByFakeMultiReferenceEntityId,
                    row => row.ComplexEntity.CreatedBy.FakeMultiReferenceEntityId)
                .InnerJoin(
                    row => row.ComplexEntity.ModifiedByFakeMultiReferenceEntityId,
                    row => row.ComplexEntity.ModifiedBy.FakeMultiReferenceEntityId)
                .LeftJoin<SubChildRow>(row => row.FakeChildEntityId, row => row.FakeSubChildEntityId)
                .Relations;

        /// <summary>
        /// Gets or sets the fake child entity id.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FakeChildEntityId { get; set; }

        /// <summary>
        /// Gets or sets the fake complex entity id.
        /// </summary>
        public int FakeComplexEntityId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the some value.
        /// </summary>
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

        /// <summary>
        /// Gets the entity relations.
        /// </summary>
        public IEnumerable<IEntityRelation> EntityRelations
        {
            get
            {
                return FakeChildRelations;
            }
        }
    }
}
