﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumeratedValue.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Core;

    /// <summary>
    /// The enumerated value.
    /// </summary>
    public class EnumeratedValue : IEquatable<EnumeratedValue>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<EnumeratedValue, object>[] ComparisonProperties =
            {
                item => item.EnumeratedValueId,
                item => item.Name,
                item => item.SortOrder
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumeratedValue"/> class.
        /// </summary>
        /// <param name="enumeratedValueId">
        /// The enumerated value ID.
        /// </param>
        public EnumeratedValue(int enumeratedValueId)
        {
            this.EnumeratedValueId = enumeratedValueId;
        }

        /// <summary>
        /// Gets the enumerated value ID.
        /// </summary>
        public int EnumeratedValueId { get; private set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        public int SortOrder { get; set; }

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
        public static bool operator ==(EnumeratedValue valueA, EnumeratedValue valueB)
        {
            return EqualityComparer<EnumeratedValue>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(EnumeratedValue valueA, EnumeratedValue valueB)
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
        public bool Equals(EnumeratedValue other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }
    }
}