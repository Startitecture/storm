// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPropertyNameMapper.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers
{
    /// <summary>
    /// The PropertyNameMapper interface.
    /// </summary>
    /// <typeparam name="TDataItem">
    /// The type of data item to map property names for.
    /// </typeparam>
    public interface IPropertyNameMapper<TDataItem>
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
        PropertyNameSelection<TDataItem> Map<TItem>(IExampleQuery<TItem> query);
    }
}