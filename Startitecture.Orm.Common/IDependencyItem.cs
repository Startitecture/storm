// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDependencyItem.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for items that are dependencies in an object graph.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
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
