// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueState.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Represents a value state for an attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Startitecture.Core;

    /// <summary>
    /// Represents a value state for an attribute.
    /// </summary>
    public class ValueState : IEquatable<ValueState>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<ValueState, object>[] ComparisonProperties =
            {
                item => item.AttributeLocation,
                item => item.Value
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueState"/> class.
        /// </summary>
        /// <param name="attributeLocation">
        /// The attribute location.
        /// </param>
        /// <param name="value">
        /// The value of the attribute.
        /// </param>
        public ValueState([NotNull] AttributeLocation attributeLocation, object value)
        {
            this.AttributeLocation = attributeLocation ?? throw new ArgumentNullException(nameof(attributeLocation));
            this.Value = value;
        }

        /// <summary>
        /// Gets the attribute location.
        /// </summary>
        public AttributeLocation AttributeLocation { get; }

        /// <summary>
        /// Gets the value of the attribute.
        /// </summary>
        public object Value { get; }

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
        public static bool operator ==(ValueState valueA, ValueState valueB)
        {
            return EqualityComparer<ValueState>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(ValueState valueA, ValueState valueB)
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
            return $"{this.AttributeLocation}='{this.Value}'";
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
        /// <param name="obj">
        /// The object to compare with the current object.
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
        public bool Equals(ValueState other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }
    }
}