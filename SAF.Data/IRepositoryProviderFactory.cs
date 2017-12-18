// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepositoryProviderFactory.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface to factories that create repository providers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data
{
    /// <summary>
    /// Provides an interface to factories that create repository providers.
    /// </summary>
    public interface IRepositoryProviderFactory
    {
        /// <summary>
        /// Creates an <see cref="T:SAF.Data.IRepositoryProvider"/>. 
        /// </summary>
        /// <returns>
        /// An <see cref="T:SAF.Data.IRepositoryProvider"/>.
        /// </returns>
        IRepositoryProvider Create();
    }
}