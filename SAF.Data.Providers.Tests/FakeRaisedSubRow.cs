// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeRaisedSubRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The fake sub raised row.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests.Models
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Core;
    using Startitecture.Orm.Model;

    /// <summary>
    /// The fake sub raised row.
    /// </summary>
    public class FakeRaisedSubRow : FakeSubRow, IEquatable<FakeRaisedSubRow>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<FakeRaisedSubRow, object>[] ComparisonProperties =
            {
                item => item.UniqueName,
                item => item.Description,
                item => item.UniqueOtherId,
                item => item.FakeSubSubEntity
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeRaisedSubRow"/> class.
        /// </summary>
        public FakeRaisedSubRow()
        {
            this.FakeSubSubEntity = new FakeSubSubRow();
        }

        /// <summary>
        /// Gets or sets the fake sub sub entity.
        /// </summary>
        [Relation]
        public FakeSubSubRow FakeSubSubEntity { get; set; }

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
        public static bool operator ==(FakeRaisedSubRow valueA, FakeRaisedSubRow valueB)
        {
            return EqualityComparer<FakeRaisedSubRow>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(FakeRaisedSubRow valueA, FakeRaisedSubRow valueB)
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
        public bool Equals(FakeRaisedSubRow other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }
    }
}
