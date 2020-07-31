// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The fake sub raised row.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The fake sub raised row.
    /// </summary>
    [Table("SubEntity", Schema = "dbo")]
    public class SubRow : EntityBase, IEquatable<SubRow>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<SubRow, object>[] ComparisonProperties =
            {
                item => item.UniqueName,
                item => item.Description,
                item => item.UniqueOtherId,
                item => item.SubSubEntity
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="SubRow"/> class.
        /// </summary>
        public SubRow()
        {
            this.SubSubEntity = new SubSubRow();
        }

        /// <summary>
        /// Gets or sets the fake sub sub entity.
        /// </summary>
        [Relation]
        public SubSubRow SubSubEntity { get; set; }

        /// <summary>
        /// Gets or sets the fake sub entity id.
        /// </summary>
        [Column(Order = 1)]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FakeSubEntityId { get; set; }

        /// <summary>
        /// Gets or sets the fake sub sub entity id.
        /// </summary>
        [Column(Order = 2)]
        public int FakeSubSubEntityId { get; set; }

        /// <summary>
        /// Gets or sets the unique name.
        /// </summary>
        [Column(Order = 3)]
        public string UniqueName { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [Column(Order = 4)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the unique other id.
        /// </summary>
        [Column(Order = 5)]
        public short UniqueOtherId { get; set; }

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
        public static bool operator ==(SubRow valueA, SubRow valueB)
        {
            return EqualityComparer<SubRow>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(SubRow valueA, SubRow valueB)
        {
            return !(valueA == valueB);
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
        public bool Equals(SubRow other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
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
    }
}
