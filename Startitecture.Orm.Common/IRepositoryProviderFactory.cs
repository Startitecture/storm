// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepositoryProviderFactory.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
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
        /// Creates an <see cref="T:Startitecture.Orm.Common.IRepositoryProvider"/>. 
        /// </summary>
        /// <returns>
        /// An <see cref="T:Startitecture.Orm.Common.IRepositoryProvider"/>.
        /// </returns>
        IRepositoryProvider Create();
    }
}