// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeDependentRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Tests
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Core;
    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The fake dependent row.
    /// </summary>
    [TableName("FakeDependentEntity")]
    [PrimaryKey("FakeDependentEntityId", AutoIncrement = false)]
    public class FakeDependentRow : TransactionItemBase, IEquatable<FakeDependentRow>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<FakeDependentRow, object>[] ComparisonProperties =
            {
                item => item.DependentIntegerValue,
                item => item.DependentTimeValue
            };

        /// <summary>
        /// Gets or sets the fake dependent entity id.
        /// </summary>
        [Column]
        public int FakeDependentEntityId { get; set; }

        /// <summary>
        /// Gets or sets the dependent integer value.
        /// </summary>
        [Column]
        public int DependentIntegerValue { get; set; }

        /// <summary>
        /// Gets or sets the dependent time value.
        /// </summary>
        [Column]
        public DateTimeOffset DependentTimeValue { get; set; }

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
        public static bool operator ==(FakeDependentRow valueA, FakeDependentRow valueB)
        {
            return EqualityComparer<FakeDependentRow>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(FakeDependentRow valueA, FakeDependentRow valueB)
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
            return Convert.ToString(this.DependentIntegerValue);
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
        public bool Equals(FakeDependentRow other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

    }
}