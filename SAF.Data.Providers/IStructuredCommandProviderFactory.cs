// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStructuredCommandProviderFactory.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The StructuredCommandProviderFactory interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers
{
    using JetBrains.Annotations;

    /// <summary>
    /// The StructuredCommandProviderFactory interface.
    /// </summary>
    public interface IStructuredCommandProviderFactory
    {
        /// <summary>
        /// Creates an <see cref="IStructuredCommandProvider"/> for the specified <paramref name="databaseContextProvider"/>.
        /// </summary>
        /// <param name="databaseContextProvider">
        /// The database context provider.
        /// </param>
        /// <returns>
        /// A <see cref="IStructuredCommandProvider"/> instance for the specified <paramref name="databaseContextProvider"/>.
        /// </returns>
        IStructuredCommandProvider Create([NotNull] IDatabaseContextProvider databaseContextProvider);
    }
}