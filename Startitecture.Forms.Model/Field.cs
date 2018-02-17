// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Field.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Forms.Model
{
    using System;
    using System.Collections.Generic;

    using SAF.StringResources;

    using Startitecture.Common.Model;
    using Startitecture.Core;

    /// <summary>
    /// The field.
    /// </summary>
    public class Field : IEquatable<Field>, IComparable, IComparable<Field>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<Field, object>[] ComparisonProperties =
            {
                item => item.DataSlice,
                item => item.Name,
                item => item.FieldType,
                item => item.ValueType,
                item => item.Label
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="Field"/> class.
        /// </summary>
        /// <param name="dataSlice">
        /// The data slice that contains the field.
        /// </param>
        /// <param name="fieldType">
        /// The field type.
        /// </param>
        /// <param name="valueType">
        /// The value type.
        /// </param>
        /// <param name="name">
        /// The name of the field.
        /// </param>
        public Field(DataSlice dataSlice, FieldType fieldType, ValueType valueType, string name)
            : this(dataSlice, fieldType, valueType, name, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Field"/> class.
        /// </summary>
        /// <param name="dataSlice">
        /// The data slice that contains the field.
        /// </param>
        /// <param name="fieldType">
        /// The field type.
        /// </param>
        /// <param name="valueType">
        /// The value type.
        /// </param>
        /// <param name="name">
        /// The name of the field.
        /// </param>
        /// <param name="fieldId">
        /// The field ID.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dataSlice"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> is null or whitespace.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="fieldType"/> or <paramref name="valueType"/> are set to an unknown value.
        /// </exception>
        public Field(DataSlice dataSlice, FieldType fieldType, ValueType valueType, string name, int? fieldId)
        {
            if (dataSlice == null)
            {
                throw new ArgumentNullException(nameof(dataSlice));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(name));
            }

            if (fieldType == FieldType.Unknown || Enum.IsDefined(typeof(FieldType), fieldType) == false)
            {
                throw new ArgumentOutOfRangeException(nameof(fieldType), fieldType, ErrorMessages.EnumerationMustBeKnownValue);
            }

            if (valueType == ValueType.Unknown || Enum.IsDefined(typeof(ValueType), valueType) == false)
            {
                throw new ArgumentOutOfRangeException(nameof(valueType), valueType, ErrorMessages.EnumerationMustBeKnownValue);
            }

            this.DataSlice = dataSlice;
            this.FieldType = fieldType;
            this.ValueType = valueType;
            this.Name = name;
            this.FieldId = fieldId;
        }

        /// <summary>
        /// Gets the field ID.
        /// </summary>
        public int? FieldId { get; private set; }

        /// <summary>
        /// Gets the data slice.
        /// </summary>
        public DataSlice DataSlice { get; private set; }

        /// <summary>
        /// Gets the field type.
        /// </summary>
        public FieldType FieldType { get; private set; }

        /// <summary>
        /// Gets the value type.
        /// </summary>
        public ValueType ValueType { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string Label { get; set; }

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
        public static bool operator ==(Field valueA, Field valueB)
        {
            return EqualityComparer<Field>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(Field valueA, Field valueB)
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
        public static bool operator <(Field valueA, Field valueB)
        {
            return Comparer<Field>.Default.Compare(valueA, valueB) < 0;
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
        public static bool operator >(Field valueA, Field valueB)
        {
            return Comparer<Field>.Default.Compare(valueA, valueB) > 0;
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
        /// <exception cref="T:System.ArgumentException">
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
        public int CompareTo(Field other)
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
        public bool Equals(Field other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion
    }
}