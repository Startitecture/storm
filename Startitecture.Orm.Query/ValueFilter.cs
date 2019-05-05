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
    using System.Linq;
    using System.Linq.Expressions;

    using JetBrains.Annotations;

    using Startitecture.Orm.Model;

    /// <summary>
    /// Contains a filter for a specific item attribute.
    /// </summary>
    public class ValueFilter
    {
        /// <summary>
        /// Selects all of the values from the value filter.
        /// </summary>
        public static readonly Func<ValueFilter, IEnumerable<object>> SelectAllValues = x => x.FilterValues;

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

        /////// <summary>
        /////// Initializes a new instance of the <see cref="ValueFilter"/> class.
        /////// </summary>
        /////// <param name="propertyName">
        /////// The property name.
        /////// </param>
        /////// <param name="filterType">
        /////// The filter type for this value filter.
        /////// </param>
        /////// <param name="values">
        /////// The values.
        /////// </param>
        /////// <exception cref="ArgumentException">
        /////// <paramref name="propertyName"/> is null or whitespace.
        /////// </exception>
        ////public ValueFilter([NotNull] string propertyName, FilterType filterType, params object[] values)
        ////{
        ////    if (string.IsNullOrWhiteSpace(propertyName))
        ////    {
        ////        throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(propertyName));
        ////    }

        ////    this.PropertyName = propertyName;
        ////    this.FilterType = filterType;
        ////    this.values.AddRange(values);
        ////}

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

        /////// <summary>
        /////// Gets the property name.
        /////// </summary>
        ////public string PropertyName { get; }

        /// <summary>
        /// Gets the filter type for this value filter.
        /// </summary>
        public FilterType FilterType { get; }

        /// <summary>
        /// Gets the filter values for the current filter.
        /// </summary>
        public IEnumerable<object> FilterValues => this.values;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(ToStringFormat, this.AttributeLocation, string.Join(ValueSeparator, this.FilterValues));
        }

        #endregion
    }
}