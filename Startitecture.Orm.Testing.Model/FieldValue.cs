// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldValue.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    using Startitecture.Core;

    /// <summary>
    /// The field value.
    /// </summary>
    public class FieldValue : IEquatable<FieldValue>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<FieldValue, object>[] ComparisonProperties =
            {
                item => item.Field,
                item => item.LastModifiedTime,
                item => item.LastModifiedBy,
                item => item.Elements
            };

        /// <summary>
        /// The elements.
        /// </summary>
        private readonly List<FieldValueElement> elements = new List<FieldValueElement>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldValue"/> class.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        public FieldValue([NotNull] Field field)
        {
            this.Field = field ?? throw new ArgumentNullException(nameof(field));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldValue"/> class.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <param name="fieldValueId">
        /// The field value ID.
        /// </param>
        public FieldValue([NotNull] Field field, long fieldValueId)
            : this(field)
        {
            this.FieldValueId = fieldValueId;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="FieldValue"/> class from being created.
        /// </summary>
        private FieldValue()
        {
        }

        /// <summary>
        /// Gets the field value id.
        /// </summary>
        public long? FieldValueId { get; private set; }

        /// <summary>
        /// Gets the field.
        /// </summary>
        public Field Field { get; private set; }

        /// <summary>
        /// Gets the last modified by.
        /// </summary>
        public DomainIdentity LastModifiedBy { get; private set; }

        /// <summary>
        /// Gets the last modified time.
        /// </summary>
        public DateTimeOffset LastModifiedTime { get; private set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public IEnumerable<FieldValueElement> Elements => this.elements;

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
        public static bool operator ==(FieldValue valueA, FieldValue valueB)
        {
            return EqualityComparer<FieldValue>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(FieldValue valueA, FieldValue valueB)
        {
            return !(valueA == valueB);
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
        public bool Equals(FieldValue other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var elementString = string.Join(";", this.elements.OrderBy(element => element.Order).Select(element => $"'{element}'"));
            return $"{this.Field} = {elementString}";
        }

        #endregion

        /// <summary>
        /// Sets the value of the field.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="identity">
        /// The identity setting the value.
        /// </param>
        /// <returns>
        /// The current <see cref="FieldValue"/>.
        /// </returns>
        public FieldValue Set(object value, DomainIdentity identity)
        {
            this.elements.Clear();

            if (value is ICollection list)
            {
                foreach (var item in list)
                {
                    var element = new FieldValueElement(item);
                    this.elements.Add(element);
                    element.AddToValue(this);
                }
            }
            else
            {
                var element = new FieldValueElement(value);
                this.elements.Add(element);
                element.AddToValue(this);
            }

            this.LastModifiedBy = identity;
            this.LastModifiedTime = DateTimeOffset.Now;
            return this;
        }

        /// <summary>
        /// Loads value elements into the value.
        /// </summary>
        /// <param name="valueElements">
        /// The elements to load.
        /// </param>
        public void Load([NotNull] IEnumerable<FieldValueElement> valueElements)
        {
            if (valueElements == null)
            {
                throw new ArgumentNullException(nameof(valueElements));
            }

            this.elements.Clear();

            foreach (var item in valueElements.OrderBy(element => element.Order))
            {
                var element = new FieldValueElement(item.Element, item.FieldValueElementId.GetValueOrDefault());
                this.elements.Add(element);
                element.AddToValue(this);
            }
        }
    }
}