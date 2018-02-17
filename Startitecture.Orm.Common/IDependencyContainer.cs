// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDependencyContainer.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for classes that contain a list of dependencies in an object graph.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Provides an interface for classes that contain a list of dependencies in an object graph.
    /// </summary>
    public interface IDependencyContainer
    {
        /// <summary>
        /// Sets the dependency for the specified key.
        /// </summary>
        /// <param name="key">
        /// The key for the dependency.
        /// </param>
        /// <param name="dependency">
        /// The dependency to associate with the key.
        /// </param>
        /// <typeparam name="TDependency">
        /// The type of dependency to associate with the key. 
        /// </typeparam>
        void SetDependency<TDependency>(object key, TDependency dependency);

        /// <summary>
        /// Gets the dependency for the specified key.
        /// </summary>
        /// <param name="key">
        /// The key for the dependency.
        /// </param>
        /// <typeparam name="TDependency">
        /// The type of dependency to associate with the key. 
        /// </typeparam>
        /// <returns>
        /// The dependency associated with the key, or <c>null</c> if the key does not exist.
        /// </returns>
        TDependency GetDependency<TDependency>(object key);

        /// <summary>
        /// Determines whether the dependency container contains the specified dependency.
        /// </summary>
        /// <param name="key">
        /// The key for the dependency.
        /// </param>
        /// <typeparam name="TDependency">
        /// The type of dependency associated with the key. 
        /// </typeparam>
        /// <returns>
        /// <c>true</c> if the container contains a dependency matching the type and with the specified key; otherwise, <c>false</c>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Allows fluent usage of the method.")]
        bool ContainsDependency<TDependency>(object key);
    }
}