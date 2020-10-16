// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RaisedOverriddenColumnNameRow.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Startitecture.Core;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The raised overridden column name row.
    /// </summary>
    [Table("overridden_column_name")]
    public class RaisedOverriddenColumnNameRow : IEquatable<RaisedOverriddenColumnNameRow>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<RaisedOverriddenColumnNameRow, object>[] ComparisonProperties =
            {
                item => item.OverriddenColumnNameId,
                item => item.Name,
                item => item.Description,
                item => item.EntryTime,
                item => item.RelatedRowId,
                item => item.RelatedRow
            };

        /// <summary>
        /// Gets or sets the overridden column name id.
        /// </summary>
        [Column("ocn_id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        /// Gets or sets the related row.
        /// </summary>
        [Relation]
        public RelatedRow RelatedRow { get; set; }

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
        public static bool operator ==(RaisedOverriddenColumnNameRow valueA, RaisedOverriddenColumnNameRow valueB)
        {
            return EqualityComparer<RaisedOverriddenColumnNameRow>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(RaisedOverriddenColumnNameRow valueA, RaisedOverriddenColumnNameRow valueB)
        {
            return !(valueA == valueB);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
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
        public bool Equals(RaisedOverriddenColumnNameRow other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }
    }
}