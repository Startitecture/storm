// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeComplexRow.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    using System;
    using System.Collections.Generic;

    using SAF.Core;
    using SAF.Data;
    using SAF.Data.Providers;

    /// <summary>
    /// The fake complex row.
    /// </summary>
    [TableName("FakeComplexEntity")]
    [PrimaryKey("FakeComplexEntityId")]
    public class FakeComplexRow : TransactionItemBase, ICompositeEntity, IEquatable<FakeComplexRow>
    {
        /// <summary>
        /// The lazy relations.
        /// </summary>
        private static readonly Lazy<IEnumerable<IEntityRelation>> LazyRelations =
            new Lazy<IEnumerable<IEntityRelation>>(
                () =>
                new TransactSqlFromClause<FakeComplexRow>().InnerJoin<FakeSubRow>(row => row.FakeSubEntityId, entity => entity.FakeSubEntityId)
                    .InnerJoin<FakeSubRow, FakeSubSubRow>(row => row.FakeSubSubEntityId, row => row.FakeSubSubEntityId)
                    .InnerJoin<FakeMultiReferenceRow>(row => row.CreatedByFakeMultiReferenceEntityId, row => row.FakeMultiReferenceEntityId)
                    .InnerJoin<FakeMultiReferenceRow>(row => row.ModifiedByFakeMultiReferenceEntityId, row => row.FakeMultiReferenceEntityId)
                    .LeftJoin<FakeDependentRow>(row => row.FakeDependentEntityId, row => row.FakeDependentEntityId)
                    .Relations);

        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<FakeComplexRow, object>[] ComparisonProperties =
            {
                item => item.UniqueName,
                item => item.CreatedByDescription,
                item => item.CreatedByFakeMultiReferenceEntityId,
                item => item.CreatedByUniqueName,
                item => item.CreationTime,
                item => item.Description,
                item => item.FakeDependentEntityDependentIntegerValue,
                item => item.FakeDependentEntityDependentTimeValue,
                item => item.FakeDependentEntityId,
                item => item.FakeEnumerationId,
                item => item.FakeOtherEnumerationId,
                item => item.FakeSubEntityDescription,
                item => item.FakeSubEntityUniqueName,
                item => item.FakeSubEntityId,
                item => item.FakeSubEntityUniqueOtherId,
                item => item.FakeSubSubEntityDescription,
                item => item.FakeSubSubEntityId,
                item => item.FakeSubSubEntityUniqueName,
                item => item.ModifiedByDescription,
                item => item.ModifiedByFakeMultiReferenceEntityId,
                item => item.ModifiedByUniqueName,
                item => item.ModifiedTime,
                ////item => item.FakeSubSubEntity,
                ////item => item.FakeSubEntity,
                ////item => item.FakeDependentEntity,
                ////item => item.CreatedBy,
                ////item => item.ModifiedBy
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeComplexRow"/> class.
        /// </summary>
        public FakeComplexRow()
        {
            // All non-optional entities are created here.
            ////this.ModifiedBy = new FakeMultiReferenceRow();
            ////this.CreatedBy = new FakeMultiReferenceRow();
            ////this.FakeSubEntity = new FakeSubRow();
            ////this.FakeSubSubEntity = new FakeSubSubRow();
        }

        /// <summary>
        /// Gets or sets the fake complex entity id.
        /// </summary>
        public int FakeComplexEntityId { get; set; }

        /// <summary>
        /// Gets or sets the unique name.
        /// </summary>
        public string UniqueName { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the fake sub entity id.
        /// </summary>
        public int FakeSubEntityId { get; set; }

        /// <summary>
        /// Gets or sets the fake enumeration.
        /// </summary>
        public int FakeEnumerationId { get; set; }

        /// <summary>
        /// Gets or sets the fake other enumeration.
        /// </summary>
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
        public DateTimeOffset CreationTime { get; set; }

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
        public DateTimeOffset ModifiedTime { get; set; }

        /// <summary>
        /// Gets or sets the fake dependent entity id.
        /// </summary>
        [RelatedEntity(typeof(FakeDependentEntity))]
        public int? FakeDependentEntityId { get; set; }

        /// <summary>
        /// Gets or sets the dependent integer value.
        /// </summary>
        [RelatedEntity(typeof(FakeDependentEntity), true)]
        public int? FakeDependentEntityDependentIntegerValue { get; set; }

        /// <summary>
        /// Gets or sets the dependent time value.
        /// </summary>
        [RelatedEntity(typeof(FakeDependentEntity), true)]
        public DateTimeOffset? FakeDependentEntityDependentTimeValue { get; set; }

        /// <summary>
        /// Gets the entity relations.
        /// </summary>
        [Ignore]
        public IEnumerable<IEntityRelation> EntityRelations
        {
            get
            {
                return LazyRelations.Value;
            }
        }

        /// <summary>
        /// Determines if two values of the same type are equal.
        /// </summary>
        /// <param name="valueA">
        /// The first value to compare.
        /// </param>
        /// <param name="valueB">
        /// The second value to compare.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(FakeComplexRow valueA, FakeComplexRow valueB)
        {
            return EqualityComparer<FakeComplexRow>.Default.Equals(valueA, valueB);
        }

        /// <summary>
        /// Determines if two values of the same type are not equal.
        /// </summary>
        /// <param name="valueA">
        /// The first value to compare.
        /// </param>
        /// <param name="valueB">
        /// The second value to compare.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(FakeComplexRow valueA, FakeComplexRow valueB)
        {
            return !(valueA == valueB);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.UniqueName;
        }

        /// <summary>
        /// Serves as the default hash function. 
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return Evaluate.GenerateHashCode(this, ComparisonProperties);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            return Evaluate.Equals(this, obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(FakeComplexRow other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }
    }
}
