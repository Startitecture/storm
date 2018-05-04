// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Cache.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   A cache implementation for data providers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;

    /// <summary>
    /// A cache implementation for data providers.
    /// </summary>
    /// <typeparam name="TKey">
    /// The type of key stored in the cache.
    /// </typeparam>
    /// <typeparam name="TValue">
    /// The type of value stored in the cache.
    /// </typeparam>
    public sealed class Cache<TKey, TValue> : IDisposable
    {
        #region Fields

        /// <summary>
        /// The cache lock.
        /// </summary>
        private readonly ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

        /// <summary>
        /// The cache map.
        /// </summary>
        private readonly Dictionary<TKey, TValue> cacheMap = new Dictionary<TKey, TValue>();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count => this.cacheMap.Count;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The flush.
        /// </summary>
        public void Flush()
        {
            // Cache it
            this.cacheLock.EnterWriteLock();

            try
            {
                this.cacheMap.Clear();
            }
            finally
            {
                this.cacheLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Gets the item from the cache.
        /// </summary>
        /// <param name="key">
        /// The cache key.
        /// </param>
        /// <param name="factory">
        /// The factory that creates the value.
        /// </param>
        /// <returns>
        /// The value created by the factory.
        /// </returns>
        public TValue Get(TKey key, Func<TValue> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            // Check cache
            this.cacheLock.EnterReadLock();
            TValue value;

            try
            {
                if (this.cacheMap.TryGetValue(key, out value))
                {
                    Trace.TraceInformation($"{key}: Got '{value}' from cache.");
                    return value;
                }
            }
            finally
            {
                this.cacheLock.ExitReadLock();
            }

            // Cache it
            this.cacheLock.EnterWriteLock();

            try
            {
                // Check again
                if (this.cacheMap.TryGetValue(key, out value))
                {
                    Trace.TraceInformation($"{key}: Got '{value}' from cache (attempt 2).");
                    return value;
                }

                // Create it
                var watch = Stopwatch.StartNew();
                value = factory();
                watch.Stop();
                ////Trace.TraceInformation($"{key}: Got '{value}' from factory: {watch.Elapsed}.");

                // Store it
                this.cacheMap.Add(key, value);

                // Done
                return value;
            }
            finally
            {
                this.cacheLock.ExitWriteLock();
            }
        }

        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.cacheLock?.Dispose();
        }
    }
}