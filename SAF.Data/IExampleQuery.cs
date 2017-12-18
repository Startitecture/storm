// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExampleQuery.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface to a query by example entity query.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data
{
    /// <summary>
    /// Provides an interface to a query by example entity query.
    /// </summary>
    /// <typeparam name="TExample">
    /// The type of object that contains properties for the example-based query.
    /// </typeparam>
    public interface IExampleQuery<out TExample> : IPropertyNameSelection
    {
        /// <summary>
        /// Gets or sets the maximum number of items to return. Values of zero or less indicate no limit.
        /// </summary>
        int Limit { get; set; }

        /// <summary>
        /// Gets or sets the 1-based page number to retrieve. Values of zero or less indicate all items will be retrieved.
        /// </summary>
        long Page { get; set; }

        /// <summary>
        /// Gets or sets the page size. Values of zero or less indicate all items will be retrieved.
        /// </summary>
        long PageSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether null properties require a set value.
        /// TODO: Fix this hack by allowing each value to indicate nullity.
        /// </summary>
        bool NullPropertiesRequireSetValue { get; set; }

        /// <summary>
        /// Gets a value that serves as the baseline example for the query. If the boundary example is set, this property should be
        /// used as the lower bound of an inclusive range query (i.e., using BETWEEN). Otherwise, this property should be used to 
        /// generate a query against the indexed columns of the entity. If this property is not set, all entities should be returned.
        /// </summary>
        TExample BaselineExample { get; }

        /// <summary>
        /// Gets a value that serves as the boundary example for the query. If left unset, the baseline example will be 
        /// used to generate a query against the indexed columns of the entity.
        /// </summary>
        TExample BoundaryExample { get; }
    }
}
