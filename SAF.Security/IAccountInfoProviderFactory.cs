// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAccountInfoProviderFactory.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for classes that create account information providers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Security
{
    /// <summary>
    /// Provides an interface for classes that create account information providers.
    /// </summary>
    public interface IAccountInfoProviderFactory
    {
        /// <summary>
        /// Creates an <see cref="IAccountInfoProvider"/> instance.
        /// </summary>
        /// <returns>
        /// An <see cref="IAccountInfoProvider"/> instance.
        /// </returns>
        IAccountInfoProvider Create();
    }
}