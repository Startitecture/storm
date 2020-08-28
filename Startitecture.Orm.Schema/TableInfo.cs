// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableInfo.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Used by IMapper to override table bindings for an object.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Schema
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Core;

    /// <summary>
    /// Used by IMapper to override table bindings for an object.
    /// </summary>
    [Obsolete("TableInfo is only used by DatabaseType and its inheritors, which are deprecated.")]
    public class TableInfo : IEquatable<TableInfo>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<TableInfo, object>[] ComparisonProperties =
            {
                item => item.TableName,
                item => item.AutoIncrement,
                item => item.PrimaryKey,
                item => item.SequenceName
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="TableInfo"/> class.
        /// </summary>
        /// <param name="tableName">
        /// The table name.
        /// </param>
        public TableInfo(string tableName)
            : this(tableName, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableInfo"/> class.
        /// </summary>
        /// <param name="tableName">
        /// The table name.
        /// </param>
        /// <param name="sequenceName">
        /// The sequence name.
        /// </param>
        public TableInfo(string tableName, string sequenceName)
        {
            this.TableName = tableName;
            this.SequenceName = sequenceName;
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether the primary key column is auto-incrementing.
        /// </summary>
        public bool AutoIncrement { get; set; }

        /// <summary>
        /// Gets or sets the name of the primary key column of the table.
        /// </summary>
        public string PrimaryKey { get; set; }

        /// <summary>
        /// Gets the name of the sequence used for auto-incrementing Oracle primary key fields.
        /// </summary>
        public string SequenceName { get; private set; }

        /// <summary>
        /// Gets the database table name.
        /// </summary>
        public string TableName { get; private set; }

        #endregion

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
        public static bool operator ==(TableInfo valueA, TableInfo valueB)
        {
            return EqualityComparer<TableInfo>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(TableInfo valueA, TableInfo valueB)
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
            return this.TableName;
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
        public bool Equals(TableInfo other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion
    }
}