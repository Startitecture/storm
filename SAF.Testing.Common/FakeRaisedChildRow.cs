// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeRaisedChildRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Defines the FakeRaisedChildRow type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    using System.Collections.Generic;

    using SAF.Data;
    using SAF.Data.Providers;

    using Startitecture.Orm.Model;
    using Startitecture.Orm.Query;

    /// <summary>
    /// The fake raised child row.
    /// </summary>
    [TableName("FakeChildEntity")]
    [PrimaryKey("FakeChildEntityId", AutoIncrement = true)]
    public class FakeRaisedChildRow : TransactionItemBase, ICompositeEntity
    {
        /// <summary>
        /// The fake child relations.
        /// </summary>
        private static readonly IEnumerable<IEntityRelation> FakeChildRelations =
            new TransactSqlFromClause<FakeRaisedChildRow>()
                .InnerJoin(
                    row => row.FakeComplexEntityId,
                    row => row.FakeComplexEntity.FakeComplexEntityId)
                .InnerJoin(row => row.FakeComplexEntity.FakeSubEntityId, row => row.FakeComplexEntity.FakeSubEntity.FakeSubEntityId)
                .InnerJoin(
                    row => row.FakeComplexEntity.FakeSubEntity.FakeSubSubEntityId,
                    row => row.FakeComplexEntity.FakeSubEntity.FakeSubSubEntity.FakeSubSubEntityId)
                .InnerJoin(
                    row => row.FakeComplexEntity.CreatedByFakeMultiReferenceEntityId,
                    row => row.FakeComplexEntity.CreatedBy.FakeMultiReferenceEntityId)
                .InnerJoin(
                    row => row.FakeComplexEntity.ModifiedByFakeMultiReferenceEntityId,
                    row => row.FakeComplexEntity.ModifiedBy.FakeMultiReferenceEntityId)
                .LeftJoin<FakeSubChildRow>(row => row.FakeChildEntityId, row => row.FakeSubChildEntityId)
                .Relations;

        /// <summary>
        /// Gets or sets the fake child entity id.
        /// </summary>
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
        public FakeRaisedComplexRow FakeComplexEntity { get; set; }

        /// <summary>
        /// Gets or sets the parent fake child entity id.
        /// </summary>
        /// <remarks>
        /// For unmapped entities we still need to use the related entity approach.
        /// </remarks>
        [RelatedEntity(typeof(FakeSubChildRow))]
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
