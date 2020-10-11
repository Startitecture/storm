// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValueFilter.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides an interface for a value filter on a specific attribute.
    /// </summary>
    public interface IValueFilter
    {
        /// <summary>
        /// Gets the attribute location.
        /// </summary>
        AttributeLocation AttributeLocation { get; }

        /// <summary>
        /// Gets the filter type for this value filter.
        /// </summary>
        FilterType FilterType { get; }

        /// <summary>
        /// Gets the filter values for the current filter.
        /// </summary>
        IEnumerable<object> FilterValues { get; }
    }
}