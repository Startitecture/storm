// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeRaisedDataRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The fake raised data row.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Orm.Model;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Sql;

    /// <summary>
    /// The fake raised data row.
    /// </summary>
    public class FakeRaisedDataRow : FakeDataRowBase, ICompositeEntity
    {
        /// <summary>
        /// The lazy relations.
        /// </summary>
        private static readonly Lazy<EntityRelationSet<FakeRaisedDataRow>> LazyRelations = 
            new Lazy<EntityRelationSet<FakeRaisedDataRow>>(
                () =>
                    {
                        return
                            new SqlFromClause<FakeRaisedDataRow>()
                                .InnerJoin(row => row.FakeDataId, row => row.FakeRelated.FakeDataId)
                                .InnerJoin(row => row.FakeRelated.RelatedId, row => row.FakeDependencyEntity.FakeComplexEntityId)
                                .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                                .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.FakeComplexEntityId)
                                .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                                .LeftJoin<FakeSubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId);
                    });

        /// <summary>
        /// Gets or sets the fake related row.
        /// </summary>
        [Relation]
        public FakeRelatedRow FakeRelated { get; set; }

        /// <summary>
        /// Gets or sets the other alias.
        /// </summary>
        [Relation]
        public FakeRelatedRow OtherAlias { get; set; }

        /// <summary>
        /// Gets or sets the related dependency.
        /// </summary>
        [Relation]
        public FakeDependencyRow RelatedDependency { get; set; }

        /// <summary>
        /// Gets or sets the related alias.
        /// </summary>
        [Relation]
        public FakeRelatedRow RelatedAlias { get; set; }

        /// <summary>
        /// Gets or sets the related dependency.
        /// </summary>
        [Relation]
        public FakeDependencyRow FakeDependencyEntity { get; set; }

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
                return LazyRelations.Value.Relations;
            }
        }
    }
}
