// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComplexFlatRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Startitecture.Core;
    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The fake complex row.
    /// </summary>
    [Table("ComplexEntity")]
    public class ComplexFlatRow : TransactionItemBase, IEquatable<ComplexFlatRow>
    {
        /// <summary>
        /// The lazy relations.
        /// </summary>
        private static readonly Lazy<IEnumerable<IEntityRelation>> LazyRelations =
            new Lazy<IEnumerable<IEntityRelation>>(
                () =>
                new EntityRelationSet<ComplexFlatRow>().InnerJoin<SubRow>(row => row.FakeSubEntityId, entity => entity.FakeSubEntityId)
                    .InnerJoin<SubRow, SubSubRow>(row => row.FakeSubSubEntityId, row => row.FakeSubSubEntityId)
                    .InnerJoin<MultiReferenceRow>(row => row.CreatedByFakeMultiReferenceEntityId, row => row.FakeMultiReferenceEntityId)
                    .InnerJoin<MultiReferenceRow>(row => row.ModifiedByFakeMultiReferenceEntityId, row => row.FakeMultiReferenceEntityId)
                    .LeftJoin<DependentRow>(row => row.FakeDependentEntityId, row => row.FakeDependentEntityId)
                    .Relations);

        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<ComplexFlatRow, object>[] ComparisonProperties =
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
                ////item => item.SubSubEntity,
                ////item => item.SubEntity,
                ////item => item.DependentEntity,
                ////item => item.CreatedBy,
                ////item => item.ModifiedBy
            };

        /// <summary>
        /// Gets or sets the fake complex entity id.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        [RelatedEntity(typeof(SubRow), true)]
        public string FakeSubEntityUniqueName { get; set; }

        /// <summary>
        /// Gets or sets the fake sub description.
        /// </summary>
        [RelatedEntity(typeof(SubRow), true)]
        public string FakeSubEntityDescription { get; set; }

        // Here we do not need to alias the name because it is unique within the selection.

        /// <summary>
        /// Gets or sets the unique other id.
        /// </summary>
        [RelatedEntity(typeof(SubRow))]
        public short FakeSubEntityUniqueOtherId { get; set; }

        /// <summary>
        /// Gets or sets the fake sub sub entity id.
        /// </summary>
        [RelatedEntity(typeof(SubRow))]
        public int FakeSubSubEntityId { get; set; }

        /// <summary>
        /// Gets or sets the fake sub sub unique name.
        /// </summary>
        [RelatedEntity(typeof(SubSubRow), true)]
        public string FakeSubSubEntityUniqueName { get; set; }

        /// <summary>
        /// Gets or sets the fake sub sub description.
        /// </summary>
        [RelatedEntity(typeof(SubSubRow), true)]
        public string FakeSubSubEntityDescription { get; set; }

        /// <summary>
        /// Gets or sets the created by fake multi reference entity id.
        /// </summary>
        [RelatedEntity(typeof(MultiReferenceRow), true, "CreatedBy")]
        public int CreatedByFakeMultiReferenceEntityId { get; set; }

        /// <summary>
        /// Gets or sets the created by unique name.
        /// </summary>
        [RelatedEntity(typeof(MultiReferenceRow), true, "CreatedBy")]
        public string CreatedByUniqueName { get; set; }

        /// <summary>
        /// Gets or sets the created by description.
        /// </summary>
        [RelatedEntity(typeof(MultiReferenceRow), true, "CreatedBy")]
        public string CreatedByDescription { get; set; }

        /// <summary>
        /// Gets or sets the creation time.
        /// </summary>
        public DateTimeOffset CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the modified by fake multi reference entity id.
        /// </summary>
        [RelatedEntity(typeof(MultiReferenceRow), true, "ModifiedBy")]
        public int ModifiedByFakeMultiReferenceEntityId { get; set; }

        /// <summary>
        /// Gets or sets the modified by unique name.
        /// </summary>
        [RelatedEntity(typeof(MultiReferenceRow), true, "ModifiedBy")]
        public string ModifiedByUniqueName { get; set; }

        /// <summary>
        /// Gets or sets the modified by description.
        /// </summary>
        [RelatedEntity(typeof(MultiReferenceRow), true, "ModifiedBy")]
        public string ModifiedByDescription { get; set; }

        /// <summary>
        /// Gets or sets the modified time.
        /// </summary>
        public DateTimeOffset ModifiedTime { get; set; }

        /// <summary>
        /// Gets or sets the fake dependent entity id.
        /// </summary>
        [RelatedEntity(typeof(DependentRow))]
        public int? FakeDependentEntityId { get; set; }

        /// <summary>
        /// Gets or sets the dependent integer value.
        /// </summary>
        [RelatedEntity(typeof(DependentRow), true)]
        public int? FakeDependentEntityDependentIntegerValue { get; set; }

        /// <summary>
        /// Gets or sets the dependent time value.
        /// </summary>
        [RelatedEntity(typeof(DependentRow), true)]
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
        public static bool operator ==(ComplexFlatRow valueA, ComplexFlatRow valueB)
        {
            return EqualityComparer<ComplexFlatRow>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(ComplexFlatRow valueA, ComplexFlatRow valueB)
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
        public bool Equals(ComplexFlatRow other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }
    }
}
