// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnifiedField.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SAF.Testing.Common;

    using Startitecture.Core;

    /// <summary>
    /// The unified field.
    /// </summary>
    public class UnifiedField : IEquatable<UnifiedField>, IComparable<UnifiedField>, IComparable
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<UnifiedField, object>[] ComparisonProperties =
            {
                item => item.Name,
                item => item.Caption,
                item => item.Label,
                item => item.UnifiedFieldType,
                item => item.UnifiedValueType,
                item => item.CustomFieldId,
                item => item.SystemFieldSourceId
            };

        /// <summary>
        /// The identifier field types.
        /// </summary>
        private static readonly IEnumerable<UnifiedFieldType> IdentifierFieldTypes = new[]
                                                                                         {
                                                                                             UnifiedFieldType.Checkbox,
                                                                                             UnifiedFieldType.RadioButton,
                                                                                             UnifiedFieldType.DropDown,
                                                                                             UnifiedFieldType.PickList
                                                                                         };

        /// <summary>
        /// The numeric value types.
        /// </summary>
        private static readonly IEnumerable<UnifiedValueType> NumericValueTypes = new[]
                                                                                      {
                                                                                          UnifiedValueType.Currency,
                                                                                          UnifiedValueType.Decimal,
                                                                                          UnifiedValueType.Integer
                                                                                      };

        /// <summary>
        /// Initializes a new instance of the <see cref="UnifiedField"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="unifiedFieldType">
        /// The unified field type.
        /// </param>
        /// <param name="unifiedValueType">
        /// The unified value type.
        /// </param>
        /// <param name="unifiedFieldId">
        /// The unified field ID.
        /// </param>
        /// <param name="customFieldId">
        /// The custom field ID.
        /// </param>
        /// <param name="systemFieldId">
        /// The system field ID.
        /// </param>
        public UnifiedField(
            string name,
            UnifiedFieldType unifiedFieldType,
            UnifiedValueType unifiedValueType,
            int? unifiedFieldId = null,
            int? customFieldId = null,
            int? systemFieldId = null)
        {
            this.Name = name;
            this.UnifiedFieldType = unifiedFieldType;
            this.UnifiedValueType = unifiedValueType;
            this.UnifiedFieldId = unifiedFieldId;
            this.CustomFieldId = customFieldId;
            this.SystemFieldSourceId = systemFieldId;
        }

        /// <summary>
        /// Gets the unified field ID.
        /// </summary>
        public int? UnifiedFieldId { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the caption.
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets the unified field type.
        /// </summary>
        public UnifiedFieldType UnifiedFieldType { get; private set; }

        /// <summary>
        /// Gets the unified value type.
        /// </summary>
        public UnifiedValueType UnifiedValueType { get; private set; }

        /// <summary>
        /// Gets the system field source ID.
        /// </summary>
        public int? SystemFieldSourceId { get; private set; }

        /// <summary>
        /// Gets the custom field source ID.
        /// </summary>
        public int? CustomFieldId { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the current Unified Field is a custom field.
        /// </summary>
        public bool IsCustomField => this.CustomFieldId.HasValue;

        /// <summary>
        /// Gets a value indicating whether the current Unified Field is a system field.
        /// </summary>
        public bool IsSystemField => this.SystemFieldSourceId.HasValue;

        /// <summary>
        /// Gets a value indicating whether the current Unified Field is a user sourced field.
        /// </summary>
        public bool IsUserSourcedField => IdentifierFieldTypes.Contains(this.UnifiedFieldType) == false;

        /// <summary>
        /// Gets a value indicating whether the current Unified Field has date values.
        /// </summary>
        public bool IsDateValueType => this.UnifiedValueType == UnifiedValueType.Date;

        /// <summary>
        /// Gets a value indicating whether the current Unified Field has text values.
        /// </summary>
        public bool IsTextValueType => this.UnifiedValueType == UnifiedValueType.Text;

        /// <summary>
        /// Gets a value indicating whether the current Unified Field has numeric values.
        /// </summary>
        public bool IsNumericValueType => NumericValueTypes.Contains(this.UnifiedValueType);

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
        public static bool operator ==(UnifiedField valueA, UnifiedField valueB)
        {
            return EqualityComparer<UnifiedField>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(UnifiedField valueA, UnifiedField valueB)
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
        public static bool operator <(UnifiedField valueA, UnifiedField valueB)
        {
            return Comparer<UnifiedField>.Default.Compare(valueA, valueB) < 0;
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
        public static bool operator >(UnifiedField valueA, UnifiedField valueB)
        {
            return Comparer<UnifiedField>.Default.Compare(valueA, valueB) > 0;
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
        public int CompareTo(UnifiedField other)
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
            return $"{this.Name} ({this.UnifiedFieldType} {this.UnifiedValueType}";
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
        public bool Equals(UnifiedField other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        /// <summary>
        /// Sources the current field from the specified field source.
        /// </summary>
        /// <param name="fieldSource">
        /// The field source to associate with the field.
        /// </param>
        /// <exception cref="BusinessException">
        /// The <see cref="UnifiedFieldSource.SourceType"/> of <paramref name="fieldSource"/> is not
        /// <see cref="UnifiedFieldSourceType.SystemField"/> or <see cref="UnifiedFieldSourceType.CustomField"/>.
        /// </exception>
        public void SourceFrom(UnifiedFieldSource fieldSource)
        {
            switch (fieldSource.SourceType)
            {
                case UnifiedFieldSourceType.SystemField:
                    this.SystemFieldSourceId = fieldSource.SourceId;
                    break;
                case UnifiedFieldSourceType.CustomField:
                    this.CustomFieldId = fieldSource.SourceId;
                    break;
                default:
                    throw new BusinessException(fieldSource, "Field source type not supported.");
            }
        }
    }
}