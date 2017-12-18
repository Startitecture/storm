// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnifiedFieldValue.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The unified field value.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    using SAF.Core;
    using SAF.Testing.Common;

    /// <summary>
    /// The unified field value.
    /// </summary>
    public class UnifiedFieldValue : IEquatable<UnifiedFieldValue>, ITrackChanges
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<UnifiedFieldValue, object>[] ComparisonProperties =
            {
                item => item.UnifiedField,
                item => item.LastModifiedBy,
                item => item.LastModifiedTime
            };

        /// <summary>
        /// The field values.
        /// </summary>
        private readonly List<object> fieldValues = new List<object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UnifiedFieldValue"/> class.
        /// </summary>
        /// <param name="unifiedField">
        /// The unified field.
        /// </param>
        public UnifiedFieldValue([NotNull] UnifiedField unifiedField)
        {
            if (unifiedField == null)
            {
                throw new ArgumentNullException(nameof(unifiedField));
            }

            this.UnifiedField = unifiedField;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnifiedFieldValue"/> class.
        /// </summary>
        /// <param name="unifiedField">
        /// The unified field.
        /// </param>
        /// <param name="lastModifiedBy">
        /// The last modified by person.
        /// </param>
        /// <param name="lastModifiedTime">
        /// The last modified time.
        /// </param>
        /// <param name="unifiedFieldValueId">
        /// The unified field value ID.
        /// </param>
        public UnifiedFieldValue(
            [NotNull] UnifiedField unifiedField,
            [NotNull] Person lastModifiedBy,
            DateTimeOffset lastModifiedTime,
            long? unifiedFieldValueId)
            : this(unifiedField)
        {
            if (unifiedField == null)
            {
                throw new ArgumentNullException(nameof(unifiedField));
            }

            if (lastModifiedBy == null)
            {
                throw new ArgumentNullException(nameof(lastModifiedBy));
            }

            this.LastModifiedBy = lastModifiedBy;
            this.LastModifiedTime = lastModifiedTime;
            this.UnifiedFieldValueId = unifiedFieldValueId;
        }

        /// <summary>
        /// Gets the unified field value ID.
        /// </summary>
        public long? UnifiedFieldValueId { get; private set; }

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
        /// Gets the field values.
        /// </summary>
        public IEnumerable<object> FieldValues
        {
            get
            {
                return this.fieldValues;
            }
        }

        /// <summary>
        /// Gets the last modified person.
        /// </summary>
        public Person LastModifiedBy { get; private set; }

        /// <summary>
        /// Gets the last modified person ID.
        /// </summary>
        public int? LastModifiedPersonId
        {
            get
            {
                return this.LastModifiedBy?.PersonId;
            }
        }

        /// <summary>
        /// Gets the last modified time.
        /// </summary>
        public DateTimeOffset LastModifiedTime { get; private set; }

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
        public static bool operator ==(UnifiedFieldValue valueA, UnifiedFieldValue valueB)
        {
            return EqualityComparer<UnifiedFieldValue>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(UnifiedFieldValue valueA, UnifiedFieldValue valueB)
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
            var values = string.Join(",", this.fieldValues);
            return $"{this.UnifiedField}:{values}";
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
            var propertyHash = Evaluate.GenerateHashCode(this, ComparisonProperties);
            var valuesHash = Evaluate.GenerateHashCode(this.FieldValues);

            return Evaluate.GenerateHashCode(propertyHash, valuesHash);
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
        public bool Equals(UnifiedFieldValue other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties) && other != null && this.FieldValues.SequenceEqual(other.FieldValues);
        }

        #endregion

        /// <summary>
        /// Updates change tracking for the current object.
        /// </summary>
        /// <param name="editor">
        /// The editor of the object.
        /// </param>
        public void UpdateChangeTracking(Person editor)
        {
            if (editor == null)
            {
                throw new ArgumentNullException(nameof(editor));
            }

            this.LastModifiedTime = DateTimeOffset.Now;
            this.LastModifiedBy = editor;
        }

        /// <summary>
        /// Loads the unified fields values.
        /// </summary>
        /// <param name="values">
        /// The values to load.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="values"/> is null.
        /// </exception>
        public void LoadValues([NotNull] params object[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            // Equivalent to setting a field to blank/null.
            if (values.Any() == false)
            {
                this.fieldValues.Clear();
                return;
            }

            // If the value is a source value then we need to select the string value from it, otherwise use the raw value.
            var unifiedSourceValues = values.OfType<UnifiedSourceValue>().ToList();
            var convertedValues = unifiedSourceValues.OrderBy(x => x.Order).Select(x => x.StringValue).Union(values.Except(unifiedSourceValues));

            var filteredValues = new List<object>();
            this.LoadUserSourcedValues(convertedValues, this.UnifiedField.UnifiedValueType, filteredValues);
            this.fieldValues.Clear();
            this.fieldValues.AddRange(filteredValues);
        }

        /// <summary>
        /// Sets the unified field values.
        /// </summary>
        /// <param name="person">
        /// The person setting the values.
        /// </param>
        /// <param name="values">
        /// The values to set.
        /// </param>
        public void SetValues([NotNull] Person person, [NotNull] params object[] values)
        {
            if (person == null)
            {
                throw new ArgumentNullException(nameof(person));
            }

            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            this.LastModifiedBy = person;
            this.LastModifiedTime = DateTimeOffset.Now;
            this.LoadValues(values);
        }

        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">
        /// The value to convert.
        /// </param>
        /// <param name="valueType">
        /// The unified value type.
        /// </param>
        /// <param name="converter">
        /// The converter to use for conversion.
        /// </param>
        /// <typeparam name="T">
        /// The type to convert to.
        /// </typeparam>
        /// <returns>
        /// The converted value as the specified type.
        /// </returns>
        /// <exception cref="BusinessException">
        /// The value could not be converted. The inner exception provides additional details.
        /// </exception>
        private static T ConvertValue<T>(object value, UnifiedValueType valueType, Func<object, T> converter)
        {
            T convertedValue;

            try
            {
                convertedValue = converter(value);
            }
            catch (FormatException ex)
            {
                var failureMessage = string.Format(FieldsMessages.ValueCannotBeConvertedToValueType, value, valueType);
                throw new BusinessException(value, failureMessage, ex);
            }
            catch (OverflowException ex)
            {
                var failureMessage = string.Format(FieldsMessages.ValueCannotBeConvertedToValueType, value, valueType);
                throw new BusinessException(value, failureMessage, ex);
            }
            catch (InvalidCastException ex)
            {
                var failureMessage = string.Format(FieldsMessages.ValueCannotBeConvertedToValueType, value, valueType);
                throw new BusinessException(value, failureMessage, ex);
            }

            return convertedValue;
        }

        /// <summary>
        /// Loads user sourced values into the specified filtered values list.
        /// </summary>
        /// <param name="values">
        /// The values to load.
        /// </param>
        /// <param name="valueType">
        /// The value type to load.
        /// </param>
        /// <param name="filteredValues">
        /// The filtered values list.
        /// </param>
        /// <exception cref="BusinessException">
        /// <paramref name="valueType"/> is unknown.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The enumeration value of <paramref name="valueType"/> is not one of the named values.
        /// </exception>
        private void LoadUserSourcedValues(IEnumerable<object> values, UnifiedValueType valueType, List<object> filteredValues)
        {
            switch (valueType)
            {
                case UnifiedValueType.Unknown:
                    throw new BusinessException(this.UnifiedField, FieldsMessages.UnknownFieldTypeCannotHaveValuesSet);

                case UnifiedValueType.Identifier:
                case UnifiedValueType.Integer:
                    filteredValues.AddRange(values.Select(value => ConvertValue(value, valueType, Convert.ToInt64)).Cast<object>());
                    break;

                case UnifiedValueType.Numeric:
                    filteredValues.AddRange(values.Select(value => ConvertValue(value, valueType, Convert.ToDouble)).Cast<object>());
                    break;

                case UnifiedValueType.Currency:
                    filteredValues.AddRange(values.Select(value => ConvertValue(value, valueType, Convert.ToDecimal)).Cast<object>());
                    break;

                case UnifiedValueType.Date:

                    foreach (var value in values)
                    {
                        // Here we use the implicit nullable cast to catch both nullable and value date types.
                        var dateTimeOffset = value as DateTimeOffset?;

                        if (dateTimeOffset != null)
                        {
                            filteredValues.Add(value);
                            continue;
                        }

                        var dateTime = value as DateTime?;

                        if (dateTime != null)
                        {
                            filteredValues.Add(new DateTimeOffset(dateTime.GetValueOrDefault()));
                            continue;
                        }

                        var dateTimeString = Convert.ToString(value);
                        DateTimeOffset dateTimeOffsetValue;

                        if (DateTimeOffset.TryParse(dateTimeString, out dateTimeOffsetValue))
                        {
                            filteredValues.Add(dateTimeOffsetValue);
                            continue;
                        }

                        var message = string.Format(FieldsMessages.ValueCannotBeConvertedToValueType, value, valueType);
                        throw new BusinessException(value, message);
                    }

                    break;
                case UnifiedValueType.Text:
                    filteredValues.AddRange(values.Select(Convert.ToString));
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(valueType));
            }
        }
    }
}
