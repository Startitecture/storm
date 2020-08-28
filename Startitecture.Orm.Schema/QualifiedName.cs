// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QualifiedName.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Represents a qualified name for a schema entity or attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Schema
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Core;

    /// <summary>
    /// Represents a qualified name for a schema entity or attribute.
    /// </summary>
    public struct QualifiedName : IEquatable<QualifiedName>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<QualifiedName, object>[] ComparisonProperties =
            {
                item => item.Attribute,
                item => item.Entity,
                item => item.Schema,
                item => item.Database
            };

        /// <summary>
        /// The to string text.
        /// </summary>
        private readonly string toString;

        /// <summary>
        /// Initializes a new instance of the <see cref="QualifiedName"/> struct.
        /// </summary>
        /// <param name="database">
        /// The database portion of the name.
        /// </param>
        /// <param name="schema">
        /// The schema portion of the name.
        /// </param>
        /// <param name="entity">
        /// The entity portion of the name.
        /// </param>
        /// <param name="attribute">
        /// The attribute portion of the name.
        /// </param>
        public QualifiedName(string database, string schema, string entity, string attribute)
        {
            this.toString = string.Empty;
            this.Database = database;

            if (string.IsNullOrWhiteSpace(database) == false)
            {
                this.toString = database;
            }

            this.Schema = schema;

            if (string.IsNullOrWhiteSpace(schema) == false)
            {
                this.toString = string.IsNullOrWhiteSpace(this.toString) ? string.Concat(this.toString, ".", schema) : schema;
            }

            this.Entity = entity;

            if (string.IsNullOrWhiteSpace(entity) == false)
            {
                this.toString = string.IsNullOrWhiteSpace(this.toString) ? string.Concat(this.toString, ".", entity) : entity;
            }

            this.Attribute = attribute;

            if (string.IsNullOrWhiteSpace(attribute) == false)
            {
                this.toString = string.IsNullOrWhiteSpace(this.toString) ? string.Concat(this.toString, ".", attribute) : attribute;
            }
        }

        /// <summary>
        /// Gets the database portion of the name.
        /// </summary>
        public string Database { get; }

        /// <summary>
        /// Gets the schema portion of the name.
        /// </summary>
        public string Schema { get; }

        /// <summary>
        /// Gets the entity portion of the name.
        /// </summary>
        public string Entity { get; }

        /// <summary>
        /// Gets the attribute portion of the name.
        /// </summary>
        public string Attribute { get; }

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
        public static bool operator ==(QualifiedName valueA, QualifiedName valueB)
        {
            return EqualityComparer<QualifiedName>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(QualifiedName valueA, QualifiedName valueB)
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
            return this.toString;
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
        public bool Equals(QualifiedName other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion
    }
}