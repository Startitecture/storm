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
        /// <param name="itemAttribute">
        /// The item attribute.
        /// </param>
        /// <param name="filterType">
        /// The filter type for this value filter.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        public ValueFilter(EntityAttributeDefinition itemAttribute, FilterType filterType, params object[] values)
        {
            this.ItemAttribute = itemAttribute;
            this.FilterType = filterType;
            this.values.AddRange(values);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the item attribute for the current filter.
        /// </summary>
        public EntityAttributeDefinition ItemAttribute { get; }

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
            return string.Format(ToStringFormat, this.ItemAttribute.PropertyName, string.Join(ValueSeparator, this.FilterValues));
        }

        #endregion
    }
}