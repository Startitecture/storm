// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeChildRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Defines the FakeChildRow type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests.Models
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Orm.Model;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Sql;

    /// <summary>
    /// The fake child row.
    /// </summary>
    [TableName("FakeComplexEntity")]
    [PrimaryKey("FakeComplexEntityId")]
    public class FakeChildRow : TransactionItemBase, ICompositeEntity
    {
        /// <summary>
        /// The fake child relations.
        /// </summary>
        private static readonly IEnumerable<IEntityRelation> FakeChildRelations = new SqlFromClause<FakeChildRow>()
            .InnerJoin<FakeComplexRow>(row => row.FakeComplexEntityId, row => row.FakeComplexEntityId)
            .InnerJoin<FakeComplexRow, FakeSubRow>(row => row.FakeSubEntityId, entity => entity.FakeSubEntityId)
            .InnerJoin<FakeSubRow, FakeSubSubRow>(row => row.FakeSubSubEntityId, row => row.FakeSubSubEntityId)
            .InnerJoin<FakeComplexRow, FakeMultiReferenceRow>(
                row => row.CreatedByFakeMultiReferenceEntityId,
                row => row.FakeMultiReferenceEntityId,
                "CreatedBy")
            .InnerJoin<FakeComplexRow, FakeMultiReferenceRow>(
                row => row.ModifiedByFakeMultiReferenceEntityId,
                row => row.FakeMultiReferenceEntityId,
                "ModifiedBy")
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
        /// Gets or sets the unique name.
        /// </summary>
        [RelatedEntity(typeof(FakeComplexRow), false)]
        public string FakeComplexEntityUniqueName { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [RelatedEntity(typeof(FakeComplexRow), false)]
        public string FakeComplexEntityDescription { get; set; }

        /// <summary>
        /// Gets or sets the fake sub entity id.
        /// </summary>
        [RelatedEntity(typeof(FakeComplexRow), false)]
        public int FakeSubEntityId { get; set; }

        /// <summary>
        /// Gets or sets the fake enumeration.
        /// </summary>
        [RelatedEntity(typeof(FakeComplexRow), false)]
        public int FakeEnumerationId { get; set; }

        /// <summary>
        /// Gets or sets the fake other enumeration.
        /// </summary>
        [RelatedEntity(typeof(FakeComplexRow), false)]
        public int FakeOtherEnumerationId { get; set; }

        /// <summary>
        /// Gets or sets the fake sub unique name.
        /// </summary>
        [RelatedEntity(typeof(FakeSubRow), true)]
        public string FakeSubEntityUniqueName { get; set; }

        /// <summary>
        /// Gets or sets the fake sub description.
        /// </summary>
        [RelatedEntity(typeof(FakeSubRow), true)]
        public string FakeSubEntityDescription { get; set; }

        // Here we do not need to alias the name because it is unique within the selection.

        /// <summary>
        /// Gets or sets the unique other id.
        /// </summary>
        [RelatedEntity(typeof(FakeSubRow))]
        public short FakeSubEntityUniqueOtherId { get; set; }

        /// <summary>
        /// Gets or sets the fake sub sub entity id.
        /// </summary>
        [RelatedEntity(typeof(FakeSubRow))]
        public int FakeSubSubEntityId { get; set; }

        /// <summary>
        /// Gets or sets the fake sub sub unique name.
        /// </summary>
        [RelatedEntity(typeof(FakeSubSubRow), true)]
        public string FakeSubSubEntityUniqueName { get; set; }

        /// <summary>
        /// Gets or sets the fake sub sub description.
        /// </summary>
        [RelatedEntity(typeof(FakeSubSubRow), true)]
        public string FakeSubSubEntityDescription { get; set; }

        /// <summary>
        /// Gets or sets the created by fake multi reference entity id.
        /// </summary>
        [RelatedEntity(typeof(FakeMultiReferenceRow), true, "CreatedBy")]
        public int CreatedByFakeMultiReferenceEntityId { get; set; }

        /// <summary>
        /// Gets or sets the created by unique name.
        /// </summary>
        [RelatedEntity(typeof(FakeMultiReferenceRow), true, "CreatedBy")]
        public string CreatedByUniqueName { get; set; }

        /// <summary>
        /// Gets or sets the created by description.
        /// </summary>
        [RelatedEntity(typeof(FakeMultiReferenceRow), true, "CreatedBy")]
        public string CreatedByDescription { get; set; }

        /// <summary>
        /// Gets or sets the creation time.
        /// </summary>
        [RelatedEntity(typeof(FakeComplexRow), false)]
        public DateTimeOffset FakeComplexEntityCreationTime { get; set; }

        /// <summary>
        /// Gets or sets the modified by fake multi reference entity id.
        /// </summary>
        [RelatedEntity(typeof(FakeMultiReferenceRow), true, "ModifiedBy")]
        public int ModifiedByFakeMultiReferenceEntityId { get; set; }

        /// <summary>
        /// Gets or sets the modified by unique name.
        /// </summary>
        [RelatedEntity(typeof(FakeMultiReferenceRow), true, "ModifiedBy")]
        public string ModifiedByUniqueName { get; set; }

        /// <summary>
        /// Gets or sets the modified by description.
        /// </summary>
        [RelatedEntity(typeof(FakeMultiReferenceRow), true, "ModifiedBy")]
        public string ModifiedByDescription { get; set; }

        /// <summary>
        /// Gets or sets the modified time.
        /// </summary>
        [RelatedEntity(typeof(FakeComplexRow), false)]
        public DateTimeOffset FakeComplexEntityModifiedTime { get; set; }

        /// <summary>
        /// Gets or sets the parent fake child entity id.
        /// </summary>
        [RelatedEntity(typeof(FakeChildRow), false)]
        public int? ParentFakeChildEntityId { get; set; }

        /// <summary>
        /// Gets or sets the fake dependent entity id.
        /// </summary>
        [RelatedEntity(typeof(FakeDependentRow))]
        public int? FakeDependentEntityId { get; set; }

        /// <summary>
        /// Gets or sets the dependent integer value.
        /// </summary>
        [RelatedEntity(typeof(FakeDependentRow), true)]
        public int? FakeDependentEntityDependentIntegerValue { get; set; }

        /// <summary>
        /// Gets or sets the dependent time value.
        /// </summary>
        [RelatedEntity(typeof(FakeDependentRow), true)]
        public DateTimeOffset? FakeDependentEntityDependentTimeValue { get; set; }

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
