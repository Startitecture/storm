// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOrderedElementSearcher.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The OrderedElementSearcher interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// The OrderedElementSearcher interface.
    /// </summary>
    public interface IOrderedElementSearcher
    {
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
        short GetOrder([NotNull] IOrderedElement orderedElement);
    }
}