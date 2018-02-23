// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeRaisedComplexRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests.Models
{
    using System;
    using System.Collections.Generic;

    using SAF.Data.Providers.Tests;

    using Startitecture.Core;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Sql;

    /// <summary>
    /// The fake complex row.
    /// </summary>
    [TableName("[dbo].[FakeComplexEntity]")]
    [PrimaryKey("FakeComplexEntityId")]
    public class FakeRaisedComplexRow : TransactionItemBase, ICompositeEntity, IEquatable<FakeRaisedComplexRow>
    {
        /// <summary>
        /// The lazy relations.
        /// </summary>
        private static readonly Lazy<IEnumerable<IEntityRelation>> LazyRelations =
            new Lazy<IEnumerable<IEntityRelation>>(
                () =>
                new SqlFromClause<FakeRaisedComplexRow>()
                    .InnerJoin(row => row.FakeSubEntityId, row => row.FakeSubEntity.FakeSubEntityId)
                    .InnerJoin(row => row.FakeSubEntity.FakeSubSubEntityId, row => row.FakeSubEntity.FakeSubSubEntity.FakeSubSubEntityId)
                    .InnerJoin(row => row.CreatedByFakeMultiReferenceEntityId, row => row.CreatedBy.FakeMultiReferenceEntityId)
                    .InnerJoin(row => row.ModifiedByFakeMultiReferenceEntityId, row => row.ModifiedBy.FakeMultiReferenceEntityId)
                    .LeftJoin(row => row.FakeComplexEntityId, row => row.FakeDependentEntity.FakeDependentEntityId)
                    .Relations);

        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<FakeRaisedComplexRow, object>[] ComparisonProperties =
            {
                item => item.UniqueName,
                item => item.CreationTime,
                item => item.Description,
                item => item.FakeEnumerationId,
                item => item.FakeOtherEnumerationId,
                item => item.FakeSubEntityId,
                item => item.ModifiedTime,
                item => item.FakeSubEntity,
                item => item.FakeDependentEntity,
                item => item.CreatedBy,
                item => item.ModifiedBy
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeRaisedComplexRow"/> class. 
        /// </summary>
        public FakeRaisedComplexRow()
        {
            // All non-optional entities are created here. Not necessary but avoids having to create them with reflection
            this.ModifiedBy = new FakeMultiReferenceRow();
            this.CreatedBy = new FakeMultiReferenceRow();
            this.FakeSubEntity = new FakeRaisedSubRow();
        }

        /// <summary>
        /// Gets or sets the fake complex entity id.
        /// </summary>
        [Column]
        public int FakeComplexEntityId { get; set; }

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

        /// <summary>
        /// Gets or sets the fake sub entity id.
        /// </summary>
        [Column]
        public int FakeSubEntityId { get; set; }

        /// <summary>
        /// Gets or sets the fake enumeration.
        /// </summary>
        [Column]
        public int FakeEnumerationId { get; set; }

        /// <summary>
        /// Gets or sets the fake other enumeration.
        /// </summary>
        [Column]
        public int FakeOtherEnumerationId { get; set; }

        /// <summary>
        /// Gets or sets the creation time.
        /// </summary>
        [Column]
        public DateTimeOffset CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the modified time.
        /// </summary>
        [Column]
        public DateTimeOffset ModifiedTime { get; set; }

        /// <summary>
        /// Gets or sets the created by fake multi reference entity id.
        /// </summary>
        [Column]
        public int CreatedByFakeMultiReferenceEntityId { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        [Relation]
        public FakeMultiReferenceRow CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the modified by fake multi reference entity id.
        /// </summary>
        [Column]
        public int ModifiedByFakeMultiReferenceEntityId { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        [Relation]
        public FakeMultiReferenceRow ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the fake sub entity.
        /// </summary>
        [Relation]
        public FakeRaisedSubRow FakeSubEntity { get; set; }

        /// <summary>
        /// Gets or sets the fake dependent entity.
        /// </summary>
        [Relation]
        public FakeDependentRow FakeDependentEntity { get; set; }

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
        public static bool operator ==(FakeRaisedComplexRow valueA, FakeRaisedComplexRow valueB)
        {
            return EqualityComparer<FakeRaisedComplexRow>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(FakeRaisedComplexRow valueA, FakeRaisedComplexRow valueB)
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
        public bool Equals(FakeRaisedComplexRow other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }
    }
}
