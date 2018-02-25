// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnifiedSourceValue.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The unified source value.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Startitecture.Core;

    /// <summary>
    /// The unified source value.
    /// </summary>
    public class UnifiedSourceValue : IEquatable<UnifiedSourceValue>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<UnifiedSourceValue, object>[] ComparisonProperties =
            {
                item => item.UnifiedField,
                item => item.Name,
                item => item.StringValue,
                item => item.Order,
                item => item.Caption,
                item => item.ToolTip
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="UnifiedSourceValue"/> class.
        /// </summary>
        /// <param name="unifiedField">
        /// The unified field.
        /// </param>
        public UnifiedSourceValue([NotNull] UnifiedField unifiedField)
            : this(unifiedField, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnifiedSourceValue"/> class.
        /// </summary>
        /// <param name="unifiedField">
        /// The unified field.
        /// </param>
        /// <param name="unifiedSourceValueId">
        /// The unified source value id.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="unifiedField"/> is null.
        /// </exception>
        public UnifiedSourceValue([NotNull] UnifiedField unifiedField, int? unifiedSourceValueId)
        {
            if (unifiedField == null)
            {
                throw new ArgumentNullException(nameof(unifiedField));
            }

            this.UnifiedField = unifiedField;
            this.UnifiedSourceValueId = unifiedSourceValueId;
        }

        /// <summary>
        /// Gets the unified source value id.
        /// </summary>
        public int? UnifiedSourceValueId { get; private set; }

        /// <summary>
        /// Gets the unified field.
        /// </summary>
        public UnifiedField UnifiedField { get; private set; }

        /// <summary>
        /// Gets the unified field id.
        /// </summary>
        public int? UnifiedFieldId
        {
            get
            {
                return this.UnifiedField?.UnifiedFieldId;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        public short Order { get; set; }

        /// <summary>
        /// Gets or sets the caption.
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Gets or sets the tool tip.
        /// </summary>
        public string ToolTip { get; set; }

        /// <summary>
        /// Gets or sets the string value.
        /// </summary>
        public string StringValue { get; set; }

        #region Equality Methods and Operators

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
        public static bool operator ==(UnifiedSourceValue valueA, UnifiedSourceValue valueB)
        {
            return EqualityComparer<UnifiedSourceValue>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(UnifiedSourceValue valueA, UnifiedSourceValue valueB)
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
            return $"{this.Name}:{this.StringValue}";
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
        public bool Equals(UnifiedSourceValue other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion
    }
}