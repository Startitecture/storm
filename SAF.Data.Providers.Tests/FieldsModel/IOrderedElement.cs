// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOrderedElement.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The IOrderedElement interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System;

    using JetBrains.Annotations;

    using SAF.Core;
    using SAF.Testing.Common;

    /// <summary>
    /// The IOrderedElement interface.
    /// </summary>
    public interface IOrderedElement
    {
        /// <summary>
        /// Gets the order of the element in the container.
        /// </summary>
        short Order { get; }

        /// <summary>
        /// Sets order of the current element.
        /// </summary>
        /// <param name="searcher">
        /// The ordered element searcher.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="searcher"/> is null.
        /// </exception>
        /// <exception cref="BusinessException">
        /// The current element is not in the searcher's list.
        /// </exception>
        void SetOrder([NotNull] IOrderedElementSearcher searcher);
    }
}