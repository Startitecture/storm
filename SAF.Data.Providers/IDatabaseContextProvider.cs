// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDatabaseContextProvider.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers
{
    /// <summary>
    /// Provides an interface for classes that contain database contexts.
    /// </summary>
    public interface IDatabaseContextProvider
    {
        /// <summary>
        /// Gets the current database context.
        /// </summary>
        IDatabaseContext DatabaseContext { get; }
    }
}
