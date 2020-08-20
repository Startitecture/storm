// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepositoryProviderFactory.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface to factories that create repository providers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
{
    /// <summary>
    /// Provides an interface to factories that create repository providers.
    /// </summary>
    public interface IRepositoryProviderFactory
    {
        /// <summary>
        /// Creates an <see cref="IRepositoryProvider"/>. 
        /// </summary>
        /// <param name="connectionString">
        /// The connection string to the repository.
        /// </param>
        /// <returns>
        /// An <see cref="IRepositoryProvider"/>.
        /// </returns>
        IRepositoryProvider Create(string connectionString);
    }
}