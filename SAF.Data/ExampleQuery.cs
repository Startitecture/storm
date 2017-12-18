// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExampleQuery.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains data for a convention-based query.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;

    using SAF.Core;

    /// <summary>
    /// Contains data for a convention-based query.
    /// </summary>
    /// <typeparam name="TExample">
    /// The type of item that is the example for the query.
    /// </typeparam>
    public class ExampleQuery<TExample> : IExampleQuery<TExample>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExampleQuery{TExample}"/> class.
        /// </summary>
        public ExampleQuery()
            : this(default(TExample))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExampleQuery{TExample}"/> class.
        /// </summary>
        /// <param name="baselineExample">
        /// The baseline example.
        /// </param>
        /// <param name="propertiesToInclude">
        /// The properties to include.
        /// </param>
        public ExampleQuery(TExample baselineExample, params Expression<Func<TExample, object>>[] propertiesToInclude)
            : this(baselineExample, default(TExample), propertiesToInclude)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExampleQuery{TExample}"/> class.
        /// </summary>
        /// <param name="baselineExample">
        /// The baseline example.
        /// </param>
        /// <param name="boundaryExample">
        /// The boundary example.
        /// </param>
        /// <param name="propertiesToInclude">
        /// The properties to include.
        /// </param>
        public ExampleQuery(
            TExample baselineExample,
            TExample boundaryExample,
            params Expression<Func<TExample, object>>[] propertiesToInclude)
        {
            this.BoundaryExample = boundaryExample;
            this.BaselineExample = baselineExample;
            this.PropertiesToInclude = new Collection<string>(propertiesToInclude.Select(x => x.GetPropertyName()).ToList());
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether null properties require a set value.
        /// </summary>
        public bool NullPropertiesRequireSetValue { get; set; }

        /// <summary>
        /// Gets a value that serves as the baseline example for the query. If the boundary example is set, this property should be
        /// used as the lower bound of an inclusive range query (i.e., using BETWEEN). Otherwise, this property should be used to 
        /// generate a query against the indexed columns of the entity. If this property is not set, all entities should be returned.
        /// </summary>
        public TExample BaselineExample { get; private set; }

        /// <summary>
        /// Gets a value that serves as the boundary example for the query. If left unset, the baseline example will be 
        /// used to generate a query against the indexed columns of the entity.
        /// </summary>
        public TExample BoundaryExample { get; private set; }

        /// <summary>
        /// Gets or sets the maximum number of items to return. Values of zero or less indicate no limit.
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Gets or sets the page size. Values of zero or less indicate all items will be retrieved.
        /// </summary>
        public long PageSize { get; set; }

        /// <summary>
        /// Gets or sets the 1-based page number to retrieve. Values of zero or less indicate all items will be retrieved.
        /// </summary>
        public long Page { get; set; }

        /// <summary>
        /// Gets the property names stored in the collection.
        /// </summary>
        public Collection<string> PropertiesToInclude { get; private set; }

        #endregion
    }
}