// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OverriddenColumnNameRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Core;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The overridden column name row.
    /// </summary>
    [TableName("overridden_column_name")]
    [ExplicitColumns]
    [PrimaryKey("ocn_id", AutoIncrement = true)]
    public class OverriddenColumnNameRow : IEquatable<OverriddenColumnNameRow>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<OverriddenColumnNameRow, object>[] ComparisonProperties =
            {
                item => item.Name,
                item => item.OverriddenColumnNameId,
                item => item.Description,
                item => item.EntryTime,
                item => item.RelatedRowId,
                item => item.RelatedRowName
            };

        /// <summary>
        /// Gets or sets the overridden column name id.
        /// </summary>
        [Column("ocn_id")]
        public int OverriddenColumnNameId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Column("ocn_name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [Column("ocn_description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the entry time.
        /// </summary>
        [Column("ocn_entry_time")]
        public DateTimeOffset EntryTime { get; set; }

        /// <summary>
        /// Gets or sets the related row id.
        /// </summary>
        [Column("ocn_rr_id")]
        public int RelatedRowId { get; set; }

        /// <summary>
        /// Gets or sets the related row name.
        /// </summary>
        [RelatedEntity(typeof(RelatedRow), false, null, PhysicalName = "rr_name")]
        public string RelatedRowName { get; set; }

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
        public static bool operator ==(OverriddenColumnNameRow valueA, OverriddenColumnNameRow valueB)
        {
            return EqualityComparer<OverriddenColumnNameRow>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(OverriddenColumnNameRow valueA, OverriddenColumnNameRow valueB)
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
        public bool Equals(OverriddenColumnNameRow other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion

    }
}