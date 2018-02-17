// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderedElementSearcher.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The ordered element sorter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    using Startitecture.Core;

    /// <summary>
    /// The ordered element sorter.
    /// </summary>
    /// <typeparam name="T">
    /// The type of ordered element to search for.
    /// </typeparam>
    public sealed class OrderedElementSearcher<T> : IOrderedElementSearcher
        where T : IOrderedElement
    {
        /// <summary>
        /// The ordered elements.
        /// </summary>
        private readonly List<IOrderedElement> orderedElements;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedElementSearcher{T}"/> class. 
        /// </summary>
        /// <param name="orderedElements">
        /// The ordered elements.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="orderedElements"/> is null.
        /// </exception>
        public OrderedElementSearcher([NotNull] SortedSet<T> orderedElements)
        {
            if (orderedElements == null)
            {
                throw new ArgumentNullException(nameof(orderedElements));
            }

            this.orderedElements = new List<IOrderedElement>(orderedElements.OfType<IOrderedElement>());
        }

        /// <summary>
        /// Gets the order of the element as a short.
        /// </summary>
        /// <param name="orderedElement">
        /// The ordered element.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="orderedElement"/> is null.
        /// </exception>
        /// <returns>
        /// The index of the element as a <see cref="short"/>, or the negative bitwise value if the element is not found, or the 
        /// negative bitwise value of the item count if no larger item is found.
        /// </returns>
        public short GetOrder(IOrderedElement orderedElement)
        {
            if (orderedElement == null)
            {
                throw new ArgumentNullException(nameof(orderedElement));
            }

            return Convert.ToInt16(this.orderedElements.BinarySearch(orderedElement, Singleton<OrderedElementComparer>.Instance) + 1);
        }
    }
}