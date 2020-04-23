// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComplexRaisedRow.cs" company="Startitecture">
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
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The fake complex row.
    /// </summary>
    [Table("ComplexEntity", Schema = "dbo")]
    public class ComplexRaisedRow : EntityBase, IEquatable<ComplexRaisedRow>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<ComplexRaisedRow, object>[] ComparisonProperties =
            {
                item => item.UniqueName,
                item => item.CreationTime,
                item => item.Description,
                item => item.FakeEnumerationId,
                item => item.FakeOtherEnumerationId,
                item => item.FakeSubEntityId,
                item => item.ModifiedTime,
                item => item.SubEntity,
                item => item.DependentEntity,
                item => item.CreatedBy,
                item => item.ModifiedBy
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexRaisedRow"/> class. 
        /// </summary>
        public ComplexRaisedRow()
        {
            // All non-optional entities are created here. Not necessary but avoids having to create them with reflection
            this.ModifiedBy = new MultiReferenceRow();
            this.CreatedBy = new MultiReferenceRow();
            this.SubEntity = new SubRow();
        }

        /// <summary>
        /// Gets or sets the fake complex entity id.
        /// </summary>
        [Column(Order = 1)]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ComplexEntityId { get; set; }

        /// <summary>
        /// Gets or sets the unique name.
        /// </summary>
        [Column(Order = 2)]
        public string UniqueName { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [Column(Order = 3)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the fake sub entity id.
        /// </summary>
        [Column(Order = 4)]
        public int FakeSubEntityId { get; set; }

        /// <summary>
        /// Gets or sets the fake enumeration.
        /// </summary>
        [Column(Order = 5)]
        public int FakeEnumerationId { get; set; }

        /// <summary>
        /// Gets or sets the fake other enumeration.
        /// </summary>
        [Column(Order = 6)]
        public int FakeOtherEnumerationId { get; set; }

        /// <summary>
        /// Gets or sets the creation time.
        /// </summary>
        [Column(Order = 7)]
        public DateTimeOffset CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the modified time.
        /// </summary>
        [Column(Order = 8)]
        public DateTimeOffset ModifiedTime { get; set; }

        /// <summary>
        /// Gets or sets the created by fake multi reference entity ID.
        /// </summary>
        [Column(Order = 9)]
        public int CreatedByFakeMultiReferenceEntityId { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        [Relation]
        public MultiReferenceRow CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the modified by fake multi reference entity ID.
        /// </summary>
        [Column(Order = 10)]
        public int ModifiedByFakeMultiReferenceEntityId { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        [Relation]
        public MultiReferenceRow ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the fake sub entity.
        /// </summary>
        [Relation]
        public SubRow SubEntity { get; set; }

        /// <summary>
        /// Gets or sets the fake dependent entity.
        /// </summary>
        [Relation]
        public DependentRow DependentEntity { get; set; }

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
        public static bool operator ==(ComplexRaisedRow valueA, ComplexRaisedRow valueB)
        {
            return EqualityComparer<ComplexRaisedRow>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(ComplexRaisedRow valueA, ComplexRaisedRow valueB)
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
        public bool Equals(ComplexRaisedRow other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }
    }
}
