// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldValueElement.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    using Startitecture.Core;

    /// <summary>
    /// The field value element.
    /// </summary>
    public class FieldValueElement : IEquatable<FieldValueElement>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<FieldValueElement, object>[] ComparisonProperties =
            {
                item => item.Element,
                item => item.Order
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldValueElement"/> class.
        /// </summary>
        /// <param name="element">
        /// The value element.
        /// </param>
        public FieldValueElement(object element)
        {
            this.Element = element;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldValueElement"/> class.
        /// </summary>
        /// <param name="element">
        /// The value element.
        /// </param>
        /// <param name="fieldValueElementId">
        /// The field value element ID.
        /// </param>
        public FieldValueElement(object element, long fieldValueElementId)
            : this(element)
        {
            this.FieldValueElementId = fieldValueElementId;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="FieldValueElement"/> class from being created.
        /// </summary>
        private FieldValueElement()
        {
        }

        /// <summary>
        /// Gets the field value element ID.
        /// </summary>
        public long? FieldValueElementId { get; private set; }

        /// <summary>
        /// Gets the value element.
        /// </summary>
        public object Element { get; }

        /// <summary>
        /// Gets the field value.
        /// </summary>
        public FieldValue FieldValue { get; private set; }

        /// <summary>
        /// Gets the order.
        /// </summary>
        public int Order { get; private set; }

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
        public static bool operator ==(FieldValueElement valueA, FieldValueElement valueB)
        {
            return EqualityComparer<FieldValueElement>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(FieldValueElement valueA, FieldValueElement valueB)
        {
            return !(valueA == valueB);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.Element}";
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
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public bool Equals(FieldValueElement other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        /// <summary>
        /// Adds the element to a value.
        /// </summary>
        /// <param name="fieldValue">
        /// The field value to add this element to.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fieldValue"/> is null.
        /// </exception>
        internal void AddToValue([NotNull] FieldValue fieldValue)
        {
            if (fieldValue == null)
            {
                throw new ArgumentNullException(nameof(fieldValue));
            }

            this.Order = fieldValue.Elements.Count();
            this.FieldValue = fieldValue;
        }
    }
}