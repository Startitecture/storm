// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeQuery.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.EntityServices.Tests
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.Serialization;

    using SAF.Core;
    using SAF.Data;
    using SAF.Testing.Common;

    /// <summary>
    /// The fake query.
    /// </summary>
    [DataContract]
    [ExcludeFromCodeCoverage]
    public class FakeQuery : IExampleQuery<FakeDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeQuery"/> class.
        /// </summary>
        /// <param name="selectors">
        /// The selectors.
        /// </param>
        public FakeQuery(params Expression<Func<FakeDto, object>>[] selectors)
        {
            this.PropertiesToInclude = new Collection<string>(selectors.Select(x => x.GetPropertyName()).ToArray());
        }

        /// <summary>
        /// Gets or sets the limit.
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        public long Page { get; set; }

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        public long PageSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether null properties require set value.
        /// </summary>
        public bool NullPropertiesRequireSetValue { get; set; }

        /// <summary>
        /// Gets or sets the baseline example.
        /// </summary>
        [DataMember]
        public FakeDto BaselineExample { get; set; }

        /// <summary>
        /// Gets or sets the boundary example.
        /// </summary>
        [DataMember]
        public FakeDto BoundaryExample { get; set; }

        /// <summary>
        /// Gets or sets the properties to include.
        /// </summary>
        [DataMember]
        public Collection<string> PropertiesToInclude { get; set; }
    }
}
