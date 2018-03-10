// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelatedRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    using Startitecture.Core;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The related row.
    /// </summary>
    [TableName("related_row")]
    [ExplicitColumns]
    [PrimaryKey("rr_id", AutoIncrement = true)]
    public class RelatedRow : IEquatable<RelatedRow>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<RelatedRow, object>[] ComparisonProperties =
            {
                item => item.RelatedRowId,
                item => item.Name
            };

        /// <summary>
        /// Gets or sets the related row id.
        /// </summary>
        [Column("rr_id")]
        public int RelatedRowId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Column("rr_name")]
        public string Name { get; set; }

        #region Equality and Comparison Methods

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
        public static bool operator ==(RelatedRow valueA, RelatedRow valueB)
        {
            return EqualityComparer<RelatedRow>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(RelatedRow valueA, RelatedRow valueB)
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
            return this.Name;
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
        public bool Equals(RelatedRow other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion
    }
}