// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectPropertyNameMapper.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers
{
    using System;
    using System.Linq;

    using JetBrains.Annotations;

    /// <summary>
    /// Maps property names by directly assigning the source property name to the target property name.
    /// </summary>
    /// <typeparam name="TDataItem">
    /// The type of the data item to map properties to.
    /// </typeparam>
    public class DirectPropertyNameMapper<TDataItem> : IPropertyNameMapper<TDataItem>
    {
        /// <summary>
        /// Maps the property names included in the example query to a property name selection for the
        /// <typeparamref name="TDataItem"/> type.
        /// </summary>
        /// <param name="query">
        /// The query to map.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item represented in the query.
        /// </typeparam>
        /// <returns>
        /// A <see cref="PropertyNameSelection{TDataItem}"/> for the <typeparamref name="TDataItem"/> type.
        /// </returns>
        public PropertyNameSelection<TDataItem> Map<TItem>([NotNull] IExampleQuery<TItem> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return new PropertyNameSelection<TDataItem>(query.PropertiesToInclude.ToArray());
        }
    }
}