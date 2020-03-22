// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldPlacement.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The field placement.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Testing.Model.PM;

    /// <summary>
    /// The field placement.
    /// </summary>
    public class FieldPlacement : IEquatable<FieldPlacement>,
                                  IComparable<FieldPlacement>,
                                  IComparable,
                                  IOrderedElement
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<FieldPlacement, object>[] ComparisonProperties =
            {
                item => item.Order,
                item => item.UnifiedField,
                item => item.LayoutSection,
                item => item.CssStyle
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldPlacement"/> class.
        /// </summary>
        /// <param name="unifiedField">
        /// The unified field.
        /// </param>
        public FieldPlacement([NotNull] UnifiedField unifiedField)
        {
            if (unifiedField == null)
            {
                throw new ArgumentNullException(nameof(unifiedField));
            }

            this.UnifiedField = unifiedField;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldPlacement"/> class.
        /// </summary>
        /// <param name="unifiedField">
        /// The unified field.
        /// </param>
        /// <param name="layoutSection">
        /// The layout section.
        /// </param>
        /// <param name="fieldPlacementId">
        /// The field placement ID.
        /// </param>
        public FieldPlacement(
            [NotNull] UnifiedField unifiedField,
            [NotNull] LayoutSection layoutSection,
            int? fieldPlacementId)
        {
            if (layoutSection == null)
            {
                throw new ArgumentNullException(nameof(layoutSection));
            }

            if (unifiedField == null)
            {
                throw new ArgumentNullException(nameof(unifiedField));
            }

            this.LayoutSection = layoutSection;
            this.UnifiedField = unifiedField;
            this.FieldPlacementId = fieldPlacementId;
        }

        /// <summary>
        /// Gets the field placement ID.
        /// </summary>
        public int? FieldPlacementId { get; private set; }

        /// <summary>
        /// Gets the layout section ID.
        /// </summary>
        public LayoutSection LayoutSection { get; private set; }

        /// <summary>
        /// Gets the layout section ID.
        /// </summary>
        public int? LayoutSectionId
        {
            get
            {
                return this.LayoutSection?.LayoutSectionId;
            }
        }

        /// <summary>
        /// Gets the unified field.
        /// </summary>
        public UnifiedField UnifiedField { get; private set; }

        /// <summary>
        /// Gets the unified field ID.
        /// </summary>
        public int? UnifiedFieldId
        {
            get
            {
                return this.UnifiedField?.UnifiedFieldId;
            }
        }

        /// <summary>
        /// Gets or sets the CSS style.
        /// </summary>
        public string CssStyle { get; set; }

        /// <summary>
        /// Gets the order of the field placement in the section.
        /// </summary>
        public short Order { get; private set; }

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
        public static bool operator ==(FieldPlacement valueA, FieldPlacement valueB)
        {
            return EqualityComparer<FieldPlacement>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(FieldPlacement valueA, FieldPlacement valueB)
        {
            return !(valueA == valueB);
        }

        /// <summary>
        /// Determines if the first value is less than the second value.
        /// </summary>
        /// <param name="valueA">
        /// The first value to compare.
        /// </param>
        /// <param name="valueB">
        /// The second value to compare.
        /// </param>
        /// <returns>
        /// <c>true</c> if the first value is less than the second value; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator <(FieldPlacement valueA, FieldPlacement valueB)
        {
            return Comparer<FieldPlacement>.Default.Compare(valueA, valueB) < 0;
        }

        /// <summary>
        /// Determines if the first value is greater than the second value.
        /// </summary>
        /// <param name="valueA">
        /// The first value to compare.
        /// </param>
        /// <param name="valueB">
        /// The second value to compare.
        /// </param>
        /// <returns>
        /// <c>true</c> if the first value is greater than the second value; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator >(FieldPlacement valueA, FieldPlacement valueB)
        {
            return Comparer<FieldPlacement>.Default.Compare(valueA, valueB) > 0;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates
        /// whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings:
        /// Value Meaning Less than zero This instance precedes <paramref name="obj"/> in the sort order. Zero This instance
        /// occurs in the same position in the sort order as <paramref name="obj"/>. Greater than zero This instance follows
        /// <paramref name="obj"/> in the sort order.
        /// </returns>
        /// <param name="obj">
        /// An object to compare with this instance.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="obj"/> is not the same type as this instance.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public int CompareTo(object obj)
        {
            return Evaluate.Compare(this, obj);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following
        /// meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This
        /// object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public int CompareTo(FieldPlacement other)
        {
            return Evaluate.Compare(this, other, ComparisonProperties);
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
            return this.UnifiedField?.Name ?? this.GetType().Name;
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
        public bool Equals(FieldPlacement other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion

        /// <summary>
        /// Sets order of the current element.
        /// </summary>
        /// <param name="searcher">
        /// The ordered element searcher.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="searcher"/> is null.
        /// </exception>
        /// <exception cref="BusinessException">
        /// The current element is not in the searcher's list.
        /// </exception>
        public void SetOrder(IOrderedElementSearcher searcher)
        {
            if (searcher == null)
            {
                throw new ArgumentNullException(nameof(searcher));
            }

            var order = searcher.GetOrder(this);

            if (order < 0)
            {
                throw new BusinessException(this, FieldsMessages.OrderedElementOrderIsNegative);
            }

            this.Order = order;
        }

        /// <summary>
        /// Adds the current field placement to a layout section.
        /// </summary>
        /// <param name="section">
        /// The section to add the current field placement to.
        /// </param>
        /// <param name="order">
        /// The order of the placement in the current section.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="section"/> is null.
        /// </exception>
        internal void AddToLayoutSection([NotNull] LayoutSection section, short order)
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            this.LayoutSection = section;
            this.Order = order;
        }
    }
}