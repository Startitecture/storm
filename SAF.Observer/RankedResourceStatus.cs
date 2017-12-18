// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RankedResourceStatus.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.Observer
{
    using System;

    using SAF.Core;

    /// <summary>
    /// Provides a rank for a cluster service resource.
    /// </summary>
    public class RankedResourceStatus : IEquatable<RankedResourceStatus>, IComparable, IComparable<RankedResourceStatus>
    {
        /// <summary>
        /// The to string format.
        /// </summary>
        private const string ToStringFormat = "[{0}] {1}";

        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<RankedResourceStatus, object>[] ComparisonProperties =
            {
                item => item.ResourceRank, 
                item => item.ResourceStatus
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="RankedResourceStatus"/> class.
        /// </summary>
        /// <param name="resourceRank">
        /// The resource rank.
        /// </param>
        /// <param name="resourceStatus">
        /// The resource status.
        /// </param>
        public RankedResourceStatus(int resourceRank, ResourceStatus resourceStatus)
        {
            this.ResourceStatus = resourceStatus;
            this.ResourceRank = resourceRank;
        }

        #region Public Properties

        /// <summary>
        /// Gets the resource rank.
        /// </summary>
        public int ResourceRank { get; private set; }

        /// <summary>
        /// Gets the resource status.
        /// </summary>
        public ResourceStatus ResourceStatus { get; private set; }

        #endregion

        #region Value Comparison

        /// <summary>
        /// Compares two values for equality.
        /// </summary>
        /// <param name="valueA">
        /// The first value.
        /// </param>
        /// <param name="valueB">
        /// The second value.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(RankedResourceStatus valueA, RankedResourceStatus valueB)
        {
            return Evaluate.Equals(valueA, valueB);
        }

        /// <summary>
        /// Compares two values for inequality.
        /// </summary>
        /// <param name="valueA">
        /// The first value.
        /// </param>
        /// <param name="valueB">
        /// The second value.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(RankedResourceStatus valueA, RankedResourceStatus valueB)
        {
            return !(valueA == valueB);
        }

        /// <summary>
        /// Determines if the first value is less than the second value.
        /// </summary>
        /// <param name="valueA">
        /// The first value.
        /// </param>
        /// <param name="valueB">
        /// The second value.
        /// </param>
        /// <returns>
        /// <c>true</c> if the first value is less than the second value; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator <(RankedResourceStatus valueA, RankedResourceStatus valueB)
        {
            return Evaluate.Compare(valueA, valueB) < 0;
        }

        /// <summary>
        /// Determines if the first value is greater than the second value.
        /// </summary>
        /// <param name="valueA">
        /// The first value.
        /// </param>
        /// <param name="valueB">
        /// The second value.
        /// </param>
        /// <returns>
        /// <c>true</c> if the first value is greater than the second value; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator >(RankedResourceStatus valueA, RankedResourceStatus valueB)
        {
            return Evaluate.Compare(valueA, valueB) > 0;
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="RankedResourceStatus"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return Evaluate.GenerateHashCode(this, ComparisonProperties);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="RankedResourceStatus"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="RankedResourceStatus"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// The <see cref="T:System.Object"/> to compare with the current <see cref="RankedResourceStatus"/>. 
        /// </param>
        /// <filterpriority>2</filterpriority>
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
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public bool Equals(RankedResourceStatus other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref name="obj"/>. Zero This instance is equal to <paramref name="obj"/>. Greater than zero This instance is greater than <paramref name="obj"/>. 
        /// </returns>
        /// <param name="obj">
        /// An object to compare with this instance. 
        /// </param>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="obj"/> is not the same type as this instance. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public int CompareTo(object obj)
        {
            return Evaluate.Compare(this, obj);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public int CompareTo(RankedResourceStatus other)
        {
            return Evaluate.Compare(this, other, ComparisonProperties);
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="RankedResourceStatus"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="RankedResourceStatus"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return String.Format(ToStringFormat, this.ResourceRank, this.ResourceStatus);
        }
    }
}