// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MapResult.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    /// <summary>
    /// Contains the result of a mapping operation.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the object that was mapped to.
    /// </typeparam>
    public class MapResult<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapResult{T}"/> class.
        /// </summary>
        /// <param name="item">
        /// The item that was mapped.
        /// </param>
        /// <param name="values">
        /// The values that were mapped.
        /// </param>
        public MapResult([NotNull] T item, [NotNull] Dictionary<string, object> values)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            this.Item = item;
            this.Values = values ?? throw new ArgumentNullException(nameof(values));
        }

        /// <summary>
        /// Gets the item that was mapped from the source.
        /// </summary>
        public T Item { get; }

        /// <summary>
        /// Gets all the property names and values that were mapped from the source.
        /// </summary>
        public Dictionary<string, object> Values { get; }
    }
}