// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolvedAttributeLocation.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Defines the location of a resolved attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Core;

    /// <summary>
    /// Defines the location of a resolved attribute.
    /// </summary>
    public struct ResolvedAttributeLocation : IEquatable<ResolvedAttributeLocation>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<ResolvedAttributeLocation, object>[] ComparisonProperties =
            {
                item => item.Schema,
                item => item.EntityName,
                item => item.AttributeName
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="ResolvedAttributeLocation"/> struct.
        /// </summary>
        /// <param name="schema">
        /// The schema.
        /// </param>
        /// <param name="entityName">
        /// The entity name.
        /// </param>
        /// <param name="attributeName">
        /// The attribute name.
        /// </param>
        public ResolvedAttributeLocation(string schema, string entityName, string attributeName)
        {
            this.Schema = schema;
            this.EntityName = entityName;
            this.AttributeName = attributeName;
        }

        /// <summary>
        /// Gets the schema.
        /// </summary>
        public string Schema { get; }

        /// <summary>
        /// Gets the entity name.
        /// </summary>
        public string EntityName { get; }

        /// <summary>
        /// Gets the attribute name.
        /// </summary>
        public string AttributeName { get; }

        #region Equality and Comparison Methods and Operators

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
        public static bool operator ==(ResolvedAttributeLocation valueA, ResolvedAttributeLocation valueB)
        {
            return EqualityComparer<ResolvedAttributeLocation>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(ResolvedAttributeLocation valueA, ResolvedAttributeLocation valueB)
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
            return $"{this.Schema}.{this.EntityName}.{this.AttributeName}";
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
        public bool Equals(ResolvedAttributeLocation other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion
    }
}