// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeFlatDataRow.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    using System;
    using System.Collections.Generic;

    using SAF.Data;
    using SAF.Data.Providers;

    /// <summary>
    /// The fake data row.
    /// </summary>
    [PrimaryKey("FakeRowId", AutoIncrement = true)]
    [TableName("[FakeData]")]
    public class FakeFlatDataRow : FakeDataRowBase, ICompositeEntity
    {
        /// <summary>
        /// The lazy relations.
        /// </summary>
        private static readonly Lazy<IEnumerable<IEntityRelation>> LazyRelations = new Lazy<IEnumerable<IEntityRelation>>(RelateEntities);

        /// <summary>
        /// Gets or sets the fake related related id.
        /// </summary>
        [RelatedEntity(typeof(FakeRelatedRow), true)]
        public int FakeRelatedRelatedId { get; set; }

        /// <summary>
        /// Gets or sets the fake related related property.
        /// </summary>
        [RelatedEntity(typeof(FakeRelatedRow), true)]
        public string FakeRelatedRelatedProperty { get; set; }

        /// <summary>
        /// Gets or sets the related id.
        /// </summary>
        [RelatedEntity(typeof(FakeRelatedRow), false, "RelatedAlias")]
        public int RelatedId { get; set; }

        /// <summary>
        /// Gets or sets the related alias related property.
        /// </summary>
        [RelatedEntity(typeof(FakeRelatedRow), true, "RelatedAlias")]
        public string RelatedAliasRelatedProperty { get; set; }

        /// <summary>
        /// Gets or sets the other alias related id.
        /// </summary>
        [RelatedEntity(typeof(FakeRelatedRow), true, "OtherAlias")]
        public int OtherAliasRelatedId { get; set; }

        /// <summary>
        /// Gets or sets the other alias related property.
        /// </summary>
        [RelatedEntity(typeof(FakeRelatedRow), true, "OtherAlias")]
        public string OtherAliasRelatedProperty { get; set; }

        /// <summary>
        /// Gets or sets the fake sub data id.
        /// </summary>
        [RelatedEntity(typeof(FakeSubDataRow))]
        public int? FakeSubDataId { get; set; }

        /// <summary>
        /// Gets or sets the fake sub data id.
        /// </summary>
        [RelatedEntity(typeof(FakeSubDataRow))]
        public int? ParentFakeDataId { get; set; }

        /// <summary>
        /// Gets the entity relations.
        /// </summary>
        public IEnumerable<IEntityRelation> EntityRelations
        {
            get
            {
                return LazyRelations.Value;
            }
        }

        /// <summary>
        /// Relates the entities of the current composite entity.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="IEntityRelation"/> elements for a composite entity.
        /// </returns>
        private static IEnumerable<IEntityRelation> RelateEntities()
        {
            return
                new TransactSqlFromClause<FakeFlatDataRow>()
                    .InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId)
                    .InnerJoin<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId)
                    .InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "OtherAlias")
                    .InnerJoin<FakeRelatedRow, FakeDependencyRow>(
                        row => row.RelatedId,
                        "OtherAlias",
                        row => row.FakeComplexEntityId,
                        "RelatedDependency")
                    .InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "RelatedAlias")
                    .LeftJoin<FakeSubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId)
                    .Relations;
        }
    }
}