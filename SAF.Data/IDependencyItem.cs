// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDependencyItem.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for items that are dependencies in an object graph.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data
{
    /// <summary>
    /// Provides an interface for items that are dependencies in an object graph.
    /// </summary>
    public interface IDependencyItem
    {
        /// <summary>
        /// Gets the dependency container.
        /// </summary>
        IDependencyContainer DependencyContainer { get; }

        /// <summary>
        /// Sets the dependency container for the item.
        /// </summary>
        /// <param name="container">
        /// The container that the item should use.
        /// </param>
        void SetDependencyContainer(IDependencyContainer container);
    }
}
