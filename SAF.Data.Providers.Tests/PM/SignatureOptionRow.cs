// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SignatureOptionRow.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The signature option row.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.PM
{
    using System;
    using System.Collections.Generic;

    using SAF.Core;

    /// <summary>
    /// The signature option row.
    /// </summary>
    public partial class SignatureOptionRow : IEquatable<SignatureOptionRow>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<SignatureOptionRow, object>[] ComparisonProperties =
            {
                item => item.AllowExternalSigners,
                item => item.EnableSignedDocumentRecipients,
                item => item.SignatureRejectionBehaviorId,
                item => item.SignatureTypeId
            };

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
        public static bool operator ==(SignatureOptionRow valueA, SignatureOptionRow valueB)
        {
            return EqualityComparer<SignatureOptionRow>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(SignatureOptionRow valueA, SignatureOptionRow valueB)
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
            return this.SignatureOptionId.ToString();
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
        public bool Equals(SignatureOptionRow other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion
    }
}
