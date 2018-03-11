// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainAggregateRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Entities
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Core;
    using Startitecture.Orm.Model;

    /// <summary>
    /// The domain aggregate row.
    /// </summary>
    public partial class DomainAggregateRow : IEquatable<DomainAggregateRow>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<DomainAggregateRow, object>[] ComparisonProperties =
            {
                item => item.AggregateOption,
                item => item.CategoryAttribute,
                item => item.CategoryAttributeId,
                item => item.CreatedBy,
                item => item.CreatedByDomainIdentityId,
                item => item.CreatedTime,
                item => item.Description,
                item => item.LastModifiedBy,
                item => item.LastModifiedByDomainIdentityId,
                item => item.LastModifiedTime,
                item => item.Name,
                item => item.OtherAggregate,
                item => item.SubContainer,
                item => item.SubContainerId,
                item => item.Template,
                item => item.TemplateId,
            };

        /// <summary>
        /// Gets or sets the sub container.
        /// </summary>
        [Relation]
        public SubContainerRow SubContainer { get; set; }

        /// <summary>
        /// Gets or sets the aggregate option.
        /// </summary>
        [Relation]
        public AggregateOptionRow AggregateOption { get; set; }

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        [Relation]
        public TemplateRow Template { get; set; }

        /// <summary>
        /// Gets or sets the category attribute.
        /// </summary>
        [Relation]
        public CategoryAttributeRow CategoryAttribute { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        [Relation]
        public DomainIdentityRow CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the last modified by.
        /// </summary>
        [Relation]
        public DomainIdentityRow LastModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the other aggregate.
        /// </summary>
        [Relation]
        public OtherAggregateRow OtherAggregate { get; set; }

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
        public static bool operator ==(DomainAggregateRow valueA, DomainAggregateRow valueB)
        {
            return EqualityComparer<DomainAggregateRow>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(DomainAggregateRow valueA, DomainAggregateRow valueB)
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
        public bool Equals(DomainAggregateRow other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion
    }
}