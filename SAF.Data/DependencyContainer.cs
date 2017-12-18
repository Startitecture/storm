// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyContainer.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Allows setting and retrieval of dependencies for an object graph.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Allows setting and retrieval of dependencies for an object graph.
    /// </summary>
    public sealed class DependencyContainer : IDependencyContainer, IDisposable
    {
        /// <summary>
        /// The dependencies.
        /// </summary>
        private readonly Dictionary<string, object> dependencies = new Dictionary<string, object>();

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
        public void SetDependency<TDependency>(object key, TDependency dependency)
        {
            var keyString = CreateKey<TDependency>(key);

            if (this.dependencies.ContainsKey(keyString))
            {
                this.dependencies[keyString] = dependency;
            }
            else
            {
                this.dependencies.Add(keyString, dependency);
            }
        }

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
        public TDependency GetDependency<TDependency>(object key)
        {
            var keyString = CreateKey<TDependency>(key);

            if (this.dependencies.TryGetValue(keyString, out var value))
            {
                return (TDependency)value;
            }

            return default(TDependency);
        }

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
        public bool ContainsDependency<TDependency>(object key)
        {
            var keyString = CreateKey<TDependency>(key);

            var containsKey = this.dependencies.TryGetValue(keyString, out var value);

            return containsKey && value is TDependency;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            // Don't allow objects loaded into the dependency cache to pin this object.
            this.dependencies.Clear();
        }

        /// <summary>
        /// Creates a key based on the provided key and dependency type.
        /// </summary>
        /// <param name="key">
        /// The key that identifies the dependency.
        /// </param>
        /// <typeparam name="TDependency">
        /// The type of dependency to identify.
        /// </typeparam>
        /// <returns>
        /// The dependency type and key value concatenated as a <see cref="string"/>.
        /// </returns>
        private static string CreateKey<TDependency>(object key)
        {
            return string.Concat(typeof(TDependency).FullName, ':', key);
        }
    }
}
