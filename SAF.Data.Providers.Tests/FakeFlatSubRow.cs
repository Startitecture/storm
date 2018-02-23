// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeFlatSubRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The fake flat sub row.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Core;
    using Startitecture.Orm.Model;

    /// <summary>
    /// The fake flat sub row.
    /// </summary>
    public class FakeFlatSubRow : FakeSubRow, IEquatable<FakeFlatSubRow>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<FakeFlatSubRow, object>[] ComparisonProperties =
            {
                item => item.UniqueName,
                item => item.Description,
                item => item.FakeSubSubEntityDescription,
                item => item.FakeSubSubEntityUniqueName,
                item => item.UniqueOtherId
            };

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
        public static bool operator ==(FakeFlatSubRow valueA, FakeFlatSubRow valueB)
        {
            return EqualityComparer<FakeFlatSubRow>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(FakeFlatSubRow valueA, FakeFlatSubRow valueB)
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
        public bool Equals(FakeFlatSubRow other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

    }
}
