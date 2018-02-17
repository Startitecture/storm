// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheResult.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Contains the result of a cache query.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Core
{
    /// <summary>
    /// Contains the result of a cache query.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of item being selected by the query.
    /// </typeparam>
    public class CacheResult<TItem>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheResult{TItem}"/> class.
        /// </summary>
        /// <param name="item">
        /// The item retrieved from the cache, if any.
        /// </param>
        /// <param name="hit">
        /// A value indicating whether the item was retrieved from the cache.
        /// </param>
        /// <param name="key">
        /// The key used to retrieve the item from the cache.
        /// </param>
        public CacheResult(TItem item, bool hit, string key)
        {
            this.Key = key;
            this.Hit = hit;
            this.Item = item;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether hit.
        /// </summary>
        public bool Hit { get; private set; }

        /// <summary>
        /// Gets the item.
        /// </summary>
        public TItem Item { get; private set; }

        /// <summary>
        /// Gets the key.
        /// </summary>
        public string Key { get; private set; }

        #endregion
    }
}