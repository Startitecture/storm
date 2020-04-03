// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueFilter.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Contains a filter for a specific item attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Query
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Model;

    /// <summary>
    /// Contains a filter for a specific item attribute.
    /// </summary>
    public class ValueFilter : IEquatable<ValueFilter>
    {
        /// <summary>
        /// Selects all of the non-null values from the value filter.
        /// </summary>
        public static readonly Func<ValueFilter, IEnumerable<object>> SelectNonNullValues = x => x.FilterValues.Where(v => v != null); 

        #region Constants

        /// <summary>
        /// The to string format.
        /// </summary>
        private const string ToStringFormat = "{0}[{1}]";

        /// <summary>
        /// The value separator.
        /// </summary>
        private const string ValueSeparator = ",";

        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<ValueFilter, object>[] ComparisonProperties =
            {
                item => item.AttributeLocation,
                item => item.FilterType,
                item => item.values
            };

        #endregion

        #region Fields

        /// <summary>
        /// The values.
        /// </summary>
        private readonly List<object> values = new List<object>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueFilter"/> class.
        /// </summary>
        /// <param name="attributeLocation">
        /// The attribute location.
        /// </param>
        /// <param name="filterType">
        /// The filter type.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        public ValueFilter([NotNull] AttributeLocation attributeLocation, FilterType filterType, [NotNull] params object[] values)
        {
            if (attributeLocation == null)
            {
                throw new ArgumentNullException(nameof(attributeLocation));
            }

            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            this.AttributeLocation = attributeLocation;
            this.FilterType = filterType;
            this.values.AddRange(values);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueFilter"/> class.
        /// </summary>
        /// <param name="attributeExpression">
        /// The attribute expression.
        /// </param>
        /// <param name="filterType">
        /// The filter type for this value filter.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="attributeExpression"/> is null.
        /// </exception>
        public ValueFilter([NotNull] LambdaExpression attributeExpression, FilterType filterType, [NotNull] params object[] values)
        {
            if (attributeExpression == null)
            {
                throw new ArgumentNullException(nameof(attributeExpression));
            }

            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            ////this.PropertyName = attributeExpression.GetPropertyName();
            this.AttributeLocation = new AttributeLocation(attributeExpression);
            this.FilterType = filterType;
            this.values.AddRange(values);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the attribute location.
        /// </summary>
        public AttributeLocation AttributeLocation { get; }

        /// <summary>
        /// Gets the filter type for this value filter.
        /// </summary>
        public FilterType FilterType { get; }

        /// <summary>
        /// Gets the filter values for the current filter.
        /// </summary>
        public IEnumerable<object> FilterValues => this.values;

        #endregion

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
        public static bool operator ==(ValueFilter valueA, ValueFilter valueB)
        {
            return EqualityComparer<ValueFilter>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(ValueFilter valueA, ValueFilter valueB)
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
        public bool Equals(ValueFilter other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, ToStringFormat, this.AttributeLocation, string.Join(ValueSeparator, this.FilterValues));
        }

        #endregion
    }
}