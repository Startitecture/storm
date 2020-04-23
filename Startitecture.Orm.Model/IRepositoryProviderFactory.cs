// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepositoryProviderFactory.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface to factories that create repository providers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    /// <summary>
    /// Provides an interface to factories that create repository providers.
    /// </summary>
    public interface IRepositoryProviderFactory
    {
        /// <summary>
        /// Creates an <see cref="IRepositoryProvider"/>. 
        /// </summary>
        /// <returns>
        /// An <see cref="IRepositoryProvider"/>.
        /// </returns>
        IRepositoryProvider Create();
    }
}