﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyComparisonResult.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Contains the result of a property comparison.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Core
{
    using System;
    using System.Globalization;

    using Startitecture.Resources;

    /// <summary>
    /// Contains the result of a property comparison.
    /// </summary>
    public struct PropertyComparisonResult : IEquatable<PropertyComparisonResult>
    {
        /// <summary>
        /// The to string format.
        /// </summary>
        private const string ToStringFormat = "{0}: '{1}' -> '{2}'";

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyComparisonResult"/> struct.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="originalValue">
        /// The original value.
        /// </param>
        /// <param name="newValue">
        /// The new value.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="propertyName"/> is null or whitespace.
        /// </exception>
        public PropertyComparisonResult(string propertyName, object originalValue, object newValue)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(propertyName));
            }

            this.PropertyName = propertyName;
            this.OriginalValue = originalValue;
            this.NewValue = newValue;
        }

        #region Public Properties

        /// <summary>
        /// Gets the property name.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Gets the original value.
        /// </summary>
        public object OriginalValue { get; }

        /// <summary>
        /// Gets the new value.
        /// </summary>
        public object NewValue { get; }

        #endregion

        /// <summary>
        /// Indicates whether two objects of the same type are equal.
        /// </summary>
        /// <param name="left">
        /// The first object.
        /// </param>
        /// <param name="right">
        /// The second object.
        /// </param>
        /// <returns>
        /// <c>true</c> if the objects are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(PropertyComparisonResult left, PropertyComparisonResult right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Indicates whether two objects of the same type are not equal.
        /// </summary>
        /// <param name="left">
        /// The first object.
        /// </param>
        /// <param name="right">
        /// The second object.
        /// </param>
        /// <returns>
        /// <c>true</c> if the objects are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(PropertyComparisonResult left, PropertyComparisonResult right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="String"/> containing a fully qualified type name.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, ToStringFormat, this.PropertyName, this.OriginalValue, this.NewValue);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            return obj is PropertyComparisonResult result && this.Equals(result);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(PropertyComparisonResult other)
        {
            return Evaluate.Equals(this, other, result => result.PropertyName, result => result.OriginalValue, result => result.NewValue);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return Evaluate.GenerateHashCode(this.PropertyName, this.OriginalValue, this.NewValue);
        }
    }
}